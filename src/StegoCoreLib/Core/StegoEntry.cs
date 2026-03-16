using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Validation;
using StegoCore.Exceptions;

namespace StegoCore.Core;

public abstract class StegoEntry : IStegoEntry
{
    protected Image<Rgba32> image;

    public Image<Rgba32> StegoImage => image;

    public StegoEntry(Stream imageStream)
    {
        ArgumentValidator.ValidateImageStream(imageStream, nameof(imageStream));
        
        try
        {
            image = Image.Load<Rgba32>(imageStream);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not ArgumentNullException)
        {
            throw new UnsupportedImageFormatException("Failed to load image from stream. The format may not be supported.", null, ex);
        }
    }

    public StegoEntry(string imagePath)
    {
        ArgumentValidator.ValidateImageFilePath(imagePath, nameof(imagePath));
        
        try
        {
            image = Image.Load<Rgba32>(imagePath);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not ArgumentNullException && ex is not FileNotFoundException)
        {
            string extension = Path.GetExtension(imagePath);
            throw new UnsupportedImageFormatException($"Failed to load image from file '{imagePath}'. The format '{extension}' may not be supported.", extension, ex);
        }
    }

    public StegoEntry(byte[] imageBytes)
    {
        ArgumentValidator.ValidateImageBytes(imageBytes, nameof(imageBytes));
        
        try
        {
            image = Image.Load<Rgba32>(imageBytes);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not ArgumentNullException)
        {
            throw new UnsupportedImageFormatException("Failed to load image from byte array. The format may not be supported.", null, ex);
        }
    }

    // protected void LoadSecretData(string filePath)
    // {
    //     byte[] buffer = null;
    //     using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
    //     {
    //         buffer = new byte[fs.Length];
    //         fs.Read(buffer, 0, (int)fs.Length);
    //     }
    //     secretData = new SecretData(buffer);
    // }

    private bool _disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                image?.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
