namespace Laneful.Exceptions;

/// <summary>
/// Exception thrown when the API returns an error response.
/// </summary>
public class ApiException : LanefulException
{
    public int StatusCode { get; }
    public string ErrorMessage { get; }

    public ApiException(string message, int statusCode, string errorMessage) : base(message)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public ApiException(string message, int statusCode, string errorMessage, Exception innerException) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}

