namespace StegoCore
{
    using System.IO;
    using Core;
    using StegoCore.Algorithms;
    using StegoCore.Extensions;

    public sealed class Stego : StegoEntry
    {
        /// <summary>
        /// Intitializes new instance of the <see cref="Stego"/> class
        /// </summary>
        /// <param name="imageStream">Stream of Image</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="imageStream"/> is null.</exception>
        public Stego(Stream imageStream)
            : base(imageStream, null)
        {      
        }

        /// <summary>
        /// Initializes new instance of the <see cref="Stego" /> class
        /// </summary>
        /// <param name="imageStream">Stream of Image</param>
        /// <param name="secretData">Stream of SecretData</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="imageStream"/> or <paramref name="secretData"/> is null.</exception>
        public Stego(Stream imageStream, Stream secretData)
            : base(imageStream, secretData)
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

        /// <summary>
        /// Sets the secret data to hide in <see cref="StegoBase.StegoImage" />
        /// </summary>
        /// <param name="file">Path to secret data</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="file"/> is null.</exception>
        /// <exception cref="System.FileNotFoundException">Thrown if the <paramref name="file"/> is invalid.</exception>
        public void SetSecretData(string file)
        {
            base.LoadSecretData(file);
        }

        /// <summary>
        /// Sets the secret data to hide in <see cref="StegoBase.StegoImage" />
        /// </summary>
        /// <param name="byte">Secret data bytes </param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="bytes"/> is null.</exception>
        public void SetSecretData(byte[] bytes)
        {
            base.LoadSecretData(bytes);
        }

        /// <summary>
        /// Embed secret data in Imaga
        /// </summary>
        /// <param name="algorithm">Algorithm used in embending</param>
        /// <exception cref="System.NullReferenceException">Thrown if the <paramref name="algorithm"/> is is not known algorithm type.</exception>
        /// <exception cref="System.InvalidOperationException">TThrown if the <paramref name="algorithm"/> does not inherit from StegoAlgorithm.</exception>
        /// <exception cref="StegoCore.Exceptions.DataToBigException">Thrown if the secred data is to big for embending</exception>
        /// <returns></returns>
        public ImageSharp.Image Embed(AlgorithmEnum algorithm)
        {
            var alg = AlgorithmFactory.Create(algorithm);
            return alg.Embed(this.image, this.secretData);
        }

        public byte[] Decode(AlgorithmEnum algorithm, ImageSharp.Image stegoImage)
        {
            var alg = AlgorithmFactory.Create(algorithm);
            return alg.Decode(stegoImage);
        }

        public byte[] Decode(AlgorithmEnum algorithm)
        {
            if (this.image == null)
                return null;
            var alg = AlgorithmFactory.Create(algorithm);
            return alg.Decode(this.image);
        }

    }
}
