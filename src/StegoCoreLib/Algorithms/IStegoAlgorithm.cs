namespace StegoCore.Algorithms
{
    using ImageSharp;
    using StegoCore.Core;

    public interface IStegoAlgorithm
    {
        Image Embed(Image baseImage, SecretData secret);

        byte[] Decode(Image stegoImage);

        int ReadSecretLength(Image stegoImage);
         
    }

    public abstract class StegoAlgorithm : IStegoAlgorithm
    {
        protected int SecretDataLength = 32;

        public abstract byte[] Decode(Image stegoImage);
        public abstract Image Embed(Image baseImage, SecretData secret);
        public abstract int ReadSecretLength(Image stegoImage);
    }
}