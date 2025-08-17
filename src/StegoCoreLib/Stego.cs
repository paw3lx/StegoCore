using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Algorithms;
using StegoCore.Core;
using StegoCore.Validation;
using StegoCore.Exceptions;
using StegoCore.Constants;

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
    /// <param name="imagePath">Image file path</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="imagePath"/> is null.</exception>
    public Stego(string imagePath)
        : base(imagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stego"/> class
    /// </summary>
    /// <param name="imageBytes">Image data as byte array</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="imageBytes"/> is null.</exception>
    /// <exception cref="System.ArgumentException">Thrown if the <paramref name="imageBytes"/> is empty.</exception>
    public Stego(byte[] imageBytes)
        : base(imageBytes)
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
    /// <param name="secretData">The secret data to embed</param>
    /// <param name="algorithm">Algorithm used in embedding</param>
    /// <param name="settings">Optional algorithm settings</param>
    /// <exception cref="System.ArgumentNullException">Thrown when secretData is null</exception>
    /// <exception cref="System.ArgumentException">Thrown when algorithm is invalid</exception>
    /// <exception cref="StegoCore.Exceptions.InvalidAlgorithmException">Thrown when the algorithm is not supported</exception>
    /// <exception cref="StegoCore.Exceptions.InsufficientCapacityException">Thrown when the secret data is too large for the image</exception>
    /// <returns>Image with embedded secret data</returns>
    public Image<Rgba32> Embed(SecretData secretData, AlgorithmEnum algorithm, ISettings? settings = null)
    {
        ArgumentValidator.ValidateSecretData(secretData, nameof(secretData));
        ArgumentValidator.ValidateAlgorithm(algorithm, nameof(algorithm));
        ArgumentValidator.ValidateImage(image, nameof(image));

        try
        {
            StegoAlgorithm alg = AlgorithmFactory.Create(algorithm);
            return alg.Embed(this.image, secretData, settings);
        }
        catch (Exception ex) when (ex is not StegoException && ex is not ArgumentException && ex is not ArgumentNullException)
        {
            throw new InvalidAlgorithmException($"Failed to create or execute algorithm '{algorithm}'", algorithm.ToString(), ex);
        }
    }

    /// <summary>
    /// Decodes secret data from Image
    /// </summary>
    /// <param name="algorithm">Algorithm used in decoding</param>
    /// <param name="settings">Optional algorithm settings</param>
    /// <exception cref="System.ArgumentException">Thrown when algorithm is invalid</exception>
    /// <exception cref="StegoCore.Exceptions.InvalidAlgorithmException">Thrown when the algorithm is not supported</exception>
    /// <exception cref="StegoCore.Exceptions.DecodeException">Thrown if error while decoding occurs</exception>
    /// <returns>Bytes of decoded secret data</returns>
    public byte[] Decode(AlgorithmEnum algorithm, ISettings? settings = null)
    {
        ArgumentValidator.ValidateAlgorithm(algorithm, nameof(algorithm));
        ArgumentValidator.ValidateImage(image, nameof(image));

        try
        {
            StegoAlgorithm alg = AlgorithmFactory.Create(algorithm);
            return alg.Decode(image, settings);
        }
        catch (Exception ex) when (ex is not StegoException && ex is not ArgumentException && ex is not ArgumentNullException)
        {
            throw new InvalidAlgorithmException($"Failed to create or execute algorithm '{algorithm}'", algorithm.ToString(), ex);
        }
    }

    /// <summary>
    /// Sets the image used for embedding or decoding
    /// </summary>
    /// <param name="image">Image to set</param>
    /// <exception cref="System.ArgumentNullException">Thrown when image is null</exception>
    public void SetImage(Image<Rgba32> image)
    {
        ArgumentValidator.ValidateImage(image, nameof(image));
        this.image = image;
    }

    /// <summary>
    /// Checks if the image can embed the specified amount of data using the given algorithm
    /// </summary>
    /// <param name="dataSize">The size of data in bytes to check</param>
    /// <param name="algorithm">The algorithm to use for capacity calculation</param>
    /// <returns>True if the image has sufficient capacity, false otherwise</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when image is null</exception>
    /// <exception cref="System.ArgumentException">Thrown when algorithm is invalid</exception>
    public bool CanEmbed(long dataSize, AlgorithmEnum algorithm)
    {
        ArgumentValidator.ValidateImage(image, nameof(image));
        ArgumentValidator.ValidateAlgorithm(algorithm, nameof(algorithm));

        if (dataSize <= 0)
        {
            return true; // Zero or negative data size is always embeddable
        }

        // Add header size to the required data size
        long totalRequiredSize = dataSize + StegoConstants.SecretDataLengthBytes;

        double capacityRatio = algorithm switch
        {
            AlgorithmEnum.Lsb => StegoConstants.LsbCapacityRatio,
            AlgorithmEnum.ZhaoKoch => StegoConstants.ZhaoKochCapacityRatio,
            _ => throw new InvalidAlgorithmException($"Unknown algorithm: {algorithm}", algorithm.ToString())
        };

        long totalPixels = (long)image.Width * image.Height;
        long availableCapacity = (long)(totalPixels * capacityRatio);

        return totalRequiredSize <= availableCapacity;
    }

    /// <summary>
    /// Gets the maximum data capacity for the current image using the specified algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm to use for capacity calculation</param>
    /// <returns>The maximum data capacity in bytes</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when image is null</exception>
    /// <exception cref="System.ArgumentException">Thrown when algorithm is invalid</exception>
    public long GetCapacity(AlgorithmEnum algorithm)
    {
        ArgumentValidator.ValidateImage(image, nameof(image));
        ArgumentValidator.ValidateAlgorithm(algorithm, nameof(algorithm));

        double capacityRatio = algorithm switch
        {
            AlgorithmEnum.Lsb => StegoConstants.LsbCapacityRatio,
            AlgorithmEnum.ZhaoKoch => StegoConstants.ZhaoKochCapacityRatio,
            _ => throw new InvalidAlgorithmException($"Unknown algorithm: {algorithm}", algorithm.ToString())
        };

        long totalPixels = (long)image.Width * image.Height;
        long totalCapacity = (long)(totalPixels * capacityRatio);
        
        // Subtract header size to get usable capacity for secret data
        return Math.Max(0, totalCapacity - StegoConstants.SecretDataLengthBytes);
    }
}
