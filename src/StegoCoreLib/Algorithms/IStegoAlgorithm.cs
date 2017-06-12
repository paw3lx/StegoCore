namespace StegoCore.Algorithms
{
    using ImageSharp;
    using StegoCore.Core;

    public interface IStegoAlgorithm
    {
        Image Embed(Image baseImage, SecretData secret);

        byte[] Decode(Image stegoImage);

        int ReadSecretLength(Image stegoImage);

        bool EmbedPossible(Image image, int secretLength);
         
    }

    public abstract class StegoAlgorithm : IStegoAlgorithm
    {
        protected int SecretDataLength = 32;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stegoImage"></param>
        /// <returns></returns>
        public abstract byte[] Decode(Image stegoImage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseImage"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public abstract Image Embed(Image baseImage, SecretData secret);

        /// <summary>
        /// Checks if embed is possible
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="secretLength">length of the secret to embed</param>
        /// <returns>possibility of embeding</returns>
        public abstract bool EmbedPossible(Image image, int secretLength);

        /// <summary>
        /// Reads emended secret length in image
        /// </summary>
        /// <param name="stegoImage">Image with emended message</param>
        /// <returns>secret length</returns>
        public abstract int ReadSecretLength(Image stegoImage);
    }
}