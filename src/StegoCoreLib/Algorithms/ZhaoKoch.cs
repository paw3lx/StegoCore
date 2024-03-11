using System;
using System.Collections;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Core;
using StegoCore.Exceptions;
using StegoCore.Extensions;
using StegoCore.Model;

namespace StegoCore.Algorithms;

public class ZhaoKoch : StegoAlgorithm
{
    private int _d;
    public override Image<Rgba32> Embed(Image<Rgba32> baseImage, SecretData secret, ISettings settings = null)
    {
        BitArray secretBits = secret.SecretWithLengthBits;
        if (IsEmbedPossible(baseImage, secretBits.Length) == false)
            throw new InvalidDataException("Secret data is to big for embending.");
        _d = settings is Settings gSet ? gSet.D : 5;
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
            PixelLuma[][] luminanceMatrix = GetLuminanceMatrix(baseImage, width, height);
            float[][] yMatrix = luminanceMatrix.GetY();
            float[][] matrixWithBit = InsertOneBit(yMatrix, secretBits.Get(index), _d);
            luminanceMatrix = luminanceMatrix.SetY(matrixWithBit);
            SetLuminance(baseImage, luminanceMatrix, width, height);
            index++;
            width += 8;
        }

        return baseImage;
    }

    public override bool IsEmbedPossible(Image<Rgba32> image, int secretLength)
    {
        int count = (image.Width / 8) * (image.Height / 8);
        return count >= secretLength;
    }

    public override byte[] Decode(Image<Rgba32> stegoImage, ISettings settings = null)
    {
        int length = ReadSecretLength(stegoImage, settings) * 8;
        if (length <= 0 || !IsEmbedPossible(stegoImage, length))
            throw new DecodeException($"Cannot read secret from this image file. Readed secret length: {length}");
        _d = settings is Settings gSet ? gSet.D : 5;
        BitArray bits = ReadBits(stegoImage, this.SecretDataLength, length + this.SecretDataLength);
        return bits.ToByteArray();
    }

    public override int ReadSecretLength(Image<Rgba32> stegoImage, ISettings settings = null)
    {
        BitArray lengthBits = ReadBits(stegoImage, 0, this.SecretDataLength);
        byte[] bytes = lengthBits.ToByteArray();
        int length = BitConverter.ToInt32(bytes, 0);
        return length;
    }

    private BitArray ReadBits(Image<Rgba32> stegoImage, int start, int end)
    {
        int length = end - start;
        if (length <= 0)
            throw new InvalidDataException("end has to be > than start");
        BitArray bits = new BitArray(length);
        int width = 0;
        int height = 0;
        int index = 0;
        while (index < end)
        {
            if (width + 8 > stegoImage.Width)
            {
                height += 8;
                width = 0;
            }
            if (height + 8 >= stegoImage.Height)
            {
                break;
            }
            if (index < start)
            {
                index++;
                width += 8;
                continue;
            }
            PixelLuma[][] luminanceMatrix = GetLuminanceMatrix(stegoImage, width, height);
            float[][] yMatrix = luminanceMatrix.GetY();
            bool bit = ReadOneBit(yMatrix, _d);
            bits.Set(index - start, bit);
            index++;
            width += 8;
        }

        return bits;
    }

    private static PixelLuma[][] GetLuminanceMatrix(Image<Rgba32> pixels, int width, int height)
    {
        int i, j;
        PixelLuma[][] luminance = new PixelLuma[8][];
        for (i = 0; i < 8; i++)
        {
            luminance[i] = new PixelLuma[8];
            for (j = 0; j < 8; j++)
            {
                Rgba32 pixel = pixels[width + i, height + j];
                PixelLuma luma = new(pixel.R, pixel.G, pixel.B);
                luminance[i][j] = luma;
            }
        }
        return luminance;
    }

    private static void SetLuminance(Image<Rgba32> pixels, PixelLuma[][] luminanceMatrix, int width, int height)
    {
        int i, j;
        for (i = 0; i < 8; i++)
        {
            for (j = 0; j < 8; j++)
            {
                Rgba32 pixel = pixels[width + i, height + j];
                PixelLuma luma = luminanceMatrix[i][j];
                pixel.R = luma.R;
                pixel.G = luma.G;
                pixel.B = luma.B;
                pixels[width + i, height + j] = pixel;
            }
        }
    }

    public float[][] InsertOneBit(float[][] matrix, bool bit, float d)
    {
        float[][] quantizeMatrix = Dct(matrix);

        float k1 = quantizeMatrix[3][4];
        float k2 = quantizeMatrix[4][3];
        float k = Math.Abs(k1) - Math.Abs(k2);
        if (bit)
        {
            while (!(Math.Abs(k1) - Math.Abs(k2) < -(d + 5)))
            {
                if (k2 < 0)
                    k2--;
                if (k2 >= 0)
                    k2++;
            }
        }
        else
        {
            while (!(Math.Abs(k1) - Math.Abs(k2) > (d + 5)))
            {
                if (k1 < 0)
                    k1--;
                if (k1 >= 0)
                    k1++;
            }
        }
        quantizeMatrix[3][4] = k1;
        quantizeMatrix[4][3] = k2;
        float[][] outMatrix = DctInv(quantizeMatrix);
        return outMatrix;
    }

    private bool ReadOneBit(float[][] matrix, float d)
    {
        float[][] quantized = Dct(matrix);
        float k1 = quantized[3][4];
        float k2 = quantized[4][3];
        if (Math.Abs(k1) - Math.Abs(k2) < -d)
            return true;
        return false;
    }

    public static float[][] Dct(float[][] baseMatrix)
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

    public static float[][] DctInv(float[][] dct)
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
                        if (u == 0) t *= 1 / (float)Math.Sqrt(2);
                        if (v == 0) t *= 1 / (float)Math.Sqrt(2);
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
