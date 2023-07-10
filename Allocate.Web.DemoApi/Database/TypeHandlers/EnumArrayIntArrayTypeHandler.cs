using System.Data;

using Dapper;

namespace Allocate.Common.Database.TypeHandlers;

public class EnumArrayIntArrayTypeHandler<T> : SqlMapper.TypeHandler<T[]> where T : Enum
{
    public override void SetValue(IDbDataParameter parameter, T[] value)
    {
        //we know T is an enum so cast it
        parameter.Value = value.Select(someEnum => Convert.ToInt32(someEnum)).ToArray();
    }

    public override T[] Parse(object value)
    {
        var array = (int[])value;
        var result = new List<T>();
        foreach (int intEnum in array)
        {
            result.Add((T)(object)intEnum);
        }
        return result.ToArray();
    }
}