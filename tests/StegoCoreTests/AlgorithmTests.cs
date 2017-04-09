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
        private string jpgOutFileName = "out.jpg";
        private string pngOutFileName = "out.png";
        
        public void Dispose()
        {
            //System.IO.File.Delete(bmpOutFileName);
            //System.IO.File.Delete(jpgOutFileName);
        }

        [Fact]
        public void AlgorithmFactory_CreateLsb()
        {
            var created = AlgorithmFactory.Create(AlgorithmEnum.Lsb);      

            Assert.Equal(typeof(Lsb), created.GetType());
        }

        [Fact]
        public void Lsb_Encrypt_Decrypt_Bmp()
        {
            Lsb_Encrypt_Decrypt(new BmpFormat(), this.bmpOutFileName);
        }

        [Fact]
        public void Lsb_Encrypt_Decrypt_Png()
        {
            Lsb_Encrypt_Decrypt(new PngFormat(), this.pngOutFileName);
        }

        private void Lsb_Encrypt_Decrypt(IImageFormat imageFormat, string outFileName)
        {
            Configuration.Default.AddImageFormat(imageFormat);

            var lsb = AlgorithmFactory.Create(AlgorithmEnum.Lsb);
            byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            
            EncryptAndSave(lsb, secretDataBytes, outFileName);

            var stegoImage = new Image(outFileName);
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