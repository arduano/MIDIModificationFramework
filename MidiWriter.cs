using MIDIModificationFramework.MIDI_Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class MidiWriter
    {
        Stream writer;

        long chunkStart = 0;

        public MidiWriter(Stream writer)
        {
            this.writer = writer;
        }

        public void Write(string text)
        {
            for (int i = 0; i < text.Length; i++) writer.WriteByte((byte)text[i]);
        }

        public void Write(MIDIEvent e)
        {
            var data = e.GetDataWithDelta();
            writer.Write(data, 0, data.Length);
        }

        public void Write(byte[] data)
        {
            writer.Write(data, 0, data.Length);
        }

        public void Write(ushort v)
        {
            for (int i = 1; i >= 0; i--) writer.WriteByte((byte)((v >> (i * 8)) & 0xFF));
        }

        public void Write(uint v)
        {
            for (int i = 3; i >= 0; i--) writer.WriteByte((byte)((v >> (i * 8)) & 0xFF));
        }

        public void Write(byte v)
        {
            writer.WriteByte(v);
        }

        public void WriteVariableLen(int i)
        {
            var b = new byte[5];
            int len = 0;
            while (true)
            {
                byte v = (byte)(i & 0x7F);
                i = i >> 7;
                if (i != 0)
                {
                    v = (byte)(v | 0x80);
                    b[len++] = v;
                }
                else
                {
                    b[len++] = v;
                    break;
                }
            }
            Write(b.Take(len).ToArray());
        }

        public void WriteFormat(ushort s)
        {
            long pos = writer.Position;
            writer.Position = 8;
            Write((ushort)s);
            writer.Position = pos;
        }

        public void WriteNtrks(ushort s)
        {
            long pos = writer.Position;
            writer.Position = 10;
            Write((ushort)s);
            writer.Position = pos;
        }

        public void WritePPQ(ushort s)
        {
            long pos = writer.Position;
            writer.Position = 12;
            Write((ushort)s);
            writer.Position = pos;
        }

        public void Init()
        {
            writer.Position = 0;
            Write("MThd");
            Write((uint)6);
            WriteFormat(1);
            WriteNtrks(0);
            WritePPQ(96);
        }

        public void InitTrack()
        {
            chunkStart = writer.Length;
            writer.Position = chunkStart;
            Write("MTrk");
            Write((uint)0);
        }

        public void EndTrack()
        {
            uint len = (uint)(writer.Position - chunkStart) - 8;
            writer.Position = chunkStart + 4;
            Write(len);
            writer.Position = writer.Length;
        }

        public void Close()
        {
            writer.Flush();
            writer.Close();
        }
    }
}
