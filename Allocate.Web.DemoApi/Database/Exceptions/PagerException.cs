using System.Runtime.Serialization;

namespace Allocate.Common.Database.Exceptions;

[Serializable]
public class PagerException : Exception
{
    public PagerException()
    {
    }

    public PagerException(string message) : base(message)
    {
    }

    public PagerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected PagerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}