using System;
using System.Collections;
using System.IO;
using ImageSharp;
using StegoCore.Core;
using StegoCore.Extensions;
using StegoCore.Exceptions;
using StegoCore.Model;
using System.Collections.Generic;

namespace StegoCore.Algorithms
{
    public class Lsb : StegoAlgorithm
    {
        public override Image Embed(Image baseImage, SecretData secret, Settings settings)
        {
            BitArray secretBits = secret.SecretWithLengthBits;
            if (EmbedPossible(baseImage, secretBits.Length) == false)
                throw new DataToBigException("Secret data is to big for embending.");
            Random r = new Random((settings?.Key ?? string.Empty).GetHashCode());
            using(var pixels = baseImage.Lock())
            {
                int ind = 0;
                while(ind < secretBits.Length)
                {
                    List<Tuple<int,int>> occupied = new List<Tuple<int, int>>();
                    int width = r.Next(pixels.Width);
                    int height = r.Next(pixels.Height);
                    var pair = new Tuple<int,int>(width, height);
                    if (occupied.Contains(pair))
                        continue;
                    occupied.Add(pair);
                    var pixel = pixels[width, height];
                    pixel.R = SetLsb(pixel.R, secretBits[ind]);
                    pixel.B = SetLsb(pixel.B, secretBits[ind + 1]);
                    pixels[width, height] = pixel;
                    ind += 2;
                }
                



                // for (int i = 0; i < baseImage.Height; i++)
                // {
                //     for (int j = 0; j < baseImage.Width; j++)
                //     {               
                //         int index = (i * baseImage.Width + j) * 2;
                //         if (index >= secretBits.Length - 2)
                //             break;
                //         var pixel = pixels[j, i];
                //         pixel.R = SetLsb(pixel.R, secretBits[index]);
                //         pixel.B = SetLsb(pixel.B, secretBits[index + 1]);
                //         pixels[j, i] = pixel;
                //     }
                // }              
            }

            return baseImage;
        }

        public override byte[] Decode(Image stegoImage, Settings settings)
        {
            int length = ReadSecretLength(stegoImage) * 8;
            if (length <= 0 || !EmbedPossible(stegoImage, length))
                throw new DecodeException($"Cannot read secret from this image file. Readed secret length: {length}");
            BitArray bits = ReadBits(stegoImage, this.SecretDataLength, length + this.SecretDataLength);
            return bits.ToByteArray();
        }

        public override int ReadSecretLength(Image stegoImage)
        {
            BitArray lengthBits = ReadBits(stegoImage, 0, this.SecretDataLength);
            byte[] bytes = lengthBits.ToByteArray();
            int length = BitConverter.ToInt32(bytes, 0);
            return length;
        }

        private BitArray ReadBits(Image stegoImage, int start, int end)
        {
            int length = end - start;
            if (length <= 0)
                throw new InvalidDataException("end has to be > than start");
            BitArray bits = new BitArray(length);
            int index = 0;
            Random random = new Random((string.Empty).GetHashCode());
            List<Tuple<int,int>> occupied = new List<Tuple<int, int>>();
            using (var pixels = stegoImage.Lock())
            {
                while (index < end)
                {
                    int width = random.Next(stegoImage.Width);
                    int height = random.Next(stegoImage.Height);
                    var pair = new Tuple<int,int>(width, height);
                    if (occupied.Contains(pair))
                        continue;
                    occupied.Add(pair);
                    if (index < start)
                    {
                        index += 2;
                        continue;
                    }
                    var pixel = pixels[width, height];
                    var r = pixel.R;
                    var b = pixel.B;
                    bool bitR = GetBit(r, 0);
                    bool bitB = GetBit(b, 0);
                    bits.Set(index - start, bitR);
                    bits.Set(index - start + 1, bitB);
                    index += 2;
                }

            }
            return bits;
            
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