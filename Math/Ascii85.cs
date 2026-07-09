using System;
using System.Text;
using System.IO;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Conversion between binary data and an Ascii85 string
    /// </summary>
    public static class Ascii85
    {
        /// <summary>
        /// Encodes a byte array into an Ascii85 string
        /// </summary>
        public static string Encode(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            // Calculate the exact string length to allocate exactly enough memory
            int exactLength = GetEncodedLength(bytes);
            return string.Create(exactLength, bytes, static (chars, state) => // Writes the encoded data into the Span
            {
                int charIdx = 0;
                int i = 0;

                // Stack-allocate for digits, no GC involvement
                Span<char> temp = stackalloc char[5];

                // Process full groups of 4 bytes
                while (i <= state.Length - 4)
                {
                    uint val = (uint)(state[i] << 24) | (uint)(state[i + 1] << 16) |
                               (uint)(state[i + 2] << 8) | state[i + 3];
                    if (val == 0)
                    {
                        chars[charIdx++] = 'z';
                    }
                    else
                    {
                        uint v = val;
                        for (int k = 4; k >= 0; k--)
                        {
                            temp[k] = (char)(c_firstCharacter + (v % 85));
                            v /= 85;
                        }
                        temp.CopyTo(chars.Slice(charIdx, 5));
                        charIdx += 5;
                    }
                    i += 4;
                }

                // Process the tail (1–3 bytes)
                int rem = state.Length - i;
                if (rem > 0)
                {
                    uint val = 0;
                    for (int j = 0; j < rem; j++)
                        val |= (uint)(state[i + j]) << (24 - 8 * j);

                    int charsToWrite = rem + 1; // correct number of characters

                    uint v = val;
                    for (int k = 4; k >= 0; k--)
                    {
                        temp[k] = (char)(c_firstCharacter + (v % 85));
                        v /= 85;
                    }
                    temp.Slice(0, charsToWrite).CopyTo(chars.Slice(charIdx, charsToWrite));
                }
            });
        }

        /// <summary>
        /// Decodes an Ascii85 string into a byte array
        /// </summary>
        public static byte[] Decode(string encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            int decodedLength = GetDecodedLength(encoded);
            byte[] result = new byte[decodedLength];

            int byteIdx = 0;
            int count = 0;
            uint value = 0;

            for (int i = 0; i < encoded.Length; i++)
            {
                char ch = encoded[i];
                if (ch == 'z' && count == 0)
                {
                    result[byteIdx++] = 0;
                    result[byteIdx++] = 0;
                    result[byteIdx++] = 0;
                    result[byteIdx++] = 0;
                }
                else if (ch < c_firstCharacter || ch > c_lastCharacter)
                {
                    throw new FormatException($"Invalid character '{ch}' in Ascii85 block.");
                }
                else
                {
                    try
                    {
                        uint add = checked(s_powersOf85[count] * (uint)(ch - c_firstCharacter));
                        value = checked(value + add);
                    }
                    catch (OverflowException ex)
                    {
                        throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
                    }

                    count++;

                    if (count == 5)
                    {
                        result[byteIdx++] = (byte)(value >> 24);
                        result[byteIdx++] = (byte)(value >> 16);
                        result[byteIdx++] = (byte)(value >> 8);
                        result[byteIdx++] = (byte)(value);
                        count = 0;
                        value = 0;
                    }
                }
            }

            if (count == 1)
                throw new FormatException("The final Ascii85 block must contain more than one character.");

            if (count > 1)
            {
                // Pad missing characters with maximum values
                for (int padding = count; padding < 5; padding++)
                {
                    try
                    {
                        value = checked(value + 84u * s_powersOf85[padding]);
                    }
                    catch (OverflowException ex)
                    {
                        throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
                    }
                }

                result[byteIdx++] = (byte)(value >> 24);
                if (count > 2) result[byteIdx++] = (byte)(value >> 16);
                if (count > 3) result[byteIdx++] = (byte)(value >> 8);
            }

            return result;
        }


        // Helper methods for length calculation 

        private static int GetEncodedLength(byte[] bytes)
        {
            int len = 0;
            int i = 0;
            while (i <= bytes.Length - 4)
            {
                // Check for a zero group for a shortened notation
                if (bytes[i] == 0 && bytes[i + 1] == 0 && bytes[i + 2] == 0 && bytes[i + 3] == 0)
                    len += 1;   // 'z'
                else
                    len += 5;
                i += 4;
            }
            int rem = bytes.Length - i;
            if (rem > 0)
                len += rem + 1; // partial group
            return len;
        }

        private static int GetDecodedLength(string encoded)
        {
            int count = 0;
            int bytes = 0;
            foreach (char ch in encoded)
            {
                if (ch == 'z' && count == 0)
                {
                    bytes += 4;
                }
                else if (ch >= c_firstCharacter && ch <= c_lastCharacter)
                {
                    count++;
                    if (count == 5)
                    {
                        bytes += 4;
                        count = 0;
                    }
                }
                else
                {
                    throw new FormatException($"Invalid character '{ch}' in Ascii85 block.");
                }
            }
            if (count == 1)
                throw new FormatException("The final Ascii85 block must contain more than one character.");
            if (count > 1)
                bytes += count - 1;
            return bytes;
        }

        // Constants and static data

        const char c_firstCharacter = '!';
        const char c_lastCharacter = 'u';

        static readonly uint[] s_powersOf85 = {
            85u * 85u * 85u * 85u,
            85u * 85u * 85u,
            85u * 85u,
            85u,
            1
        };
    }
}
