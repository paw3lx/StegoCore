namespace StegoCoreTests
{
    using Xunit;
    using StegoCore.Algorithms;
    using StegoCore.Core;
    using ImageSharp;
    using System;
    using ImageSharp.Formats;

    public class AlgorithmTests : IDisposable
    {
        private string bmpOutFileName = "out1.bmp";
        
        public void Dispose()
        {
            System.IO.File.Delete(bmpOutFileName);
        }

        [Fact]
        public void AlgorithmFactory_CreateLsb()
        {
            var created = AlgorithmFactory.Create(AlgorithmEnum.Lsb);      

            Assert.Equal(typeof(Lsb), created.GetType());
        }

        [Fact]
        public void Lsb_Encrypt_Decrypt()
        {
            Configuration.Default.AddImageFormat(new BmpFormat());

            var lsb = AlgorithmFactory.Create(AlgorithmEnum.Lsb);
            byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            
            EncryptAndSave(lsb, secretDataBytes, bmpOutFileName);

            var stegoImage = new Image(bmpOutFileName);
            byte[] resultSecret = lsb.Decode(stegoImage);

            Assert.Equal(secretDataBytes, resultSecret);
        }

        [Fact]
        public void LSB_Embed_SecretDataLength()
        {
            var lsb = AlgorithmFactory.Create(AlgorithmEnum.Lsb);
            byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            var secretData = new SecretData(secretDataBytes);
            var imageWithSecret = lsb.Embed(new Image(FileHelper.GetPathToImage()), secretData);
            int readedLength = lsb.ReadSecretLength(imageWithSecret);

            Assert.Equal(secretDataBytes.Length, readedLength);
        }


        private void EncryptAndSave(StegoAlgorithm algorithm, byte[] secretDataBytes, string fileName)
        {
            var secretData = new SecretData(secretDataBytes);
            var result = algorithm.Embed(new Image(FileHelper.GetPathToImage()), secretData);           
            result.Save(fileName);
        }
    }
}