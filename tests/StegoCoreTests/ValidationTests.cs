using System;
using System.IO;
using Xunit;
using StegoCore;
using StegoCore.Algorithms;
using StegoCore.Exceptions;
using StegoCore.Core;

namespace StegoCoreTests;

public class ValidationTests
{
    [Fact]
    public void Constructor_WithNullStream_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Stego((Stream)null!));
    }

    [Fact]
    public void Constructor_WithNullFilePath_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Stego((string)null!));
    }

    [Fact]
    public void Constructor_WithEmptyFilePath_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Stego(""));
    }

    [Fact]
    public void Constructor_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange & Act & Assert
        Assert.Throws<FileNotFoundException>(() => new Stego("nonexistent-file.jpg"));
    }

    [Fact]
    public void Constructor_WithNullByteArray_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Stego((byte[])null!));
    }

    [Fact]
    public void Constructor_WithEmptyByteArray_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Stego(Array.Empty<byte>()));
    }

    [Fact]
    public void Embed_WithNullSecretData_ThrowsArgumentNullException()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => stego.Embed(null!, AlgorithmEnum.Lsb));
    }

    [Fact]
    public void Embed_WithInvalidAlgorithm_ThrowsArgumentException()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());
        var secretData = new SecretData(System.Text.Encoding.UTF8.GetBytes("test"));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => stego.Embed(secretData, (AlgorithmEnum)999));
    }

    [Fact]
    public void Decode_WithInvalidAlgorithm_ThrowsArgumentException()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => stego.Decode((AlgorithmEnum)999));
    }

    [Fact]
    public void SetImage_WithNullImage_ThrowsArgumentNullException()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => stego.SetImage(null!));
    }

    [Fact]
    public void CanEmbed_WithValidData_ReturnsTrue()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());
        
        // Act
        bool canEmbed = stego.CanEmbed(10, AlgorithmEnum.Lsb);
        
        // Assert
        Assert.True(canEmbed);
    }

    [Fact]
    public void CanEmbed_WithZeroData_ReturnsTrue()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());
        
        // Act
        bool canEmbed = stego.CanEmbed(0, AlgorithmEnum.Lsb);
        
        // Assert
        Assert.True(canEmbed);
    }

    [Fact]
    public void GetCapacity_WithLsbAlgorithm_ReturnsPositiveValue()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());
        
        // Act
        long capacity = stego.GetCapacity(AlgorithmEnum.Lsb);
        
        // Assert
        Assert.True(capacity > 0);
    }

    [Fact]
    public void GetCapacity_WithZhaoKochAlgorithm_ReturnsPositiveValue()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());
        
        // Act
        long capacity = stego.GetCapacity(AlgorithmEnum.ZhaoKoch);
        
        // Assert
        Assert.True(capacity > 0);
    }

    [Fact]
    public void CanEmbed_WithInvalidAlgorithm_ThrowsArgumentException()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => stego.CanEmbed(10, (AlgorithmEnum)999));
    }

    [Fact]
    public void GetCapacity_WithInvalidAlgorithm_ThrowsArgumentException()
    {
        // Arrange
        using var stego = new Stego(FileHelper.GetPathToImage());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => stego.GetCapacity((AlgorithmEnum)999));
    }
}