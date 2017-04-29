﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageSharp;
using ImageSharp.PixelFormats;
using StegoCore.Core;
using StegoCore.Model;

namespace StegoCore.Algorithms
{
    public class ZhaoKoch : IStegoAlgorithm
    {
        public Image Embed(Image baseImage, SecretData secret)
        {
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
                    var luminanceMatrix = ReadLuminanceMatrix(pixels, width, height);
                    index++;
                    width++;
                    if ((width * 8) + 8 > baseImage.Width)
                    {
                        width = 0;
                        height++;
                    }
                }

            }

            return baseImage;
        }

        private float[][] ReadLuminanceMatrix(PixelAccessor<Rgba32> pixels, int width, int height)
        {
            if (width > (pixels.Width + 8))
            {
                width = 0;
                height += 8;
            }
            if (height > pixels.Height + 8)
            {
                return null;
            }
            int i, j;
            float[][] luminance = new float[8][];
            for (i = 0; i < 8; i++)
            {
                luminance[i] = new float[8];
                for (j = 0; j < 8; j++)
                {
                    luminance[i][j] = new float();
                    var pixel = pixels[width * 8 + i, height * 8 + j];
                    PixelLuma luma = new PixelLuma(pixel.R, pixel.G, pixel.B);
                    luminance[i][j] = luma.Y;
                }
            }
            return luminance;
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

     

        public byte[] Decode(Image stegoImage)
        {
            throw new NotImplementedException();
        }

        public int ReadSecretLength(Image stegoImage)
        {
            throw new NotImplementedException();
        }

        public bool EmbedPossible(Image image, int secretLength)
        {
            return true;
        }

        private float[][] Dct(float[][] baseMatrix)
        {
            float[][] dctMatrix = new float[8][];
            float[][] matrix = new float[8][];
            int i, j, k, l;
            for (i = 0; i < 8; i++)
            {
                matrix[i] = new float[8];
                for (j = 0; j < 8; j++)
                {
                    matrix[i][j] = new float();
                    matrix[i][j] = baseMatrix[i][j] - 128;
                }
            }
            for (k = 0; k < 8; k++)
            {
                dctMatrix[k] = new float[8];
                for (l = 0; l < 8; l++)
                {
                    dctMatrix[k][l] = new float();
                    dctMatrix[k][l] = 0;
                    for (i = 0; i < 8; i++)
                    {
                        for (j = 0; j < 8; j++)
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

        private float[][] Quantize(float[][] matrix)
        {
            int i, j;

            float[][] quantizeMatrix = new float[8][];
            for (i = 0; i < 8; i++)
            {
                quantizeMatrix[i] = new float[8];
                for (j = 0; j < 8; j++)
                {
                    quantizeMatrix[i][j] = new float();
                    quantizeMatrix[i][j] = matrix[i][j] / Statics.JPEG.JpegLuminQuantTable[i][j];
                }
            }
            return quantizeMatrix;
        }

        private float[][] Dequantize(float[][] quantizedMatrix)
        {
            int i, j;

            float[][] matrix = new float[8][];
            for (i = 0; i < 8; i++)
            {
                matrix[i] = new float[8];
                for (j = 0; j < 8; j++)
                {
                    matrix[i][j] = new float();
                    matrix[i][j] = quantizedMatrix[i][j] * Statics.JPEG.JpegLuminQuantTable[i][j];
                }
            }
            return matrix;
        }

        private float[][] DctInv(float[][] dct)
        {
            float[][] dctMatrixInt = new float[8][];
            float[][] matrix = new float[8][];
            int x, y, u, v;
            for (x = 0; x < 8; x++)
            {
                dctMatrixInt[x] = new float[8];
                for (y = 0; y < 8; y++)
                {
                    float ammount = 0;
                    dctMatrixInt[x][y] = new float();
                    dctMatrixInt[x][y] = 0;
                    for (u = 0; u < 8; u++)
                    {
                        for (v = 0; v < 8; v++)
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

            for (int i = 0; i < 8; i++)
            {
                matrix[i] = new float[8];
                for (int j = 0; j < 8; j++)
                {
                    matrix[i][j] = new float();
                    matrix[i][j] = dctMatrixInt[i][j] + 128;
                }
            }
            return matrix;
        }
    }
}
