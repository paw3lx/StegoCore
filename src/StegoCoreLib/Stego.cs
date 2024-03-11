using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Algorithms;
using StegoCore.Core;
using StegoCore.Model;

namespace StegoCore;

public class Stego : StegoEntry
{
    /// <summary>
    /// Intitializes new instance of the <see cref="Stego"/> class
    /// </summary>
    /// <param name="imageStream">Stream of Image</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="imageStream"/> is null.</exception>
    public Stego(Stream imageStream)
        : base(imageStream)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stego"/> class
    /// </summary>
    /// <param name="filePath">Image file path</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="imagePath"/> is null.</exception>
    public Stego(string imagePath)
        : base(imagePath)
    {
    }

    // /// <summary>
    // /// Sets the secret data to hide in <see cref="StegoBase.StegoImage" />
    // /// </summary>
    // /// <param name="file">Path to secret data</param>
    // /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="file"/> is null.</exception>
    // /// <exception cref="System.FileNotFoundException">Thrown if the <paramref name="file"/> is invalid.</exception>
    // public void SetSecretData(string file)
    // {
    //     base.LoadSecretData(file);
    // }

    // /// <summary>
    // /// Sets the secret data to hide in <see cref="StegoBase.StegoImage" />
    // /// </summary>
    // /// <param name="byte">Secret data bytes </param>
    // /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="bytes"/> is null.</exception>
    // public void SetSecretData(byte[] bytes)
    // {
    //     base.LoadSecretData(bytes);
    // }

    /// <summary>
    /// Embed secret data in Image
    /// </summary>
    /// <param name="algorithm">Algorithm used in embending</param>
    /// <exception cref="System.NullReferenceException">Thrown if the <paramref name="algorithm"/> is is not known algorithm type.</exception>
    /// <exception cref="System.InvalidOperationException">TThrown if the <paramref name="algorithm"/> does not inherit from StegoAlgorithm.</exception>
    /// <exception cref="StegoCore.Exceptions.DataToBigException">Thrown if the secred data is to big for embending</exception>
    /// <returns>Image with embeded secret data</returns>
    public Image<Rgba32> Embed(SecretData secretData, AlgorithmEnum algorithm, ISettings settings = null)
    {
        if (image == null)
            throw new System.NullReferenceException("Image cannot be null");
        if (secretData == null)
            throw new System.NullReferenceException("Secret data cannot be null");
        StegoAlgorithm alg = AlgorithmFactory.Create(algorithm);
        return alg.Embed(this.image, secretData, settings);
    }

    /// <summary>
    /// Decodes secred data from Image
    /// </summary>
    /// <param name="algorithm">Algorithm used in decoding</param>
    /// <exception cref="System.NullReferenceException">Thrown if the <paramref name="algorithm"/> is is not known algorithm type.</exception>
    /// <exception cref="System.InvalidOperationException">TThrown if the <paramref name="algorithm"/> does not inherit from StegoAlgorithm.</exception>
    /// <exception cref="StegoCore.Exceptions.DecodeException">Thrown if error while decoding occurs</exception>
    /// <returns>Bytes of decoded secred data</returns>
    public byte[] Decode(AlgorithmEnum algorithm, ISettings settings = null)
    {
        if (image == null)
            throw new System.NullReferenceException("Image cannot be null");
        StegoAlgorithm alg = AlgorithmFactory.Create(algorithm);
        return alg.Decode(image, settings);
    }

    /// <summary>
    /// Sets the image used for emeding or decoding
    /// </summary>
    /// <param name="image">Image to set</param>
    public void SetImage(Image<Rgba32> image)
    {
        this.image = image;
    }
}
