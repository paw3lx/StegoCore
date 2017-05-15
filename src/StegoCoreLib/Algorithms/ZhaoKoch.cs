using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using ImageSharp;
using ImageSharp.PixelFormats;
using StegoCore.Core;
using StegoCore.Model;
using StegoCore.Extensions;

namespace StegoCore.Algorithms
{
    public class ZhaoKoch : StegoAlgorithm
    {
        public override Image Embed(Image baseImage, SecretData secret)
        {
            var d = 2;
            var md = 1;
            BitArray secretBits = secret.SecretWithLengthBits;
            if (EmbedPossible(baseImage, secretBits.Length) == false)
                throw new InvalidDataException("Secret data is to big for embending.");
            using (var pixels = baseImage.Lock())
            {
                int width = 0;
                int height = 0;
                int index = 0;
                while (index < secretBits.Length)
                {
                    if (width + 8 > baseImage.Width)
                    {
                        height += 8;
                        width = 0;
                    }
                    if (height + 8 >= baseImage.Height)
                    {
                        break;
                    }
                    var luminanceMatrix = GetLuminanceMatrix(pixels, width, height);
                    var yMatrix = luminanceMatrix.GetY();
                    if (PossibleInsertBit(yMatrix, secretBits.Get(index), md))
                    {                          
                        var matrixWithBit = InsertOneBit(yMatrix, secretBits.Get(index), d);
                        luminanceMatrix = luminanceMatrix.SetY(matrixWithBit);
                        SetLuminance(pixels, luminanceMatrix, width, height);
                        index++;  
                    }
                    width += 8;
                }

            }
            return baseImage;
        }

        private PixelLuma[][] GetLuminanceMatrix(PixelAccessor<Rgba32> pixels, int width, int height)
        {
            int i, j;
            PixelLuma[][] luminance = new PixelLuma[8][];
            for (i = 0; i < 8; i++)
            {
                luminance[i] = new PixelLuma[8];
                for (j = 0; j < 8; j++)
                {
                    var pixel = pixels[width + i, height + j];
                    PixelLuma luma = new PixelLuma(pixel.R, pixel.G, pixel.B);
                    luminance[i][j] = luma;
                }
            }
            return luminance;
        }

