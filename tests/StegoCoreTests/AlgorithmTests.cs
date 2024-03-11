using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore;
using StegoCore.Algorithms;
using StegoCore.Core;
using StegoCore.Exceptions;
using Xunit;

namespace StegoCoreTests;

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
    public void AlgorithmFactory_CreateLsb_Type_Equals()
    {
        var created = AlgorithmFactory.Create(AlgorithmEnum.Lsb);

        Assert.Equal(typeof(Lsb), created.GetType());
    }

    [Fact]
    public void Lsb_Encrypt_Decrypt_Bmp()
    {
        Encrypt_Decrypt(AlgorithmEnum.Lsb, SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance, this.bmpOutFileName);
    }

    [Fact]
    public void Lsb_Encrypt_Decrypt_Png()
    {
        Encrypt_Decrypt(AlgorithmEnum.Lsb, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance, this.pngOutFileName);
    }

    [Fact]
    public void ZhaoKoch_Encrypt_Decrypt_Jpeg()
    {
        Encrypt_Decrypt(AlgorithmEnum.ZhaoKoch, SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance, this.jpgOutFileName);
    }

    private void Encrypt_Decrypt(AlgorithmEnum alg, IImageFormat imageFormat, string outFileName)
    {
        Configuration.Default.ImageFormatsManager.AddImageFormat(imageFormat);

        var algorithm = AlgorithmFactory.Create(alg);
        byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());

        var notSavedStego = EncryptAndSave(algorithm, secretDataBytes, outFileName);

        var stegoImage = Image.Load(outFileName);
        byte[] resultSecret = algorithm.Decode(notSavedStego, null);

        Assert.Equal(secretDataBytes, resultSecret);
    }

    [Fact]
    public void LSB_Embed_And_Decode_SecretDataLength()
    {
        var lsb = AlgorithmFactory.Create(AlgorithmEnum.Lsb);
        byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
        var secretData = new SecretData(secretDataBytes);
        var imageWithSecret = lsb.Embed(Image.Load<Rgba32>(FileHelper.GetPathToImage()), secretData, null);
        int readedLength = lsb.ReadSecretLength(imageWithSecret, null);

        Assert.Equal(secretDataBytes.Length, readedLength);
    }

    [Fact]
    public void ZhaoKoch_Embed_And_Decode_SecretDataLength()
    {
        var lsb = AlgorithmFactory.Create(AlgorithmEnum.ZhaoKoch);
        byte[] secretDataBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
        var secretData = new SecretData(secretDataBytes);
        var imageWithSecret = lsb.Embed(Image.Load<Rgba32>(FileHelper.GetPathToImage()), secretData, null);
        int readedLength = lsb.ReadSecretLength(imageWithSecret, null);
        imageWithSecret.Save("out3.jpg");
        Assert.Equal(secretDataBytes.Length, readedLength);
    }


    [Fact]
    public void Lsb_Encrypt_With_Key_Decrypt_Without_Key_Should_Throws_Error()
    {
        var fileBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
        using (var stego = new Stego(FileHelper.GetPathToImage()))
        {
            var imageWithSecret = stego.Embed(new SecretData(fileBytes), AlgorithmEnum.Lsb, new StegoCore.Model.Settings
            {
                Key = "aaa"
            });
            stego.SetImage(imageWithSecret);

            DecodeException ex = Assert.Throws<DecodeException>(() => { stego.Decode(AlgorithmEnum.Lsb, null); });
        }
    }

    [Fact]
    public void Lsb_Encrypt_With_Key_Decrypt_With_Key()
    {
        var fileBytes = System.IO.File.ReadAllBytes(FileHelper.GetPathToSecretData());
        using (var stego = new Stego(FileHelper.GetPathToImage()))
        {
            var settings = new StegoCore.Model.Settings
            {
                Key = "aaa"
            };
            var imageWithSecret = stego.Embed(new SecretData(fileBytes), AlgorithmEnum.Lsb,settings );
            stego.SetImage(imageWithSecret);
            byte[] resultSecret = stego.Decode(AlgorithmEnum.Lsb, settings);

            Assert.Equal(fileBytes, resultSecret);
        }
    }

    private Image<Rgba32> EncryptAndSave(StegoAlgorithm algorithm, byte[] secretDataBytes, string fileName)
    {
        var secretData = new SecretData(secretDataBytes);
        var result = algorithm.Embed(Image.Load<Rgba32>(FileHelper.GetPathToImage("sky.jpg")), secretData, null);
        result.Save(fileName);
        return result;
    }
}
