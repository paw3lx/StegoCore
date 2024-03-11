using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Extensions;
using StegoCore.Model;

namespace StegoCore.Core;

public abstract class StegoEntry : IStegoEntry
{
    protected Image<Rgba32> image;

    public Image<Rgba32> StegoImage => image;

    public StegoEntry(Stream imageStream)
    {
        image = Image.Load<Rgba32>(imageStream);
    }

    public StegoEntry(string imagePath)
    {
        image = Image.Load<Rgba32>(imagePath);
    }

    public StegoEntry(byte[] imageBytes)
    {
        image = Image.Load<Rgba32>(imageBytes);
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

    public void Dispose() => Dispose(true);
}
