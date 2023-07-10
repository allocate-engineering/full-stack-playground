using System.Net;

using Allocate.Common.Exceptions;

namespace Allocate.Common.Database.Exceptions;

public class ConflictException : HttpStatusCodeException
{
    public ConflictException() : base(HttpStatusCode.Conflict)
    {

    }

    public ConflictException(string message, Exception innerException = null)
        : base(HttpStatusCode.Conflict, message, innerException)
    {
    }
}
