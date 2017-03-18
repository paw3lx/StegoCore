using System;
using Xunit;
using StegoCore;

namespace StegoCoreTests
{
    public class ConverterTests
    {
        [Fact]
        public void BitConverterTest()
        {
            var stego = new Stego(FileHelper.GetPathToImage());
            var fileBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            stego.SetSecretData(fileBytes);
            byte[] resultBytes = stego.SecretDataBytes;
            Assert.Equal(fileBytes, resultBytes);
        }
    }
}