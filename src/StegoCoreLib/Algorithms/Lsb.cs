using System;
using System.Collections;
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
            using(var pixels = baseImage.Lock())
            {
                for (int i = 0; i < baseImage.Height; i++)
                {
                    for (int j = 0; j < baseImage.Width; j++)
                    {               
                        int index = i * baseImage.Width + j;
                        if (index >= secretBits.Length)
                            break;
                        var pixel = pixels[j, i];
                        if (secretBits[index])
                        {
                            pixel.R = (byte)(pixel.R | 1);     // Make LSB 1
                        }
                        else
                        {
                            pixel.R = (byte)(pixel.R & 254);   // Make LSB 0
                        }
                        pixels[j, i] = pixel;
                    }
                }              
            }

            return baseImage;
        }

        public override byte[] Decode(Image stegoImage)
        {
            int length  = ReadSecretLength(stegoImage) * 8;
            BitArray bits = new BitArray(length);
            using(var pixels = stegoImage.Lock())
            {         
                for (int i = 0; i < pixels.Height; i++)
                {
                    for (int j = 0; j < pixels.Width; j++)
                    {               
                        int index = i * pixels.Width + j;
                        if (index < this.SecretDataLength)
                            continue;
                        if (index >= length + this.SecretDataLength)
                            break;
                        var r = pixels[j, i].R;
                        bool bit = GetBit(r, 0);
                        bits.Set(index - this.SecretDataLength, bit);
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
                        int index = i * pixels.Width + j;
                        if (index >= lengthBits.Length)
                            break;
                        var r = pixels[j, i].R;
                        bool bit = GetBit(r, 0);
                        lengthBits.Set(index, bit);
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
    }
}