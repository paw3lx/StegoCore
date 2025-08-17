using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Exceptions;

namespace StegoCore.Validation;

/// <summary>
/// Provides validation methods for arguments and input parameters
/// </summary>
internal static class ArgumentValidator
{
    /// <summary>
    /// Validates that the specified stream is not null and can be read
    /// </summary>
    /// <param name="stream">The stream to validate</param>
    /// <param name="parameterName">The name of the parameter being validated</param>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null</exception>
    /// <exception cref="ArgumentException">Thrown when the stream cannot be read</exception>
    public static void ValidateImageStream(Stream? stream, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(stream, parameterName);
        
        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable", parameterName);
        }
    }

    /// <summary>
    /// Validates that the specified file path is not null or empty and the file exists
    /// </summary>
    /// <param name="filePath">The file path to validate</param>
    /// <param name="parameterName">The name of the parameter being validated</param>
    /// <exception cref="ArgumentNullException">Thrown when the file path is null</exception>
    /// <exception cref="ArgumentException">Thrown when the file path is empty or whitespace</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist</exception>
    public static void ValidateImageFilePath(string? filePath, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(filePath, parameterName);
        
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty or whitespace", parameterName);
        }
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Image file not found: {filePath}", filePath);
        }
    }

    /// <summary>
    /// Validates that the specified byte array is not null or empty
    /// </summary>
    /// <param name="imageBytes">The byte array to validate</param>
    /// <param name="parameterName">The name of the parameter being validated</param>
    /// <exception cref="ArgumentNullException">Thrown when the byte array is null</exception>
    /// <exception cref="ArgumentException">Thrown when the byte array is empty</exception>
    public static void ValidateImageBytes(byte[]? imageBytes, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(imageBytes, parameterName);
        
        if (imageBytes.Length == 0)
        {
            throw new ArgumentException("Image byte array cannot be empty", parameterName);
        }
    }

    /// <summary>
    /// Validates that the specified image is not null
    /// </summary>
    /// <param name="image">The image to validate</param>
    /// <param name="parameterName">The name of the parameter being validated</param>
    /// <exception cref="ArgumentNullException">Thrown when the image is null</exception>
    public static void ValidateImage<TPixel>(Image<TPixel>? image, string parameterName) where TPixel : unmanaged, IPixel<TPixel>
    {
        ArgumentNullException.ThrowIfNull(image, parameterName);
    }

    /// <summary>
    /// Validates that the specified secret data is not null
    /// </summary>
    /// <param name="secretData">The secret data to validate</param>
    /// <param name="parameterName">The name of the parameter being validated</param>
    /// <exception cref="ArgumentNullException">Thrown when the secret data is null</exception>
    public static void ValidateSecretData(object? secretData, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(secretData, parameterName);
    }

    /// <summary>
    /// Validates that the image has sufficient capacity for the specified data size
    /// </summary>
    /// <param name="image">The image to check capacity for</param>
    /// <param name="requiredDataSize">The required data size in bytes</param>
    /// <param name="algorithmCapacityRatio">The algorithm's capacity ratio (e.g., 0.125 for LSB = 1 bit per pixel)</param>
    /// <exception cref="InsufficientCapacityException">Thrown when the image capacity is insufficient</exception>
    public static void ValidateImageCapacity<TPixel>(Image<TPixel> image, long requiredDataSize, double algorithmCapacityRatio) where TPixel : unmanaged, IPixel<TPixel>
    {
        ArgumentNullException.ThrowIfNull(image, nameof(image));
        
        if (requiredDataSize <= 0)
        {
            return; // No validation needed for zero or negative data size
        }

        // Calculate available capacity: (width * height * algorithmCapacityRatio)
        long totalPixels = (long)image.Width * image.Height;
        long availableCapacity = (long)(totalPixels * algorithmCapacityRatio);
        
        if (requiredDataSize > availableCapacity)
        {
            throw new InsufficientCapacityException(requiredDataSize, availableCapacity);
        }
    }

    /// <summary>
    /// Validates the algorithm enum value
    /// </summary>
    /// <param name="algorithm">The algorithm enum to validate</param>
    /// <param name="parameterName">The name of the parameter being validated</param>
    /// <exception cref="ArgumentException">Thrown when the algorithm value is not defined</exception>
    public static void ValidateAlgorithm(Enum algorithm, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(algorithm, parameterName);
        
        if (!Enum.IsDefined(algorithm.GetType(), algorithm))
        {
            throw new ArgumentException($"Invalid algorithm value: {algorithm}", parameterName);
        }
    }
}