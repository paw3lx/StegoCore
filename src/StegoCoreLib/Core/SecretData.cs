using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace StegoCore.Core
{
    public class SecretData
    {
        public SecretData(byte[] fileBytes)
        {
            this.fileBytes = fileBytes;
            this.fileLength = fileBytes.Length;
            var lengthBytes = BitConverter.GetBytes(this.fileLength); // length 4 = 32 bits
            secretWithLength = lengthBytes.Concat(fileBytes);
        }

        private byte[] fileBytes;
        private int fileLength;
        private IEnumerable<byte> secretWithLength;

        public int FileLength => fileLength;
        public byte[] FileBytes => fileBytes;
        public IEnumerable<byte> FileWithLengthBytes => secretWithLength;

        public BitArray SecretWithLengthBits => new BitArray(secretWithLength.ToArray());

        public BitArray SecretBits => new BitArray(fileBytes);



        
    }
}