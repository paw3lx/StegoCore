using System;

namespace StegoCore.Exceptions;

/// <summary>
/// Exception thrown when secret data is too large for the image capacity
/// This class is deprecated - use <see cref="InsufficientCapacityException"/> instead
/// </summary>
[Obsolete("Use InsufficientCapacityException instead", false)]
public class DataToBigException : StegoException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataToBigException"/> class
    /// </summary>
    private DataToBigException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataToBigException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DataToBigException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataToBigException"/> class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="inner">The exception that is the cause of the current exception</param>
    public DataToBigException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
