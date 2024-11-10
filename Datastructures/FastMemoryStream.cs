using System;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vintagestory.API.Datastructures;

public class FastMemoryStream : Stream
{
    byte[] buffer;
    int bufferlength;
    const int MaxLength = int.MaxValue - 3;  // DWord boundary

    public FastMemoryStream()
    {
        bufferlength = 1024;
        buffer = new byte[1024];
        Position = 0;
    }

    public FastMemoryStream(byte[] buffer, int length)
    {
        bufferlength = length;
        this.buffer = buffer;
        Position = 0;
    }

    /// <summary>
    /// When serializing to a buffer, indicates the count of bytes written so far
    /// </summary>
    public override long Position { get; set; }

    /// <summary>
    /// When deserializing from a buffer, this is the full buffer length
    /// </summary>
    public override long Length => bufferlength;

    public override void SetLength(long value)
    {
        if (value > MaxLength) throw new IndexOutOfRangeException("FastMemoryStream limited to 2GB in size");

        bufferlength = Math.Min((int)value, buffer.Length);
    }

    public byte[] ToArray()
    {
        return FastCopy(buffer, bufferlength, (int)Position);
    }

    public byte[] GetBuffer()
    {
        return buffer;
    }

    public override int Read(byte[] destBuffer, int offset, int count)
    {
        // Local objects for quicker reference inside the potentially long loop
        long origPosition = Position;
        long bufferlength = this.bufferlength;
        byte[] streamBuffer = this.buffer;

        for (int i = 0; i < count; i++)
        {
            if (origPosition + i >= bufferlength)
            {
                Position += i;
                return i;
            }
            destBuffer[offset + i] = streamBuffer[origPosition + i];
        }
        Position += count;
        return count;
    }

    public override bool CanSeek => false;

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override void Write(byte[] srcBuffer, int srcOffset, int count)
    {
        if (count <= 0) return;

        CheckCapacity(count);


        // Common small cases, very simple approach
        if (count < 256)
        {
            var buffer = this.buffer;                 // local copy of the buffer reference for performance
            uint pos = (uint)this.Position;           // local copy of this for performance: avoids field lookups and we want an (uint) anyhow, as Array.Copy on a (long) offset is no better (would throw exceptions)
            uint i = (uint)srcOffset;
            uint srcLimit = (uint)(srcOffset + count);
            while (i < srcLimit)
            {
                buffer[pos++] = srcBuffer[i++];       // using uint to index into the arrays should at least tell the runtime not to do lower bounds checks as a uint cannot be less than zero
            }
        }


        // Mid and large cases, we roll our own version of Array.Copy, with less overhead than Array.Copy itself
        else
        {
            // Not worth building spans for the common small cases, but definitely worth it for medium-large counts
            ReadOnlySpan<byte> inputBuffer = new ReadOnlySpan<byte>(srcBuffer, srcOffset, count);
            Span<byte> streamBuffer = new Span<byte>(this.buffer, (int)this.Position, count);
            inputBuffer.CopyTo(streamBuffer);   // Uses internal Buffer.MemMove which is apparently the most efficient available (Array.Copy uses the same internally)

                    // Essentially equivalent to:  Array.Copy(srcBuffer, srcOffset, this.buffer, (int)this.Position, count);   // and note that Array.Copy with a (long) dest offset is no better, just throws exceptions if the offset exceeds int.MaxValue
        }


        Position += count;
    }

    private void CheckCapacity(int count)
    {
        if (Position + count > bufferlength)
        {
            if (Position + count > MaxLength) throw new IndexOutOfRangeException("FastMemoryStream limited to 2GB in size");

            int newSize = (int)Math.Min(Math.Max((long)bufferlength * 2, Position + count), MaxLength);
            this.buffer = FastCopy(this.buffer, (int)Position, newSize);
            bufferlength = newSize;
        }
    }

    public override int ReadByte()
    {
        if (Position >= bufferlength)
        {
            return -1;
        }
        return buffer[Position++];
    }

    public override void WriteByte(byte p)
    {
        CheckCapacity(1);
        buffer[Position++] = p;
    }

    private static byte[] FastCopy(byte[] buffer, int oldLength, int newSize)
    {
        if (newSize < oldLength) oldLength = newSize;    // If this condition is met, we are making a partial copy of the old buffer, up to newSize only - for example on ToArray() method

        byte[] bufferCopy = new byte[newSize];
        if (oldLength > 256)
        {
            // Equivalent to Array.Copy(buffer, 0, buffer2, 0, oldLength);

            ReadOnlySpan<byte> oldBuffer = new ReadOnlySpan<byte>(buffer, 0, oldLength);
            Span<byte> newBuffer = new Span<byte>(bufferCopy, 0, oldLength);
            oldBuffer.CopyTo(newBuffer);
        }
        else
        {
            uint i = 0;
            if (oldLength > 15)    // 15 is arbitrary but we don't need this faff for very small arrays, and (uint)srcLimit will be calculated wrong if oldLength is less than 3
            {
                uint srcLimit = (uint)(oldLength - 3);
                for (; i < buffer.Length; i += 4)
                {
                    if (i >= srcLimit) break;
                    bufferCopy[i] = buffer[i];
                    bufferCopy[i + 1] = buffer[i + 1];
                    bufferCopy[i + 2] = buffer[i + 2];
                    bufferCopy[i + 3] = buffer[i + 3];
                }
            }
            for (; i < oldLength; i++)
            {
                bufferCopy[i] = buffer[i];
            }
        }

        return bufferCopy;
    }

    public override void Flush()
    {
        
    }

    public void Reset()
    {
        Position = 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return -1;
    }

}
