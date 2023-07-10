using System.Data;
using System.Text.Json;

using Dapper;

namespace Allocate.Common.Database.TypeHandlers;

public class JsonbJsonDocumentTypeHandler : SqlMapper.TypeHandler<JsonDocument>
{
    public override JsonDocument Parse(object value)
    {
        var result = JsonDocument.Parse(value.ToString() ?? "");
        return result;
    }

    public override void SetValue(IDbDataParameter parameter, JsonDocument value)
    {
        parameter.Value = value;
    }
}
