using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace StegoCore.Core;

public class StegoEntry : StegoBase
{
    public StegoEntry(Stream imageStream, Stream secretData)
    {
        image = Image.Load<Rgba32>(imageStream);
        LoadSecretData(secretData);
    }

    public StegoEntry(string imagePath)
    {
        image = Image.Load<Rgba32>(imagePath);

    }

    public StegoEntry(byte[] imageBytes)
    {
        image = Image.Load<Rgba32>(imageBytes);
    }

    public void SaveImage(string path)
    {
        image.Save(path);
    }

    public void SaveImage(MemoryStream stream)
    {
        // TODO : choose encoder based on stream
        image.Save(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
    }

    protected void LoadSecretData(string filePath)
    {
        byte[] buffer = null;
        using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
        }
        secretData = new SecretData(buffer);
    }

    protected void LoadSecretData(Stream stream)
    {
        byte[] buffer = null;
        stream.Read(buffer, 0, (int)stream.Length);
        secretData = new SecretData(buffer);
    }

    protected void LoadSecretData(byte[] bytes)
    {
        secretData = new SecretData(bytes);
    }
}
