using System;
using System.IO;

namespace Vintagestory.API.Datastructures;

public class FastMemoryStream : Stream
{
    byte[] buffer;
    long bufferlength;

    public FastMemoryStream()
    {
        bufferlength = 16;
        buffer = new byte[16];
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
        bufferlength = Math.Min(value, buffer.Length);
    }

    public byte[] ToArray()
    {
        return FastCopy(buffer, bufferlength, Position);
    }

    public byte[] GetBuffer()
    {
        return buffer;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Position + i >= this.bufferlength)
            {
                Position += i;
                return i;
            }
            buffer[offset + i] = this.buffer[Position + i];
        }
        Position += count;
        return count;
    }

    public override bool CanSeek => false;

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count <= 0) return;
        if (Position + count - 1 >= bufferlength)
        {
            long newSize = bufferlength * 2;
            if (newSize < Position + count) newSize = Position + count;
            this.buffer = FastCopy(this.buffer, Position, newSize);
            bufferlength = newSize;
        }

        if (count > 200)
        {
            Array.Copy(buffer, offset, this.buffer, Position, count);
            Position += count;
        }
        else
        {
            int i = offset;
            count += offset;
            int dWordBoundary = i + 3 - (i + 3) % 4;
            while (i < dWordBoundary)
            {
                this.buffer[Position++] = buffer[i++];
            }
            int fastLoopLimit = count - count % 4;
            for (; i < fastLoopLimit; i += 4)
            {
                this.buffer[Position] = buffer[i];
                this.buffer[Position + 1] = buffer[i + 1];
                this.buffer[Position + 2] = buffer[i + 2];
                this.buffer[Position + 3] = buffer[i + 3];
                Position += 4;
            }
            while (i < count)
            {
                this.buffer[Position++] = buffer[i++];
            }
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
        if (Position >= bufferlength)
        {
            buffer = FastCopy(buffer, Position, bufferlength *= 2);
        }
        buffer[Position++] = p;
    }

    private static byte[] FastCopy(byte[] buffer, long oldLength, long newSize)
    {
        byte[] buffer2 = new byte[newSize];
        Array.Copy(buffer, 0, buffer2, 0, Math.Min(oldLength, newSize));        
        return buffer2;
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
