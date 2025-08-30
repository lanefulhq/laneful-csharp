namespace Laneful.Exceptions;

/// <summary>
/// Exception thrown when HTTP communication fails.
/// </summary>
public class HttpException : LanefulException
{
    public int StatusCode { get; }

    public HttpException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpException(string message, int statusCode, Exception innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}