        private void SetLuminance(PixelAccessor<Rgba32> pixels, PixelLuma[][] luminanceMatrix, int width, int height)
        {
            int i, j;
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    var pixel = pixels[width + i, height + j];
                    var luma = luminanceMatrix[i][j];
                    pixel.R = luma.R;
                    pixel.G = luma.G;
                    pixel.B = luma.B;
                    pixels[width + i, height + j] = pixel;
                }
            }
        }

        public bool PossibleInsertBit(float[][] matrix, bool c, int md)
        {
            float[][] quantized = Quantize(Dct(matrix));
            if (c == true)
            {
                if (Math.Min(Math.Abs(quantized[1][1]), Math.Abs(quantized[2][0])) + md < Math.Abs(quantized[0][2]))
                {
                    return false;
                }
            }
            else
            {
                if (Math.Max(Math.Abs(quantized[1][1]), Math.Abs(quantized[2][0])) > Math.Abs(quantized[0][2]) + md)
                {
                    return false;
                }
            }
            return true;
        }

        public float[][] InsertOneBit(float[][] matrix, bool bit, float d)
        {
            float[][] quantizeMatrix = Quantize(Dct(matrix));

            float k1 = quantizeMatrix[1][1];
            float k2 = quantizeMatrix[2][0];
            float k3 = quantizeMatrix[0][2];
            if (bit)
            {
                k3 += d;
                do
                {
                    k1 += (float)0.05;
                    k2 += (float)0.05;
                    k3 -= (float)0.01;
                }
                while ((k1 <= k3) || (k2 <= k3));

                //write
                quantizeMatrix[1][1] = k1;
                quantizeMatrix[2][0] = k2;
                quantizeMatrix[0][2] = k3 - d;
            }
            else
            {

                k1 += d;
                k2 += d;

                do
                {
                    k1 -= (float)0.05;
                    k2 -= (float)0.05;
                    k3 += (float)0.01;
                }
                while ((k1 >= k3) || (k2 >= k3));

                quantizeMatrix[1][1] = k1 - d;
                quantizeMatrix[2][0] = k2 - d;
                quantizeMatrix[0][2] = k3;

            }
            float[][] outMatrix = DctInv(Dequantize(quantizeMatrix));

            return outMatrix;
        }

     

        public override byte[] Decode(Image stegoImage)
        {
            throw new NotImplementedException();
        }

        public bool PossibleReadBit(float[][] matrix, float d)
        {
            float[][] quantized = Quantize(Dct(matrix));
            float k1 = quantized[1][1];
            float k2 = quantized[2][0];
            float k3 = quantized[0][2];
            if ((k1 > k3 + d) && (k2 > k3 + d)) return true;
            if ((k1 + d < k3) && (k2 + d < k3)) return true;
            return false;
        }

        public bool ReadOneBit(float[][] matrix, float d)
        {
            float[][] quantized = Quantize(Dct(matrix));
            float k1 = quantized[1][1];
            float k2 = quantized[2][0];
            float k3 = quantized[0][2];

            if ((k1 > k3 + d) && (k2 > k3 + d)) return true;
            if ((k1 + d < k3) && (k2 + d < k3)) return false;
            return true;
        }

        public override int ReadSecretLength(Image stegoImage)
        {
            throw new NotImplementedException();
        }

        public override bool EmbedPossible(Image image, int secretLength)
        {
            int count = (image.Width / 8) * (image.Height / 8);
            return count >= secretLength;
        }   

        

        public float[][] Quantize(float[][] matrix)
        {
            if (matrix.Length != 8 || matrix.Any(x => x.Length != 8))
                throw new InvalidDataException("Matrix width and height are not equal 8");
            int i, j;

            float[][] quantizeMatrix = new float[8][];
            for (i = 0; i < 8; i++)
            {
                quantizeMatrix[i] = new float[8];
                for (j = 0; j < 8; j++)
                {
                    quantizeMatrix[i][j] = matrix[i][j] / Statics.JPEG.JpegLuminQuantTable[i][j];
                }
            }
            return quantizeMatrix;
        }

        public float[][] Dequantize(float[][] quantizedMatrix)
        {
            if (quantizedMatrix.Length != 8 || quantizedMatrix.Any(x => x.Length != 8))
                throw new InvalidDataException("Matrix width and height are not equal 8");
            int i, j;

            float[][] matrix = new float[8][];
            for (i = 0; i < 8; i++)
            {
                matrix[i] = new float[8];
                for (j = 0; j < 8; j++)
                {
                    matrix[i][j] = quantizedMatrix[i][j] * Statics.JPEG.JpegLuminQuantTable[i][j];
                }
            }
            return matrix;
        }

        public float[][] Dct(float[][] baseMatrix)
        {
            if (baseMatrix.Length != baseMatrix[0].Length)
                throw new InvalidDataException("Matrix width and height are different");

            int length = baseMatrix.Length;
            float[][] dctMatrix = new float[length][];
            float[][] matrix = new float[length][];
            int i, j, k, l;
            for (i = 0; i < length; i++)
            {
                matrix[i] = new float[length];
                for (j = 0; j < length; j++)
                {
                    matrix[i][j] = baseMatrix[i][j] - 128;
                }
            }
            for (k = 0; k < length; k++)
            {
                dctMatrix[k] = new float[length];
                for (l = 0; l < length; l++)
                {
                    dctMatrix[k][l] = 0;
                    for (i = 0; i < length; i++)
                    {
                        for (j = 0; j < length; j++)
                        {
                            dctMatrix[k][l] += matrix[i][j] * (float)Math.Cos((2 * i + 1) * k * Math.PI / 16) * (float)Math.Cos((2 * j + 1) * l * Math.PI / 16);
                        }
                    }
                    if (k == 0) dctMatrix[k][l] *= 1 / ((float)Math.Sqrt(2));
                    if (l == 0) dctMatrix[k][l] *= 1 / ((float)Math.Sqrt(2));
                    dctMatrix[k][l] /= 4;
                }

            }
            return dctMatrix;
        }

        public float[][] DctInv(float[][] dct)
        {
            if (dct.Length != dct[0].Length)
                throw new InvalidDataException("Matrix width and height are different");

            int length = dct.Length;
            float[][] dctMatrixInt = new float[length][];
            float[][] matrix = new float[length][];
            int x, y, u, v;
            for (x = 0; x < length; x++)
            {
                dctMatrixInt[x] = new float[length];
                for (y = 0; y < length; y++)
                {
                    float ammount = 0;
                    dctMatrixInt[x][y] = 0;
                    for (u = 0; u < length; u++)
                    {
                        for (v = 0; v < length; v++)
                        {
                            var t = dct[u][v] * (float)Math.Cos(u * (x + 0.5) * Math.PI / 8) * (float)Math.Cos(v * (y + 0.5) * Math.PI / 8);
                            if (u == 0) t *= (1 / (float)Math.Sqrt(2));
                            if (v == 0) t *= (1 / (float)Math.Sqrt(2));
                            ammount += t;
                        }
                    }
                    ammount /= 4;
                    dctMatrixInt[x][y] = ammount;
                }
            }

            for (int i = 0; i < length; i++)
            {
                matrix[i] = new float[length];
                for (int j = 0; j < length; j++)
                {
                    matrix[i][j] = dctMatrixInt[i][j] + 128;
                }
            }
            return matrix;
        }
    }
}
