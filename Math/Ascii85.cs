using System;
using System.Text;
using System.IO;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Converts between binary data and an Ascii85-encoded string.
    /// </summary>
    /// <remarks>See <a href="http://en.wikipedia.org/wiki/Ascii85">Ascii85 at Wikipedia</a>.</remarks>
    public static class Ascii85
    {
        /// <summary>
        /// Encodes the specified byte array in Ascii85.
        /// </summary>
        /// <param name="bytes">The bytes to encode.</param>
        /// <returns>An Ascii85-encoded string representing the input byte array.</returns>
        public static string Encode(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            int exactLength = GetEncodedLength(bytes);
            return string.Create(exactLength, bytes, static (chars, state) =>
            {
                int pos = 0;
                int count = 0;
                uint value = 0;

                for (int i = 0; i < state.Length; i++)
                {
                    value |= ((uint)state[i]) << (24 - (count * 8));
                    count++;

                    if (count == 4)
                    {
                        if (value == 0)
                        {
                            chars[pos++] = 'z';
                        }
                        else
                        {
                            EncodeValueInline(chars, ref pos, value, 0);
                        }
                        count = 0;
                        value = 0;
                    }
                }

                if (count > 0)
                    EncodeValueInline(chars, ref pos, value, 4 - count);
            });
        }

        /// <summary>
        /// Decodes the specified Ascii85 string into the corresponding byte array.
        /// </summary>
        /// <param name="encoded">The Ascii85 string.</param>
        /// <returns>The decoded byte array.</returns>
        public static byte[] Decode(string encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException("encoded");

            int decodedLength = GetDecodedLength(encoded);
            byte[] result = new byte[decodedLength];
            int writePos = 0;

            int count = 0;
            uint value = 0;

            for (int i = 0; i < encoded.Length; i++)
            {
                char ch = encoded[i];
                if (ch == 'z' && count == 0)
                {
                    result[writePos++] = 0;
                    result[writePos++] = 0;
                    result[writePos++] = 0;
                    result[writePos++] = 0;
                }
                else if (ch < c_firstCharacter || ch > c_lastCharacter)
                {
                    throw new FormatException("Invalid character '" + ch + "' in Ascii85 block.");
                }
                else
                {
                    try
                    {
                        checked { value += (uint)(s_powersOf85[count] * (ch - c_firstCharacter)); }
                    }
                    catch (OverflowException ex)
                    {
                        throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
                    }

                    count++;

                    if (count == 5)
                    {
                        result[writePos++] = (byte)(value >> 24);
                        result[writePos++] = (byte)((value >> 16) & 0xFF);
                        result[writePos++] = (byte)((value >> 8) & 0xFF);
                        result[writePos++] = (byte)(value & 0xFF);
                        count = 0;
                        value = 0;
                    }
                }
            }

            if (count == 1)
            {
                throw new FormatException("The final Ascii85 block must contain more than one character.");
            }
            else if (count > 1)
            {
                for (int padding = count; padding < 5; padding++)
                {
                    try
                    {
                        checked { value += 84 * s_powersOf85[padding]; }
                    }
                    catch (OverflowException ex)
                    {
                        throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
                    }
                }
                int bytesToWrite = count - 1;
                if (bytesToWrite >= 1) result[writePos++] = (byte)(value >> 24);
                if (bytesToWrite >= 2) result[writePos++] = (byte)((value >> 16) & 0xFF);
                if (bytesToWrite >= 3) result[writePos++] = (byte)((value >> 8) & 0xFF);
            }

            return result;
        }

        private static void EncodeValueInline(Span<char> chars, ref int pos, uint value, int paddingBytes)
        {
            Span<char> encoded = stackalloc char[5];
            for (int index = 4; index >= 0; index--)
            {
                encoded[index] = (char)((value % 85) + c_firstCharacter);
                value /= 85;
            }

            int charsToWrite = 5 - paddingBytes;
            for (int i = 0; i < charsToWrite; i++)
            {
                chars[pos++] = encoded[i];
            }
        }

        private static int GetEncodedLength(byte[] bytes)
        {
            int remainder = bytes.Length % 4;

            int zCount = 0;
            int normalGroups = 0;
            int count = 0;
            uint value = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                value |= ((uint)bytes[i]) << (24 - (count * 8));
                count++;
                if (count == 4)
                {
                    if (value == 0) zCount++;
                    else normalGroups++;
                    count = 0;
                    value = 0;
                }
            }

            int length = zCount + normalGroups * 5;
            if (remainder > 0) length += remainder + 1;
            return length;
        }

        private static int GetDecodedLength(string encoded)
        {
            int length = 0;
            int count = 0;
            for (int i = 0; i < encoded.Length; i++)
            {
                char ch = encoded[i];
                if (ch == 'z' && count == 0)
                {
                    length += 4;
                }
                else
                {
                    count++;
                    if (count == 5)
                    {
                        length += 4;
                        count = 0;
                    }
                }
            }
            if (count > 1) length += count - 1;
            return length;
        }

        // the first and last characters used in the Ascii85 encoding character set
        const char c_firstCharacter = '!';
        const char c_lastCharacter = 'u';

        static readonly uint[] s_powersOf85 = new uint[] { 85u * 85u * 85u * 85u, 85u * 85u * 85u, 85u * 85u, 85u, 1 };
    }
}
