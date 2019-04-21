using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class ProgramChangeEvent : MIDIEvent
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
        public byte Program { get; set; }

        public ProgramChangeEvent(uint delta, byte channel, byte program) : base(delta)
        {
            Channel = channel;
            Program = program;
        }

        public override MIDIEvent Clone()
        {
            return new ProgramChangeEvent(DeltaTime, Channel, Program);
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b11000000 | Channel),
                Program
            };
        }
    }
}
