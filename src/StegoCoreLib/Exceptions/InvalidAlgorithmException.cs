using System;

namespace StegoCore.Exceptions;

/// <summary>
/// Exception thrown when an invalid or unsupported algorithm is specified
/// </summary>
public class InvalidAlgorithmException : StegoException
{
    /// <summary>
    /// Gets the invalid algorithm name or identifier
    /// </summary>
    public string? Algorithm { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAlgorithmException"/> class
    /// </summary>
    /// <param name="algorithm">The invalid algorithm name or identifier</param>
    public InvalidAlgorithmException(string? algorithm)
        : base($"Algorithm '{algorithm}' is not supported or invalid")
    {
        Algorithm = algorithm;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAlgorithmException"/> class with a custom message
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="algorithm">The invalid algorithm name or identifier</param>
    public InvalidAlgorithmException(string message, string? algorithm)
        : base(message)
    {
        Algorithm = algorithm;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAlgorithmException"/> class with a custom message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="algorithm">The invalid algorithm name or identifier</param>
    /// <param name="innerException">The inner exception</param>
    public InvalidAlgorithmException(string message, string? algorithm, Exception innerException)
        : base(message, innerException)
    {
        Algorithm = algorithm;
    }
}