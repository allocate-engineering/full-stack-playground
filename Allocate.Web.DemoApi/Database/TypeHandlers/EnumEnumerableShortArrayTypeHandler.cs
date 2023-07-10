using System.Data;

using Dapper;

namespace Allocate.Common.Database.TypeHandlers;

public class EnumEnumerableShortArrayTypeHandler<T> : SqlMapper.TypeHandler<IEnumerable<T>> where T : Enum
{
    public override void SetValue(IDbDataParameter parameter, IEnumerable<T> value)
    {
        //we know T is an enum so cast it
        parameter.Value = value.Select(someEnum => Convert.ToInt16(someEnum)).ToArray();
    }

    public override IEnumerable<T> Parse(object value)
    {
        var array = (short[])value;
        var result = new List<T>();
        foreach (short intEnum in array)
        {
            result.Add((T)(object)intEnum);
        }
        return result;
    }
}