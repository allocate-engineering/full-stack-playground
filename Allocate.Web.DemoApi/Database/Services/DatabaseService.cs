using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Allocate.Common.Database.DataModels;
using Allocate.Common.Database.Exceptions;
using Allocate.Common.Database.Settings;

using Dapper;
using Dapper.Contrib.Extensions;

using Npgsql;

namespace Allocate.Common.Database.Services;

public interface IDatabaseService
{
    T Get<T>(Guid id) where T : DataModelBase;

    /// <summary>
    /// Get a record matching the criteria specified in the build action
    /// </summary>
    /// <param name="build">Action used to construct the query being executed.</param>
    /// <returns>A record from table T matching the build</returns>
    T Get<T>(Action<SqlBuilder> build) where T : DataModelBase;

    IEnumerable<T> GetMultiple<T>(IEnumerable<Guid> ids) where T : DataModelBase;

    ResultSet<T> GetAll<T>() where T : DataModelBase;

    IEnumerable<T> GetAllFromQuery<T>(string parameterizedQuery, object param = null) where T : DataModelBase;

    T GetScalarFromQuery<T>(string parameterizedQuery, object param = null) where T : struct;

    IEnumerable<string> GetAllStringsFromQuery(string parameterizedQuery, object param = null);

    IEnumerable<Guid> GetAllIdsFromQuery(string parameterizedQuery, object param = null);

    /// <summary>
    /// Returns records matching the query as defined by the build, and 
    /// optionally filtered using the specified Pager
    /// </summary>
    /// <param name="build">Action used to construct the query being executed.</param>
    /// <param name="pager">Pager used to split the results into pages</param>
    /// <param name="template">A custom query builder template used to format the resulting raw SQL</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>The records from table T matching the build and filtered by any defined pager</returns>
    ResultSet<T> GetAll<T>(Action<SqlBuilder> build, Pager pager = null, string template = null) where T : DataModelBase;

    int CountRows<T>(Action<SqlBuilder> build, string template = null) where T : DataModelBase;

    /// <summary>
    /// Updates or inserts the object with the given id. Will set the Id on the object if it is empty.
    /// </summary>
    void Save<T>(T obj, Guid modifiedBy, bool isInsertingWithPresetId = false) where T : DataModelBase;

    void SaveMultiple<T>(IEnumerable<T> objs, Guid modifiedBy) where T : DataModelBase;

    void SanityCheckPager(int totalRowsWithoutPagination, Pager pager);

    /// <summary>
    /// Execute parameterized SQL.
    /// </summary>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

    DateTime PerformHealthCheck();
}

public class Pager
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Offset { get; set; }
    private const int DEFAULT_PAGE_SIZE = 10;
    private const int DEFAULT_PAGE_NUMBER = 1;

    public Pager(int? page, int? pageSize)
    {
        Page = page ?? -1;
        if (Page < 1)
        {
            Page = DEFAULT_PAGE_NUMBER;
        }
        PageSize = pageSize ?? -1;
        if (PageSize < 1)
        {
            PageSize = DEFAULT_PAGE_SIZE;
        }
        Offset = (Page - 1) * PageSize;
    }
}

public class ResultSet<T> : List<T>
{
    public ResultSet()
    {
        TotalPages = 0;
        TotalResults = 0;
    }

    public ResultSet(IEnumerable<T> results, int totalPages, int totalResults) : base(results)
    {
        TotalPages = totalPages;
        TotalResults = totalResults;
    }

    public int TotalPages { get; private set; }
    public int TotalResults { get; private set; }
}

