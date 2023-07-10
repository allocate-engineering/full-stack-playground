using System.Data;
using System.Text.Json;

using Npgsql;

using NpgsqlTypes;

using static Dapper.SqlMapper;

namespace Allocate.Common.Database.TypeHandlers;

public class JsonbTypeHandler<T> : ITypeHandler
{
    public object Parse(Type destinationType, object value)
    {
        return JsonSerializer.Deserialize<T>(value.ToString() ?? "");
    }

    public void SetValue(IDbDataParameter parameter, object value)
    {
        parameter.Value = value is null || value is DBNull
            ? DBNull.Value
            : JsonSerializer.Serialize(value);
        ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Jsonb;
    }
}
