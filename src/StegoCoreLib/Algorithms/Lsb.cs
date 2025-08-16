using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore.Core;
using StegoCore.Exceptions;
using StegoCore.Extensions;
using StegoCore.Model;

namespace StegoCore.Algorithms;

public class Lsb : StegoAlgorithm
{
    public override Image<Rgba32> Embed(Image<Rgba32> baseImage, SecretData secret, ISettings? settings = null)
    {
        BitArray secretBits = secret.SecretWithLengthBits;
        if (IsEmbedPossible(baseImage, secretBits.Length) == false)
            throw new InvalidDataException("Secret data is to big for embending.");
        Random random = GetRandomGenenator(settings);
        int index = 0;
        HashSet<Tuple<int, int>> occupied = new();

        baseImage.ProcessPixelRows(accessor =>
        {
            while (index < secretBits.Length)
            {
                int width = random.Next(baseImage.Width);
                int height = random.Next(baseImage.Height);
                var pair = new Tuple<int, int>(width, height);
                if (occupied.Contains(pair))
                    continue;
                occupied.Add(pair);

                // set pixel
                Span<Rgba32> rowSpan = accessor.GetRowSpan(height);
                ref Rgba32 pixel = ref rowSpan[width];
                pixel.R = SetLsb(pixel.R, secretBits[index]);
                pixel.B = SetLsb(pixel.B, secretBits[index + 1]);
                index += 2;
            }
        });

        return baseImage;
    }

    public override byte[] Decode(Image<Rgba32> stegoImage, ISettings? settings = null)
    {
        int length = ReadSecretLength(stegoImage, settings) * 8;
        if (length <= 0 || !IsEmbedPossible(stegoImage, length))
            throw new DecodeException($"Cannot read secret from this image file. Readed secret length: {length}");
        BitArray bits = ReadBits(stegoImage, this.SecretDataLength, length + this.SecretDataLength, settings?.Key);
        return bits.ToByteArray();
    }

    public override int ReadSecretLength(Image<Rgba32> stegoImage, ISettings? settings = null)
    {
        BitArray lengthBits = ReadBits(stegoImage, 0, this.SecretDataLength, settings?.Key);
        byte[] bytes = lengthBits.ToByteArray();
        int length = BitConverter.ToInt32(bytes, 0);
        return length;
    }

    private static BitArray ReadBits(Image<Rgba32> stegoImage, int start, int end, string? key)
    {
        int length = end - start;
        if (length <= 0)
            throw new InvalidDataException("end has to be > than start");
        BitArray bits = new(length);
        int index = 0;
        Random random = GetRandomGenenator(key);
        HashSet<Tuple<int, int>> occupied = new();
        while (index < end)
        {
            int width = random.Next(stegoImage.Width);
            int height = random.Next(stegoImage.Height);
            var pair = new Tuple<int, int>(width, height);
            if (occupied.Contains(pair))
                continue;
            occupied.Add(pair);
            
            if (index < start)
            {
                index += 2;
                continue;
            }

            stegoImage.ProcessPixelRows(accessor => {
                Span<Rgba32> rowSpan = accessor.GetRowSpan(height);
                Rgba32 pixel = rowSpan[width];
                (bool bitR, bool bitB) = GetBitsFromPixel(pixel);
                bits.Set(index - start, bitR);
                bits.Set(index - start + 1, bitB);
                index += 2;
            });
        }
        return bits;

    }

    private static (bool R, bool B) GetBitsFromPixel(Rgba32 pixel)
    {
        byte r = pixel.R;
        byte b = pixel.B;
        bool bitR = GetBit(r, 0);
        bool bitB = GetBit(b, 0);
        return (bitR, bitB);
    }

    private static bool GetBit(byte b, int bitNumber)
    {
        return (b & (1 << bitNumber)) != 0;
    }

    private static byte SetLsb(byte b, bool value)
    {
        if (value)
            return unchecked((byte)(b | 0x01)); // Make LSB 1
        else
            return unchecked((byte)(b & 0xFE)); // Make LSB 0
    }

    public override bool IsEmbedPossible(Image<Rgba32> image, int secretLength)
    {
        return image.Width * image.Height * 2 >= secretLength;
    }
}
