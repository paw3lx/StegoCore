using System.Collections;

namespace StegoCoreLib.Extensions
{
    public static class BitArrayExtensions
    {
        /// <summary>
        /// Converts BitArray to byte array
        /// </summary>
        /// <param name="bits">BitArray</param>
        /// <returns>representation in byte[]</returns>
        public static byte[] ToByteArray(this BitArray bits) 
        {
            int numBytes = bits.Length / 8;
            if (bits.Length % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Length; i++) {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (0 + bitIndex));

                bitIndex++;
                if (bitIndex == 8) {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }
        
    }
}