using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class ChannelModeMessageEvent : MIDIEvent
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
        public byte C { get; set; }
        public byte V { get; set; }

        public ChannelModeMessageEvent(uint delta, byte channel, byte cc, byte vv) : base(delta)
        {
            Channel = channel;
            C = cc;
            V = vv;
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b10110000 | Channel),
                C,
                V
            };
        }
    }
}
