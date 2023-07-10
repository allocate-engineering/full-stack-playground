using Allocate.Common.Database;
using Allocate.Common.Database.DataModels;
using Allocate.Common.Database.Exceptions;
using Allocate.Common.Database.Services;

using Dapper;

namespace Allocate.Common.Database.Repositories;

/// <summary>
/// The interface is an experimental replacement for the previously use pattern of creating a repository for every single Domain entity in the system.
/// So far it has greatly reduced duplication and sped up the time to deliver a CRUD controller.  And custom repositories can still be used if required.
/// </summary>
public interface IGenericRepository
{
    /// <summary>
    /// Performs a read of a single database row by Guid id.
    /// </summary>
    /// <param name="id">The Guid ID of the record to be retrieved.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>The matching record, of type T, from table T.</returns>
    T Get<T>(Guid id) where T : DataModelBase;

    /// <summary>
    /// Upsert method for records of type T, saves an existing entry or writes a new one.
    /// </summary>
    /// <param name="obj">The object of type T to be written to the database (in table T)</param>
    /// <param name="savedBy">The Guid ID of the User making this change.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    void Save<T>(T obj, Guid savedBy) where T : DataModelBase;

    /// <summary>
    /// Gets all records from table T
    /// </summary>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>All of the records from table T</returns>
    ResultSet<T> GetAll<T>() where T : DataModelBase;

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

    /// <summary>
    /// Returns all records from table T where the PK matches the given objectIds Guids.
    /// </summary>
    /// <param name="objectIds">The PKs of the objects to retrieve.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>All of the records from table T that had a PK matching one of the given Guids.</returns>
    IEnumerable<T> GetBatch<T>(IEnumerable<Guid> objectIds) where T : DataModelBase;

    /// <summary>
    /// Returns all records from table T where T's otherIdColumn are equal to the otherId guid.  Often used for lookups
    /// on FK fields.
    /// </summary>
    /// <param name="otherIdColumn">The name of the column within table T to be searched.</param>
    /// <param name="otherId">The Guid value to search for in the given column.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>All of the records from table T where otherIdColumn matched otherId</returns>
    IEnumerable<T> GetByOtherGuid<T>(string otherIdColumn, Guid otherId) where T : DataModelBase;

    /// <summary>
    /// Returns all records from table T where T's otherIdColumn is equal to a value in the otherIds enumerable.  Often used for lookups
    /// on FK fields.
    /// </summary>
    /// <param name="otherIdColumn">The name of the column within table T to be searched.</param>
    /// <param name="otherIds">The Guid values to search for in the given column.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>All of the records from table T where T's otherIdColumn was equal to a value in the otherIds enumerable</returns>
    IEnumerable<T> GetByOtherGuids<T>(string otherIdColumn, IEnumerable<Guid> otherIds) where T : DataModelBase;

    /// <summary>
    /// Find a record in table T by searching a string column with a unique index.  For finding a User by email address, for example.
    /// </summary>
    /// <param name="otherColumnName">The name of the column within table T to be searched.</param>
    /// <param name="otherId"></param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>The single object T where a uniquely indexed column "otherColumnName" equaled the string in 'otherId'.</returns>
    T GetByOtherUniqueColumn<T>(string otherColumnName, string otherId) where T : DataModelBase;

    /// <summary>
    /// Find a record in table T by searching a unique index over two Guid columns.
    /// </summary>
    /// <param name="column1Name">The first column within table T to be searched.</param>
    /// <param name="firstId">The Guid to search for in the first column.</param>
    /// <param name="column2Name">The second column within table T to be searched.</param>
    /// <param name="secondId">The Guid to search for in the second column.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>The single record in table T having matched two guid columns in a unique index.</returns>
    T GetUsingTwoIdColumnUniqueIndex<T>(string column1Name, Guid firstId, string column2Name, Guid secondId) where T : DataModelBase;

    /// <summary>
    /// Use this method when needing to find records in table T where a Guid[] in "otherIdColumn" contains the value "otherId".
    /// An example is finding the Investing Entities where lpProfileIds contains someone's profile Id.
    /// </summary>
    /// <param name="otherIdColumn">The name of the column within table T to be searched.</param>
    /// <param name="otherId"></param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>The records in table T where a Guid[] "otherIdColumn" contains the value "otherId"</returns>
    IEnumerable<T> GetWhereIdArrayColumnContainsValue<T>(string otherIdColumn, Guid otherId) where T : DataModelBase;

