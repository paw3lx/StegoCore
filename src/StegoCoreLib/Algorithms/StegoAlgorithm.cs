using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Core;
using StegoCore.Model;

namespace StegoCore.Algorithms;

public abstract class StegoAlgorithm : IStegoAlgorithm
{
    protected int SecretDataLength = 32;

    /// <summary>
    ///
    /// </summary>
    /// <param name="stegoImage"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public abstract byte[] Decode(Image<Rgba32> stegoImage, Settings settings = null);

    /// <summary>
    ///
    /// </summary>
    /// <param name="baseImage"></param>
    /// <param name="secret"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public abstract Image<Rgba32> Embed(Image<Rgba32> baseImage, SecretData secret, Settings settings = null);

    /// <summary>
    /// Checks if embed is possible
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="secretLength">length of the secret to embed</param>
    /// <returns>possibility of embeding</returns>
    public abstract bool IsEmbedPossible(Image<Rgba32> image, int secretLength);

    /// <summary>
    /// Reads emended secret length in image
    /// </summary>
    /// <param name="stegoImage">Image with emended message</param>
    /// <param name="settings"></param>
    /// <returns>secret length</returns>
    public abstract int ReadSecretLength(Image<Rgba32> stegoImage, Settings settings = null);

    internal static Random GetRandomGenenator(Settings settings)
    {
        return GetRandomGenenator(settings?.Key);
    }

    internal static Random GetRandomGenenator(string seed)
    {
        return new Random((seed ?? string.Empty).GetHashCode());
    }
}
