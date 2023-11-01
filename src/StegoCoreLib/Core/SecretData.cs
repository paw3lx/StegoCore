using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StegoCore.Core;

public class SecretData
{
    public SecretData(byte[] fileBytes)
    {
        FileBytes = fileBytes;
        FileLength = fileBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(this.FileLength); // length 4 = 32 bits
        FileWithLengthBytes = lengthBytes.Concat(fileBytes);
    }

    public int FileLength { get; }

    public byte[] FileBytes { get; }

    public IEnumerable<byte> FileWithLengthBytes { get; }

    public BitArray SecretWithLengthBits => new(FileWithLengthBytes.ToArray());

    public BitArray SecretBits => new(FileBytes);
}
