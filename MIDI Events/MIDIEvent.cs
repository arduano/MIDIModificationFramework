using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public abstract class MIDIEvent
    {
        public long DeltaTime { get; set; }

        public MIDIEvent(long delta)
        {
            DeltaTime = delta;
        }
        public abstract byte[] GetData();

        public byte[] MakeVariableLen(int i)
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
            return b.Take(len).ToArray();
        }

        public byte[] GetDataWithDelta()
        {
            return MakeVariableLen((int)DeltaTime).Concat(GetData()).ToArray();
        }
    }
}