    /// <summary>
    /// Use this method when needing to find records in table T where a Guid[] in "otherIdColumn" contains at least one of the values in "otherIds"
    /// For example, find all Investing Entities where the LpProfileIds column contains one or more of the given lpProfileIds.
    /// </summary>
    /// <param name="otherIdColumnName">The name of the column within table T to be searched.</param>
    /// <param name="otherIds">The Guids to search for in the Guid[] contained in the column to be searched (one or more must be present).</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>The records in table T where a Guid[] "otherIdColumn" contains at least one of the values in "otherIds"</returns>
    IEnumerable<T> GetWhereIdArrayColumnContainsAtLeastOneValue<T>(string otherIdColumnName, IEnumerable<Guid> otherIds) where T : DataModelBase;

    /// <summary>
    /// Returns all of the records from table T where columnName exactly matched stringValue.
    /// </summary>
    /// <param name="columnName">The name of the column within table T to be searched.</param>
    /// <param name="stringValue">The string value to search for in the given column.</param>
    /// <typeparam name="T">A DataModel class that extends Allocate.Domain.Common.DataModels.DataModelBase</typeparam>
    /// <returns>All of the records from table T where columnName exactly matched stringValue</returns>
    IEnumerable<T> GetByStringKey<T>(string columnName, string stringValue) where T : DataModelBase;

}

public class GenericRepository : IGenericRepository
{
    private readonly IDatabaseService _databaseService;

    public GenericRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public T Get<T>(Guid id) where T : DataModelBase
    {
        return _databaseService.Get<T>(id);
    }

    public ResultSet<T> GetAll<T>() where T : DataModelBase
    {
        return _databaseService.GetAll<T>();
    }

    public ResultSet<T> GetAll<T>(Action<SqlBuilder> build, Pager pager = null, string template = null) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(build, pager, template: template);
    }

    public IEnumerable<T> GetBatch<T>(IEnumerable<Guid> ids) where T : DataModelBase
    {
        return _databaseService.GetMultiple<T>(ids);
    }

    public void Save<T>(T obj, Guid modifiedBy) where T : DataModelBase
    {
        _databaseService.Save(obj, modifiedBy);
    }

    public IEnumerable<T> GetByOtherGuid<T>(string otherIdColumn, Guid otherId) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("\"" + otherIdColumn + "\" = @OtherId", new { OtherId = otherId }));
    }

    public IEnumerable<T> GetByOtherGuids<T>(string otherIdColumn, IEnumerable<Guid> otherIds) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("\"" + otherIdColumn + "\" = ANY ( @OtherIds )", new { OtherIds = otherIds }));
    }

    public T GetByOtherUniqueColumn<T>(string otherColumnName, string otherId) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("\"" + otherColumnName + "\" = @OtherId", new { OtherId = otherId })).SingleOrDefault();
    }

    public T GetUsingTwoIdColumnUniqueIndex<T>(string column1Name, Guid firstId, string column2Name, Guid secondId) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("\"" + column1Name + "\" = @FirstId AND " + "\"" + column2Name + "\" = @SecondId", new { FirstId = firstId, SecondId = secondId })).SingleOrDefault();
    }

    public IEnumerable<T> GetWhereIdArrayColumnContainsValue<T>(string otherIdColumn, Guid otherId) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("@OtherId = ANY ( \"" + otherIdColumn + "\" )", new { OtherId = otherId }));
    }

    public IEnumerable<T> GetWhereIdArrayColumnContainsAtLeastOneValue<T>(string otherIdColumnName, IEnumerable<Guid> otherIds) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("\"" + otherIdColumnName + "\" && ( @OtherIds )", new { OtherIds = otherIds.ToList() }));
    }

    public IEnumerable<T> GetByStringKey<T>(string columnName, string stringValue) where T : DataModelBase
    {
        return _databaseService.GetAll<T>(builder =>
              builder.Where("\"" + columnName + "\" = @StringValue", new { StringValue = stringValue }));
    }
}