public class DatabaseService : IDatabaseService
{
    // _logger is used in the LogDebugMessage but its usage could be commented out, so ignore the warning
    [SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "<Pending>")]

    private readonly string _connectionString;

    // For debugging
    private int totalGetsFromDatabase = 0;

    public DatabaseService(DatabaseSettings databaseSettings)
    {
        _connectionString = databaseSettings.GetConnectionString();
    }

    private TResult UsingConnection<TResult>(Func<IDbConnection, TResult> command)
    {
        using (IDbConnection connection = new NpgsqlConnection(_connectionString))
        {
            return command(connection);
        }
    }

    private void UsingConnection(Action<IDbConnection> command)
    {
        var _ = UsingConnection(connection => { command(connection); return true; });
    }

    public T Get<T>(Guid id) where T : DataModelBase
    {
        return Get<T>(builder => builder.Where("\"Id\" = @Id", new { Id = id }));
    }

    public T Get<T>(Action<SqlBuilder> build) where T : DataModelBase
    {
        return GetAll<T>(build, pager: null).SingleOrDefault();
    }

    public IEnumerable<T> GetMultiple<T>(IEnumerable<Guid> ids) where T : DataModelBase
    {
        List<Guid> idsToRetrieveFromDatabase = new();

        // Remove any duplicate ids so we don't return duplicate objects
        HashSet<Guid> idSet = ids.ToHashSet();
        foreach (Guid id in idSet)
        {
            idsToRetrieveFromDatabase.Add(id);
        }

        // Go to the database for items not found in the cache
        IEnumerable<T> itemsToReturn = GetAll<T>(builder =>
            builder.Where("\"Id\" = ANY(@Ids)", new { Ids = idsToRetrieveFromDatabase }), pager: null);

        return itemsToReturn;
    }

    public ResultSet<T> GetAll<T>(Action<SqlBuilder> build, Pager pager = null, string template = null) where T : DataModelBase
    {
        template = template ?? GetDefaultTemplate<T>();

        var totalResults = 0;
        var totalPages = 1;

        // apply paging and calculate the total number of pages,
        // Note, these have their own exception handling logic so
        // this block should not be nested in the try-catch below
        if (pager is not null)
        {
            totalResults = CountRows<T>(build, template);
            SanityCheckPager(totalResults, pager);
            totalPages = (int)Math.Ceiling(totalResults / (double)pager.PageSize);
        }

        var builder = new SqlBuilder();
        var builderTemplate = pager is null ?
            builder.AddTemplate(template)
            : builder.AddTemplate($"{template} OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", new { pager.Offset, Limit = pager.PageSize });
        build(builder);

        var templateRawSql = builderTemplate.RawSql;
        LogCallingGetAll($"{typeof(T)} Template: {templateRawSql}");
        LogTotalGetsFromDatabase($"{typeof(T)}: {templateRawSql}");

        var results = UsingConnection(connection => connection.Query<T>(templateRawSql, builderTemplate.Parameters));

        if (pager is null)
        {
            // if no pager, then set the results to the query result count
            totalResults = results.Count();
        }

        return new ResultSet<T>(results, totalPages, totalResults) { };
    }

    public ResultSet<T> GetAll<T>() where T : DataModelBase
    {
        return GetAll<T>(builder => { /* no filters applied on unbounded GetAll */ });
    }

    public int CountRows<T>(Action<SqlBuilder> build, string template = null) where T : DataModelBase
    {
        template = template ?? GetDefaultTemplate<T>();
        // COUNT by wrapping the template query logic and stripping any orderby
        template = $"SELECT COUNT(*) FROM ( {template.Replace("/**orderby**/", "")} ) derived";

        var builder = new SqlBuilder();
        var builderTemplate = builder.AddTemplate(template);
        build(builder);
        int result = UsingConnection(connection => connection.ExecuteScalar<int>(builderTemplate.RawSql, builderTemplate.Parameters));
        return result;
    }

    public T GetScalarFromQuery<T>(string parameterizedQuery, object param = null) where T : struct
    {
        T result = UsingConnection(connection => connection.ExecuteScalar<T>(parameterizedQuery, param));
        return result;
    }

    public void Save<T>(T obj, Guid modifiedBy, bool isInsertingWithPresetId = false) where T : DataModelBase
    {
        try
        {
            var now = DateTimeOffset.UtcNow;

            UsingConnection(connection =>
            {
                if (obj.Id == Guid.Empty || isInsertingWithPresetId)
                {
                    if (obj.Id == Guid.Empty)
                    {
                        obj.Id = Guid.NewGuid();
                    }
                    connection.Insert(obj);
                }
                else
                {
                    connection.Update(obj);
                }
            });
        }
        catch (Exception e)
        {
            CheckForAlreadyExistsInDatabaseException(e);
            throw;
        }
    }

    public void SaveMultiple<T>(IEnumerable<T> objs, Guid modifiedBy) where T : DataModelBase
    {
        try
        {
            var updates = new List<T>();
            var inserts = new List<T>();
            var now = DateTimeOffset.UtcNow;

            foreach (var obj in objs)
            {
                if (obj.Id != Guid.Empty)
                {
                    updates.Add(obj);
                }
                else
                {
                    obj.Id = Guid.NewGuid();
                    inserts.Add(obj);
                }
            }

            UsingConnection(connection =>
            {
                if (updates.Count > 0)
                {
                    connection.Update(updates);
                }

                if (inserts.Count > 0)
                {
                    connection.Insert(inserts);
                }
            });

        }
        catch (Exception e)
        {
            CheckForAlreadyExistsInDatabaseException(e);
            throw;
        }
    }

    public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        // transaction set to null here, use TransactionScope semantics for transaction handling
        int result = UsingConnection(connection => connection.Execute(sql, param, null, commandTimeout, commandType));
        return result;
    }

    private void CheckForAlreadyExistsInDatabaseException(Exception e)
    {
        if (e.Message.Contains("duplicate key value violates unique constraint"))
        {
            throw new ConflictException("Database error trying to save something that already exists.", e);
        }
    }

    private void LogTotalGetsFromDatabase(string callingTemplate)
    {
        LogDebugMessage($">>> Total Gets From Database: {++totalGetsFromDatabase}. <<< ({callingTemplate})");
    }

    private void LogCallingGetAll(string callingTemplate)
    {
        LogDebugMessage($">>> Calling GetAll(builder): <<< ({callingTemplate})");
    }

    [Conditional("DEBUG")]
    private void LogDebugMessage(string message)
    {
        // Uncomment to show these messages in the console when debugging
        //_logger.LogDebug(message);
    }

    public IEnumerable<T> GetAllFromQuery<T>(string parameterizedQuery, object param) where T : DataModelBase
    {
        var all = UsingConnection(connection => connection.Query<T>(parameterizedQuery, param));
        return all;
    }

    public IEnumerable<string> GetAllStringsFromQuery(string parameterizedQuery, object param)
    {
        var all = UsingConnection(connection => connection.Query<string>(parameterizedQuery, param));
        return all;
    }

    public IEnumerable<Guid> GetAllIdsFromQuery(string parameterizedQuery, object param = null)
    {
        var all = UsingConnection(connection => connection.Query<Guid>(parameterizedQuery, param));
        return all;
    }

    public void SanityCheckPager(int totalRowsWithoutPagination, Pager pager)
    {
        if (pager is not null)
        {
            // If you're trying to request more records than we even have... no soup for you.
            if (pager.Offset > totalRowsWithoutPagination)
            {
                throw new PagerException("More rows were requested than are returned by the query.");
            }
        }
    }

    public DateTime PerformHealthCheck()
    {
        return UsingConnection(connection => connection.QuerySingle<DateTime>("select NOW();"));
    }

    string GetDefaultTemplate<T>()
    {
        // NOTE: prefixing the select criteria here to avoid any mapping issues
        // with joins bringing in columns of the same name (like 'Id')
        var tableName = SqlMapperExtensions.TableNameMapper.Invoke(typeof(T));
        // NOTE: including templated sections for common query options (joins, where, order by) but
        // this doesn't force queries to use them, it just will include them if specified
        var template = $"SELECT {tableName}.* FROM {tableName} /**innerjoin**/ /**leftjoin**/ /**where**/ /**orderby**/";
        return template;
    }
}
