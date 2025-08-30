namespace Laneful.Exceptions;

/// <summary>
/// Base exception class for all Laneful SDK exceptions.
/// </summary>
public abstract class LanefulException : Exception
{
    protected LanefulException(string message) : base(message)
    {
    }

    protected LanefulException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

