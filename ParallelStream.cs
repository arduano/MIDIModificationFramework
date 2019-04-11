using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class ParallelStreamIO : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        long length = 0;
        public override long Length => length;

        public bool Closed = false;

        long position = 0;
        public override long Position
        {
            get => position;
            set
            {
                int offset = (int)(value % pstream.ChunkDataSize);
                currentChunk = (value - offset) / pstream.ChunkDataSize;
                if (currentChunk >= locations.Count) throw new ArgumentOutOfRangeException("Position", "Position was out of range");
                currentChunkSize = ReadChunkSize(locations[(int)currentChunk]);
                currentChunkAvailableRead = currentChunkSize - offset;
                if (currentChunkAvailableRead < 0) throw new ArgumentOutOfRangeException("Position", "Position was out of range");
                currentChunkAvailableWrite = pstream.ChunkDataSize - offset;
            }
        }

        List<long> locations = new List<long>();

        long currentChunk = -1;
        int currentChunkSize = -1;
        int currentChunkAvailableRead = -1;
        int currentChunkAvailableWrite = -1;

        Func<long> GetNewChunk;

        Stream stream;
        ParallelStream pstream;

        public ParallelStreamIO(long length, List<long> locations, Func<long> getNewChunk, Stream stream, ParallelStream pstream)
        {
            this.length = length;
            this.locations = locations;
            GetNewChunk = getNewChunk;
            this.stream = stream;
            this.pstream = pstream;
            Position = 0;
        }

        public override void Flush()
        {

        }

        void WriteChunkSize(long chunk, int length)
        {
            stream.Position = pstream.HeaderChunkSize + pstream.ChunkSize * chunk;
            byte[] ib = new byte[4];
            int a = 0;
            for (int i = 3; i >= 0; i--) ib[a++] = (byte)((length >> (i * 8)) & 0xFF);
            stream.Write(ib, 0, 4);
        }

        int ReadChunkSize(long chunk)
        {
            stream.Position = pstream.HeaderChunkSize + pstream.ChunkSize * chunk;
            int v = 0;
            for (int j = 0; j < 4; j++) v = v << 8 + stream.ReadByte();
            return v;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;
            if (currentChunkAvailableRead == 0) return 0;
            lock (stream)
            {
                while (read != count)
                {
                    if (currentChunkAvailableRead <= count - read)
                    {
                        stream.Read(buffer, offset + read, currentChunkAvailableRead);
                        read += currentChunkAvailableRead;
                        try
                        {
                            Position += currentChunkAvailableRead;
                        }
                        catch
                        {
                            currentChunkAvailableRead = 0;
                            break;
                        }
                    }
                    else
                    {
                        stream.Read(buffer, offset + read, count - read);
                        position += count - read;
                        currentChunkAvailableRead -= count - read;
                    }
                }
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin) Position = offset;
            if (origin == SeekOrigin.Current) Position += offset;
            if (origin == SeekOrigin.End) Position = length - offset;
            return position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int written = 0;
            lock (stream)
            {
                while (written != count)
                {
                    int w = 0;
                    if (currentChunkAvailableWrite <= count - written)
                    {
                        stream.Write(buffer, offset + written, currentChunkAvailableWrite);
                        written += currentChunkAvailableWrite;
                        w = currentChunkAvailableWrite;
                    }
                    else
                    {
                        stream.Write(buffer, offset + written, count - written);
                        w = count - written;
                    }
                    if (w > currentChunkAvailableRead)
                    {
                        currentChunkSize += w - currentChunkAvailableRead;
                        length += w - currentChunkAvailableRead;
                        WriteChunkSize(locations[(int)currentChunk], currentChunkSize);
                        if (w == currentChunkAvailableWrite)
                        {
                            locations.Add(GetNewChunk());
                        }
                        Position += w;
                    }
                    else
                    {
                        position += w;
                        currentChunkAvailableRead -= w;
                        currentChunkAvailableWrite -= w;
                        if (currentChunkAvailableWrite == 0) Position = position;
                    }
                }
            }
        }

        public override void Close()
        {
            lock (stream)
            {
                Closed = true;
            }
        }
    }

    public class ParallelStream : IDisposable
    {
        public int ChunkSize { get; private set; } = 4096 * 4096;
        public int HeaderChunkSize { get; private set; } = 4096 * 4096;
        public int ChunkDataSize => ChunkSize - 4;

        Stream stream;

        long nextOpenChunk = 1;

        int[] headerData;

        public int GetChunkLen(long chunk)
        {
            stream.Position = chunk * ChunkSize;
            int l = 0;
            for (int i = 0; i < 4; i++) l = l << 8 + stream.ReadByte();
            return l;
        }

        public void WriteInt(int v)
        {
            byte[] ib = new byte[4];
            int a = 0;
            for (int i = 3; i >= 0; i--) ib[a++] = (byte)((v >> (i * 8)) & 0xFF);
            stream.Write(ib, 0, 4);
        }

        public void WriteHeaderEntry(long i)
        {
            stream.Position = i * 4;
            WriteInt(headerData[i]);
        }

        public void WriteFullHeaderChunk()
        {
            int l = HeaderChunkSize / 4;
            byte[] raw = new byte[HeaderChunkSize];
            int a = 0;
            for (int i = 0; i < l; i++)
            {
                int v = headerData[l];
                for (int j = 3; j >= 0; j--) raw[a++] = (byte)((v >> (j * 8)) & 0xFF);
            }
            stream.Position = 0;
            stream.Write(raw, 0, HeaderChunkSize);
        }

        int[] ParseHeaderChunk()
        {
            int l = HeaderChunkSize / 4;
            byte[] raw = new byte[HeaderChunkSize];
            int[] data = new int[l];
            stream.Read(raw, 0, HeaderChunkSize);
            unsafe
            {
                for (int i = 0; i < l; i++)
                {
                    int v = 0;
                    for (int j = 0; j < 4; j++) v = v << 8 + raw[l];
                    data[i] = v;
                }
            }
            return data;
        }

        public long GetEmptyChunk(int stream)
        {
            if (nextOpenChunk == -1)
            {
                nextOpenChunk = 0;
                while (headerData[nextOpenChunk] != 0) nextOpenChunk++;
            }
            headerData[nextOpenChunk] = stream;
            WriteHeaderEntry(nextOpenChunk);
            long c = nextOpenChunk;
            while (headerData[nextOpenChunk] != 0) nextOpenChunk++;
            return c;
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
