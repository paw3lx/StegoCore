using System;

namespace StegoCore.Exceptions;

/// <summary>
/// Exception thrown when the image does not have sufficient capacity to embed the secret data
/// </summary>
public class InsufficientCapacityException : StegoException
{
    /// <summary>
    /// Gets the required capacity in bytes
    /// </summary>
    public long RequiredCapacity { get; }

    /// <summary>
    /// Gets the available capacity in bytes
    /// </summary>
    public long AvailableCapacity { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InsufficientCapacityException"/> class
    /// </summary>
    /// <param name="requiredCapacity">The required capacity in bytes</param>
    /// <param name="availableCapacity">The available capacity in bytes</param>
    public InsufficientCapacityException(long requiredCapacity, long availableCapacity)
        : base($"Image capacity {availableCapacity} bytes is insufficient for data requiring {requiredCapacity} bytes")
    {
        RequiredCapacity = requiredCapacity;
        AvailableCapacity = availableCapacity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InsufficientCapacityException"/> class with a custom message
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="requiredCapacity">The required capacity in bytes</param>
    /// <param name="availableCapacity">The available capacity in bytes</param>
    public InsufficientCapacityException(string message, long requiredCapacity, long availableCapacity)
        : base(message)
    {
        RequiredCapacity = requiredCapacity;
        AvailableCapacity = availableCapacity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InsufficientCapacityException"/> class with a custom message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="requiredCapacity">The required capacity in bytes</param>
    /// <param name="availableCapacity">The available capacity in bytes</param>
    /// <param name="innerException">The inner exception</param>
    public InsufficientCapacityException(string message, long requiredCapacity, long availableCapacity, Exception innerException)
        : base(message, innerException)
    {
        RequiredCapacity = requiredCapacity;
        AvailableCapacity = availableCapacity;
    }
}