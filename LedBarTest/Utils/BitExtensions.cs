using System;
using System.Collections;

namespace LedBarTest.ViewModels
{
    public static class BitExtensions
    {
        public static byte ToByte(this BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }

            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }
}