using System.IO;
using SixLabors.ImageSharp;

namespace StegoCore.Core;

public class StegoEntry : StegoBase
{
    public StegoEntry(Stream imageStream, Stream secretData)
    {
        image = Image.Load(imageStream);
        this.LoadSecretData(secretData);
    }

    public StegoEntry(string imagePath)
    {
        image = Image.Load(imagePath);

    }

    public StegoEntry(byte[] imageBytes)
    {
        image = Image.Load(imageBytes);
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
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
        }
        this.secretData = new SecretData(buffer);
    }

    protected void LoadSecretData(Stream stream)
    {
        byte[] buffer = null;
        stream.Read(buffer, 0, (int)stream.Length);
        this.secretData = new SecretData(buffer);
    }

    protected void LoadSecretData(byte[] bytes)
    {
        this.secretData = new SecretData(bytes);
    }


}
