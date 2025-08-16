using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Core;

namespace StegoCore.Algorithms;

public interface IStegoAlgorithm
{
    Image<Rgba32> Embed(Image<Rgba32> baseImage, SecretData secret, ISettings? settings = null);

    byte[] Decode(Image<Rgba32> stegoImage, ISettings? settings = null);

    int ReadSecretLength(Image<Rgba32> stegoImage, ISettings? settings = null);

    bool IsEmbedPossible(Image<Rgba32> image, int secretLength);
}
