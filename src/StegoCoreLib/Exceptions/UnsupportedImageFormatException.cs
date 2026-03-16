using System;

namespace StegoCore.Exceptions;

/// <summary>
/// Exception thrown when an unsupported image format is encountered
/// </summary>
public class UnsupportedImageFormatException : StegoException
{
    /// <summary>
    /// Gets the unsupported format
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class
    /// </summary>
    /// <param name="format">The unsupported format</param>
    public UnsupportedImageFormatException(string? format)
        : base($"Image format '{format}' is not supported for steganography operations")
    {
        Format = format;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class with a custom message
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="format">The unsupported format</param>
    public UnsupportedImageFormatException(string message, string? format)
        : base(message)
    {
        Format = format;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class with a custom message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="format">The unsupported format</param>
    /// <param name="innerException">The inner exception</param>
    public UnsupportedImageFormatException(string message, string? format, Exception innerException)
        : base(message, innerException)
    {
        Format = format;
    }
}