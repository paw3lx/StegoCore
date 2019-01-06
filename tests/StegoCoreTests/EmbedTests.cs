using SixLabors.ImageSharp;
using StegoCore.Algorithms;
using Xunit;
using StegoCore;

namespace StegoCoreTests
{
    public class EmbedTests
    {
        [Fact]
        public void Embed_Decode()
        {
            var image = Image.Load(FileHelper.GetPathToImage());
            var fileBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            byte[] resultBytes = null;
            using(var stego = new Stego(FileHelper.GetPathToImage()))
            {
                stego.SetSecretData(fileBytes);
                var imageWithSecret = stego.Embed(AlgorithmEnum.Lsb);
                stego.SetImage(imageWithSecret);
                resultBytes = stego.Decode(AlgorithmEnum.Lsb);
            }            
            Assert.Equal(fileBytes, resultBytes);
        }

    }
}