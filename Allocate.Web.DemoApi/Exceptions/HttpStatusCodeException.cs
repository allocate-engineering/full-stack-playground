using System.Net;

namespace Allocate.Common.Exceptions;

/// <summary>
/// Exception that includes an HTTP Status Code and a message to use to modify the current HTTP Response.
/// Based on the exception of the same name from the old-school System.Web.Http, except that one extends 
/// Exception without defining the constructors that take a message and an exception, and constructors are not 
/// inherited in C#.  So we rolled our own.
/// Ref: https://learn.microsoft.com/en-us/dotnet/api/system.web.http.httpresponseexception?view=aspnet-webapi-5.2
/// Also see: https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1#use-exceptions-to-modify-the-response
/// </summary>
public class HttpStatusCodeException : Exception
{
    /// <summary>
    /// HTTP Status Code
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    public HttpStatusCodeException() : base()
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }

    public HttpStatusCodeException(HttpStatusCode statusCode) : base()
    {
        StatusCode = statusCode;
    }

    public HttpStatusCodeException(HttpStatusCode statusCode, string message = null, Exception innerException = null) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
