using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Extensions;
using StegoCore.Model;

namespace StegoCore.Core;

public abstract class StegoBase : IStegoEntry
{
    private bool _disposedValue = false;
    protected Image<Rgba32> image;
    protected SecretData secretData;
    protected Settings settings;

    public Image<Rgba32> StegoImage => image;
    public byte[] SecretDataBytes => secretData?.SecretBits?.ToByteArray();

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
        this.Dispose(true);
    }

    ~StegoBase()
    {
        Dispose(false);
    }

    void System.IDisposable.Dispose()
    {
        Dispose(true);
    }
}
