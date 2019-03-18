using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public abstract class MIDIEvent
    {
        uint deltatime;
        public uint DeltaTime
        {
            get => deltatime;
            set
            {
                if (value >= 2147483648) throw new ArgumentException("Delta time is too big. Must be less than 2^31", "delta");
                deltatime = value;
            }
        }

        public MIDIEvent(uint delta)
        {
            DeltaTime = delta;
        }
        public abstract byte[] GetData();

        public byte[] MakeVariableLen(int i)
        {
            var b = new byte[5];
            int len = 4;
            byte added = 0x00;
            while (true)
            {
                byte v = (byte)(i & 0x7F);
                i = i >> 7;
                v = (byte)(v | added);
                b[len--] = v;
                added = 0x80;
                if (i == 0)
                {
                    break;
                }
            }
            return b.Skip(len + 1).ToArray();
        }

        public byte[] GetDataWithDelta()
        {
            return MakeVariableLen((int)DeltaTime).Concat(GetData()).ToArray();
        }
    }
}
