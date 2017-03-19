namespace StegoCoreTests
{
    using System.Collections;
    using ImageSharp;
    using StegoCore.Algorithms;
    using Xunit;
    using StegoCore;

    public class EmbedTests
    {
        [Fact]
        public void Embed_Decode()
        {
            var image = new Image(FileHelper.GetPathToImage());
            var fileBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            byte[] resultBytes = null;
            using(var stego = new Stego(FileHelper.GetPathToImage()))
            {
                stego.SetSecretData(fileBytes);
                var imageWithSecret = stego.Embed(AlgorithmEnum.Lsb);
                resultBytes = stego.Decode(AlgorithmEnum.Lsb, imageWithSecret);
            }            
            Assert.Equal(fileBytes, resultBytes);
        }

    }
}