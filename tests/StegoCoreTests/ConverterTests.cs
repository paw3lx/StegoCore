using System;
using Xunit;
using StegoCore;
using StegoCore.Algorithms;

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

        [Fact]
        public void QuantizationTest()
        {
            var matrix = StegoCore.Statics.JPEG.JpegLuminQuantTable;
            float[][] array = new float[8][];
            for(int i = 0; i < matrix.Length; i++)
            {
                array[i] = new float[8];
                for(int j = 0; j < matrix.Length; j++)
                {
                    array[i][j] = matrix[i][j];
                }
            }             
            var zk = new ZhaoKoch();
            var quantized = zk.Quantize(array);
            var dequantized = zk.Dequantize(quantized);
            Assert.Equal(array, dequantized);
        }

        private float[][] inputMatrix = {
                new float[]{ 140, 144, 147, 140, 140, 155, 179, 175 },
                new float[]{ 144, 152, 140, 147, 140, 148, 167, 179 },
                new float[]{ 152, 155, 136, 167, 163, 162, 152, 172 },
                new float[]{ 168, 145, 156, 160, 152, 155, 136, 160 },
                new float[]{ 162, 148, 156, 148, 140, 136, 147, 162 },
                new float[]{ 147, 167, 140, 155, 155, 140, 136, 162 },
                new float[]{ 136, 156, 123, 167, 162, 144, 140, 147 },
                new float[]{ 148, 155, 136, 155, 152, 147, 147, 136 }
            };
        private float[][] outputMatrix = {
                new float[]{ 186, -18, 15, -9, 23, -9, -14, -19 },
                new float[]{ 21, -34, 26, -9, -11, 11, 14, 7 },
                new float[]{ -10, -24, -2, 6, -18, 3, -20, -1 },
                new float[]{ -8, -5, 14, -15, -8, -3, -3, 8 },
                new float[]{ -3, 10, 8, 1, -11, 18, 18, 15 },
                new float[]{ 4, -2, -18, 8, 8, -4, 1, -7 },
                new float[]{ 9, 1, -3, 4, -1, -7, -1, -2 },
                new float[]{ 0, -8, -2, 2, 1, 4, -6, 0 }
            };
        [Fact]
        public void DctTest()
        {        
            var zk = new ZhaoKoch();
            var output = zk.Dct(inputMatrix);
            var result = new float[8][];
            for(int i = 0; i< result.Length; i++)
            {
                result[i] = new float[8];
                for(int j = 0; j < result.Length; j++)
                {
                    result[i][j] = (float)Math.Round(output[i][j], 0);
                }
            }
            Assert.Equal(outputMatrix, result);
        }

        [Fact]
        public void DctInvTest()
        {
            var zk = new ZhaoKoch();
            var dct = zk.Dct(inputMatrix);
            var output = zk.DctInv(dct);
            var result = new float[8][];
            for(int i = 0; i< result.Length; i++)
            {
                result[i] = new float[8];
                for(int j = 0; j < result.Length; j++)
                {
                    result[i][j] = (float)Math.Round(output[i][j], 0);
                }
            }
            Assert.Equal(inputMatrix, result);
        }
    }
}