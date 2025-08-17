using System;

namespace StegoCore.Exceptions;

/// <summary>
/// Exception thrown when an error occurs during decoding operations
/// </summary>
public class DecodeException : StegoException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecodeException"/> class
    /// </summary>
    private DecodeException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecodeException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DecodeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecodeException"/> class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="inner">The exception that is the cause of the current exception</param>
    public DecodeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
