using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class NoteOffEvent : MIDIEvent
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
        public byte Note { get; set; }
        public byte Velocity { get; set; }

        public NoteOffEvent(long delta, byte channel, byte note) : base(delta)
        {
            Channel = channel;
            Note = note;
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b10000000 | Channel),
                Note,
                0
            };
        }
    }
}
