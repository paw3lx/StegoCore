using System;
using System.Collections;
using System.IO;
using ImageSharp;
using StegoCore.Core;
using StegoCore.Extensions;

namespace StegoCore.Algorithms
{
    public class Lsb : StegoAlgorithm
    {
        public override Image Embed(Image baseImage, SecretData secret)
        {
            BitArray secretBits = secret.SecretWithLengthBits;
            if (EmbedPossible(baseImage, secretBits.Length) == false)
                throw new InvalidDataException("Secret data is to big for embending.");
            using(var pixels = baseImage.Lock())
            {
                for (int i = 0; i < baseImage.Height; i++)
                {
                    for (int j = 0; j < baseImage.Width; j++)
                    {               
                        int index = (i * baseImage.Width + j) * 2;
                        if (index >= secretBits.Length - 2)
                            break;
                        var pixel = pixels[j, i];
                        pixel.R = SetLsb(pixel.R, secretBits[index]);
                        pixel.B = SetLsb(pixel.B, secretBits[index + 1]);
                        pixels[j, i] = pixel;
                    }
                }              
            }

            return baseImage;
        }

        public override byte[] Decode(Image stegoImage)
        {
            int length  = ReadSecretLength(stegoImage) * 8;
            if (length <= 0 || !EmbedPossible(stegoImage, length))
                throw new InvalidDataException($"Cannot read secret from this image file. Readed secret length: {length}");

            BitArray bits = new BitArray(length);
            using(var pixels = stegoImage.Lock())
            {         
                for (int i = 0; i < pixels.Height; i++)
                {
                    for (int j = 0; j < pixels.Width; j++)
                    {               
                        int index = (i * pixels.Width + j) * 2;
                        if (index < this.SecretDataLength)
                            continue;
                        if (index >= length + this.SecretDataLength)
                            break;
                        var r = pixels[j, i].R;
                        var b = pixels[j, i].B;
                        bool bitR = GetBit(r, 0);
                        bool bitB = GetBit(b, 0);
                        bits.Set(index - this.SecretDataLength, bitR);
                        bits.Set(index + 1 - this.SecretDataLength, bitB);
                    }
                }
            }
            return bits.ToByteArray();
        }

        public override int ReadSecretLength(Image stegoImage)
        {
            BitArray lengthBits = new BitArray(this.SecretDataLength);
            using(var pixels = stegoImage.Lock())
            {
                for (int i = 0; i < pixels.Height; i++)
                {
                    for (int j = 0; j < pixels.Width; j++)
                    {               
                        int index = (i * pixels.Width + j) * 2;
                        if (index >= lengthBits.Length)
                            break;
                        var r = pixels[j, i].R;
                        var b = pixels[j, i].B;
                        bool bitR = GetBit(r, 0);
                        bool bitB = GetBit(b, 0);
                        lengthBits.Set(index, bitR);
                        lengthBits.Set(index + 1, bitB);
                    }
                }
            }
            byte[] bytes = lengthBits.ToByteArray();
            int length = BitConverter.ToInt32(bytes, 0);
            return length;
        }

        private static bool GetBit(byte b, int bitNumber) 
        {
            return (b & (1 << bitNumber)) != 0;
        }

        private static byte SetLsb(byte b, bool value)
        {
            byte ret = b;
            if (value)
                ret =  (byte)(b| 1);     // Make LSB 1
            else
                ret = (byte)(b & 254);   // Make LSB 0
            return ret;
        }

        public override bool EmbedPossible(Image image, int secretLength)
        {
            return image.Width * image.Height * 2 >= secretLength;
        }
    }
}