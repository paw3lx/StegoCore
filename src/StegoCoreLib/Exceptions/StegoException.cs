using System;

namespace StegoCore.Exceptions;

/// <summary>
/// Base exception class for all StegoCore-related exceptions
/// </summary>
public abstract class StegoException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StegoException"/> class
    /// </summary>
    protected StegoException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StegoException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    protected StegoException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StegoException"/> class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    protected StegoException(string message, Exception innerException) : base(message, innerException)
    {
    }
}