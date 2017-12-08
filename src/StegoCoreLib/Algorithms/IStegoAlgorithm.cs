using SixLabors.ImageSharp;
using StegoCore.Core;
using StegoCore.Model;

namespace StegoCore.Algorithms
{
    public interface IStegoAlgorithm
    {
        Image<Rgba32> Embed(Image<Rgba32> baseImage, SecretData secret, Settings settings = null);
        byte[] Decode(Image<Rgba32> stegoImage, Settings settings = null);
        int ReadSecretLength(Image<Rgba32> stegoImage, Settings settings = null);
        bool EmbedPossible(Image<Rgba32> image, int secretLength);
    }
}