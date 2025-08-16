using SixLabors.ImageSharp;
using StegoCore;
using StegoCore.Algorithms;
using StegoCore.Core;
using Xunit;

namespace StegoCoreTests;

public class EmbedTests
{
    [Fact]
    public void Embed_Decode()
    {
        var image = Image.Load(FileHelper.GetPathToImage());
        var fileBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
        byte[]? resultBytes = null;
        using (var stego = new Stego(FileHelper.GetPathToImage()))
        {
            var settings = new StegoCore.Model.Settings
            {
                Key = "aaa"
            };
            var imageWithSecret = stego.Embed(new SecretData(fileBytes), AlgorithmEnum.Lsb, settings);
            stego.SetImage(imageWithSecret);
            resultBytes = stego.Decode(AlgorithmEnum.Lsb, settings);
        }
        Assert.Equal(fileBytes, resultBytes);
    }

}
