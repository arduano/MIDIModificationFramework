using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class PitchWheelChangeEvent : MIDIEvent
    {
        byte channel;
        public byte Channel
        {
            get { return channel; }
            set
            {
                channel = (byte)(value & 0x0F);
            }
        }

        short value;
        public short Value
        {
            get => value;
            set
            {
                if (value > 0x1FFF) this.value = 0x1FFF;
                else if (value < -0x2000) this.value = -0x2000;
                else this.value = value;
            }
        }

        public PitchWheelChangeEvent(uint delta, byte channel, short value) : base(delta)
        {
            Channel = channel;
            Value = value;
        }

        public override byte[] GetData()
        {
            int val = value + 0x2000;
            return new byte[]
            {
                (byte)(0b11100000 | Channel),
                (byte)(val & 0x7F),
                (byte)((val >> 7) & 0x7F)
            };
        }
    }
}
