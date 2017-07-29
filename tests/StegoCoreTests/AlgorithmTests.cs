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
            Encrypt_Decrypt(AlgorithmEnum.Lsb, new BmpFormat(), this.bmpOutFileName);
        }

        [Fact]
        public void Lsb_Encrypt_Decrypt_Png()
        {
            Encrypt_Decrypt(AlgorithmEnum.Lsb, new PngFormat(), this.pngOutFileName);
        }

        [Fact]
        public void ZhaoKoch_Encrypt_Decrypt_Jpeg()
        {
            Encrypt_Decrypt(AlgorithmEnum.ZhaoKoch, new JpegFormat(), this.jpgOutFileName);
        }

        private void Encrypt_Decrypt(AlgorithmEnum alg, IImageFormat imageFormat, string outFileName)
        {
            Configuration.Default.AddImageFormat(imageFormat);

            var algorithm = AlgorithmFactory.Create(alg);
            byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            
            var notSavedStego = EncryptAndSave(algorithm, secretDataBytes, outFileName);

            var stegoImage = Image.Load(outFileName);
            byte[] resultSecret = algorithm.Decode(notSavedStego, null);

            Assert.Equal(secretDataBytes, resultSecret);
        }

        [Fact]
        public void LSB_Embed_SecretDataLength()
        {
            var lsb = AlgorithmFactory.Create(AlgorithmEnum.Lsb);
            byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            var secretData = new SecretData(secretDataBytes);
            var imageWithSecret = lsb.Embed(Image.Load(FileHelper.GetPathToImage()), secretData, null);
            int readedLength = lsb.ReadSecretLength(imageWithSecret);

            Assert.Equal(secretDataBytes.Length, readedLength);
        }

        [Fact]
        public void ZhaoKoch_Embed_SecretDataLength()
        {
            var lsb = AlgorithmFactory.Create(AlgorithmEnum.ZhaoKoch);
            byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
            var secretData = new SecretData(secretDataBytes);
            var imageWithSecret = lsb.Embed(Image.Load(FileHelper.GetPathToImage()), secretData, null);
            int readedLength = lsb.ReadSecretLength(imageWithSecret);
            imageWithSecret.Save("out3.jpg");
            Assert.Equal(secretDataBytes.Length, readedLength);
        }

        


        private Image EncryptAndSave(StegoAlgorithm algorithm, byte[] secretDataBytes, string fileName)
        {
            var secretData = new SecretData(secretDataBytes);
            var result = algorithm.Embed(Image.Load(FileHelper.GetPathToImage("sky.jpg")), secretData, null);
            result.Save(fileName);
            return result;
        }
    }
}