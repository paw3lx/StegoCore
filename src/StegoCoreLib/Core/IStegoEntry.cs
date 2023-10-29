using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace StegoCore.Core;

public interface IStegoEntry : IDisposable
{
    byte[] SecretDataBytes { get; }

    Image<Rgba32> StegoImage { get; }
}
