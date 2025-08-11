using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Util
{
    /// <summary>
    /// A faster implementation of a ProtoBuf serializer.
    /// <br/>Note 1: for int and ushort and other primitives/structs, the default value of 0 will not be written (because the read value will be 0 anyhow, so not writing the 0 value makes no difference to what the reader eventually reads): except in arrays and other simple collections such as List, where 0 values will be written, so that the reader reads out all subsequent values in the array with the correct indexing
    /// <br/>Note 2: for reference types including string, if the value is null then nothing will be written.  A read collection may therefore be shorter than the written collection, if the written collection included null values: be careful serializing arrays of reference types, if necessary replace null values with something else prior to serialization.  For convenience, one special exception here is arrays or lists of string: if a null string is in the array, FastSerializer will instead automatically write an empty string "". (We don't do that for other reference types because it is not necessarily clear what the default non-null value should be....)
    /// </summary>
    public static class FastSerializer
    {
        #region single object methods

        public static void Write(FastMemoryStream stream, int field, byte[] val)
        {
            if (val == null) return;
            WriteTagLengthDelim(stream, field, val.Length);
            stream.Write(val, 0, val.Length);
        }

        public static void Write(FastMemoryStream stream, int field, FastMemoryStream val)
        {
            WriteTagLengthDelim(stream, field, (int)val.Position);
            stream.Write(val);
        }

        public static void Write(FastMemoryStream stream, int field, int val)
        {
            if (val == 0) return;
            WriteTagVarInt(stream, field);
            Write(stream, val);
        }

        public static void Write(FastMemoryStream stream, int field, float val)
        {
            if (val == 0f) return;
            WriteTagFixed32(stream, field);
            stream.Write(val);
        }

        public static void Write(FastMemoryStream stream, int field, bool val)
        {
            if (val)
            {
                WriteTagVarInt(stream, field);
                Write(stream, (byte)1);
            }
        }

        public static void Write(FastMemoryStream stream, int field, string s)
        {
            if (s == null) return;
            int byteCount = Encoding.UTF8.GetByteCount(s);
            WriteTagLengthDelim(stream, field, byteCount);
            stream.WriteUTF8String(s, byteCount);
        }

        public static void Write(FastMemoryStream stream, int field, BlockPos val)
        {
            if (val == null) return;
            int x = val.X;
            int y = val.InternalY;
            int z = val.Z;
            WriteTagLengthDelim(stream, field, 3 + GetSize(x) + GetSize(y) + GetSize(z));
            Write(stream, 1, x);
            Write(stream, 2, y);
            Write(stream, 3, z);
        }

        public static void Write(FastMemoryStream stream, int field, Vec2i val)
        {
            if (val == null) return;
            int x = val.X;
            int y = val.Y;
            if (x == 0 && y == 0)
            {
                // Don't write nothing at all if the Vec2i has value (0,0) as otherwise we might have no key for a Dictionary: so here we explicitly write the zero value of x
                WriteTagLengthDelim(stream, field, 3);
                WriteTagVarInt(stream, 1);
                Write(stream, 0);
                return;
            }
            WriteTagLengthDelim(stream, field, 2 + GetSize(x) + GetSize(y));
            Write(stream, 1, x);
            Write(stream, 2, y);
        }

        public static void Write(FastMemoryStream stream, int field, Vec4i val)
        {
            if (val == null) return;
            int x = val.X;
            int y = val.Y;
            int z = val.Z;
            int w = val.W;
            WriteTagLengthDelim(stream, field, 4 + GetSize(x) + GetSize(y) + GetSize(z) + GetSize(w));
            Write(stream, 1, x);
            Write(stream, 2, y);
            Write(stream, 3, z);
            Write(stream, 4, w);
        }

        #endregion

        #region Collections

        public static void Write(FastMemoryStream stream, int field, IEnumerable<byte[]> collection)
        {
            if (collection != null)
            {
                foreach (byte[] val in collection)
                {
                    Write(stream, field, val ?? Array.Empty<byte>());
                }
            }
        }

        public static void Write(FastMemoryStream stream, int field, IEnumerable<int> collection)
        {
            if (collection != null)
            {
                // For a Collection we must even write 0 values, to fill out the array/List/Set: so in this collection code we bypass Write(stream, field, int)
                
                int fieldtag = field * 8;
                if (fieldtag < 0x80)
                {
                    foreach (int val in collection)
                    {
                        stream.WriteByte((byte)fieldtag);
                        Write(stream, val);
                    }
                }
                else
                {
                    foreach (int val in collection)
                    {
                        stream.WriteTwoBytes(fieldtag);
                        Write(stream, val);
                    }
                }
            }
        }

        public static void Write(FastMemoryStream stream, int field, IEnumerable<ushort> collection)
        {
            if (collection != null)
            {
                // For a Collection we must even write 0 values, to fill out the array/List/Set: so in this collection code we bypass Write(stream, field, ushort)

                int fieldtag = field * 8;
                if (fieldtag < 0x80)
                {
                    foreach (ushort val in collection)
                    {
                        stream.WriteByte((byte)fieldtag);
                        Write(stream, val);
                    }
                }
                else
                {
                    foreach (ushort val in collection)
                    {
                        stream.WriteTwoBytes(fieldtag);
                        Write(stream, val);
                    }
                }
            }
        }

        public static void Write(FastMemoryStream stream, int field, IEnumerable<string> collection)
        {
            if (collection != null)
            {
                foreach (string val in collection) Write(stream, field, val ?? "");
            }
        }

        public static void Write(FastMemoryStream stream, int field, IEnumerable<BlockPos> collection)
        {
            if (collection != null)
            {
                foreach (BlockPos val in collection) Write(stream, field, val);
            }
        }

        public static void Write(FastMemoryStream stream, int field, IEnumerable<Vec4i> collection)
        {
            if (collection != null)
            {
                foreach (Vec4i val in collection) Write(stream, field, val);
            }
        }

        public static void Write(FastMemoryStream stream, int field, IDictionary<string, byte[]> dict)
        {
            if (dict != null)
            {
                foreach (var entry in dict)
                {
                    WriteTagLengthDelim(stream, field, 2 + GetSize(entry.Key) + GetSize(entry.Value));
                    Write(stream, 1, entry.Key);
                    Write(stream, 2, entry.Value);
                }
            }
        }
        
        public static void Write(FastMemoryStream stream, int field, IDictionary<Vec2i, float> dict)
        {
            if (dict != null)
            {
                foreach (var entry in dict)
                {
                    // Note: if the float Entry.Value has the default value of 0f, then the following code will not write the .Value, for better performance - the read Entry.Value will have the default value anyhow
                    WriteTagLengthDelim(stream, field, 2 + GetSize(entry.Key) + GetSize(entry.Value));
                    Write(stream, 1, entry.Key);
                    Write(stream, 2, entry.Value);
                }
            }
        }

        public static void WritePacked(FastMemoryStream stream, int field, IEnumerable<ushort> collection)
        {
            if (collection != null)
            {
                int size = 0;
                foreach (ushort val in collection) size += GetSize(val);
                WriteTagLengthDelim(stream, field, size);
                foreach (ushort val in collection) Write(stream, val);
            }
        }

        public static void WritePacked(FastMemoryStream stream, int field, IEnumerable<int> collection)
        {
            if (collection != null)
            {
                int size = 0;
                foreach (int val in collection) size += GetSize(val);
                WriteTagLengthDelim(stream, field, size);
                foreach (int val in collection) Write(stream, val);
            }
        }

        #endregion

        #region sizing

        public static int GetSize(ushort v)
        {
            if (v < 0x80) return 1;
            if (v < 0x4000) return 2;
            return 3;
        }

        public static int GetSize(int v)
        {
            if (v < 0x80) return v > 0 ? 1 : (v == 0 ? -1 : 5);   // If an int has the default value of 0, it won't be written so return -1
            if (v < 0x4000) return 2;
            if (v < 0x20_0000) return 3;
            if (v < 0x1000_0000) return 4;
            return 5;
        }

        public static int GetSize(float v)
        {
            return v == 0f ? -1 : 4;    // If 0f in an IDictionary value, no need for Write() to write it as this is the default value of float, so return -1 as the tag byte also will not be written
        }

        public static int GetSize(byte[] val)
        {
            if (val == null) return -1;    // If null, it can't be written by Write(), so return -1 as the tag byte also will not be written
            return val.Length + GetSize(val.Length);
        }

        public static int GetSize(string s)
        {
            if (s == null) return -1;    // If null, it can't be written by Write(), so return -1 as the tag byte also will not be written
            int byteCount = Encoding.UTF8.GetByteCount(s);
            return byteCount + GetSize(byteCount);
        }

        public static int GetSize(Vec2i val)
        {
            if (val == null) return -1;    // If null, it can't be written by Write(), so return -1 as the tag byte also will not be written
            int byteCount = 2 + GetSize(val.X) + GetSize(val.Y);
            return byteCount + GetSize(byteCount);
        }

        #endregion

        #region primitives

        private static void Write(FastMemoryStream stream, byte val)
        {
            stream.WriteByte(val);
        }

        private static void Write(FastMemoryStream stream, ushort val)
        {
            if (val < 0x80) 
                stream.WriteByte((byte)val);
            else if (val < 0x4000)
                stream.WriteTwoBytes(val);
            else stream.WriteThreeBytes(val);
        }

        private static void Write(FastMemoryStream stream, int val)
        {
            if (val >= 0)
            {
                if (val < 0x80)
                {
                    stream.WriteByte((byte)val);
                    return;
                }
                else if (val < 0x4000)
                {
                    stream.WriteTwoBytes(val);
                    return;
                }
                else if (val < 0x20_0000)
                {
                    stream.WriteThreeBytes(val);
                    return;
                }
            }

            // Requires 4 bytes or more (5 bytes for a negative signed int32). We can do this in two stages:
            stream.WriteThreeBytes(val & 0x1F_FFFF | 0x20_0000);
            val = val >>> 21;    // Logical right-shift operator, sets the sign bit and other left-most bits to 0, so val now cannot be negative, maximum value of val here is 0x7FF
            if (val < 0x80) stream.WriteByte((byte)val);
            else stream.WriteTwoBytes(val);
        }

        /// <summary>
        /// For long values
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="val"></param>
        private static void Write(FastMemoryStream stream, long val)
        {
            if (val >= 0)
            {
                if (val < 0x80)
                {
                    stream.WriteByte((byte)val);
                    return;
                }
                else if (val < 0x4000)
                {
                    stream.WriteTwoBytes((int)val);
                    return;
                }
                else if (val < 0x20_0000)
                {
                    stream.WriteThreeBytes((int)val);
                    return;
                }
            }

            // Requires more than 3 bytes (up to 9 bytes for a negative signed int64). We can do this in two stages:
            stream.WriteThreeBytes((int)val & 0x1F_FFFF | 0x20_0000);
            Write(stream, val >>> 21);  // Logical right-shift operator, sets the sign bit and other left-most bits to 0, so val now cannot be negative, but may still be long
        }

        #endregion

        #region Tags

        private static void WriteTagVarInt(FastMemoryStream stream, int field)
        {
            if (field < 0x10)
            {
                stream.WriteByte((byte)(field * 8));
            }
            else
            {
                stream.WriteTwoBytes(field * 8);
            }
        }

        private static void WriteTagFixed32(FastMemoryStream stream, int field)
        {
            if (field < 0x10)
            {
                stream.WriteByte((byte)(field * 8 + 5));
            }
            else
            {
                stream.WriteTwoBytes(field * 8 + 5);
            }
        }

        public static void WriteTagLengthDelim(FastMemoryStream stream, int field, int length)
        {
            if (field < 0x10)
            {
                stream.WriteByte((byte)(field * 8 + 2));
            }
            else
            {
                stream.WriteTwoBytes(field * 8 + 2);
            }

            if (length < 0x80)
            {
                stream.WriteByte((byte)length);
            }
            else if (length < 0x4000)
            {
                stream.WriteTwoBytes(length);
            }
            else if (length < 0x20_0000)
            {
                stream.WriteThreeBytes(length);
            }
            else Write(stream, length);
        }

        #endregion
    }



    /// <summary>
    /// Detected by the VintagestorySourcegen source generator, which replaces this default FastSerialize method with a source-generated version
    /// </summary>
    public interface IWithFastSerialize
    {
        public byte[] FastSerialize(FastMemoryStream ms)
        {
            // At runtime this will always be overridden by implementing classes and their FastSerialize() method
            throw new NotImplementedException("Probably, VintagestorySourcegen source generator did not succeed. Possible causes include not having Microsoft.CodeAnalysis.CSharp version 4.12.0 installed, discuss with th3dilli or radfast.");
        }
    }


    /// <summary>
    /// Indicates that the FastSerialize() method should call a custom method to serialize this field: the custom method being of the pattern FastSerializerDelegate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CustomFastSerializerAttribute : Attribute
    {
        public CustomFastSerializerAttribute()
        {
        }
    }


    public delegate void FastSerializerDelegate(FastMemoryStream ms, int id, ref int count, ref int position);
}
