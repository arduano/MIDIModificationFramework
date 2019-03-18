using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class NoteOnEvent : MIDIEvent
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

        public NoteOnEvent(uint delta, byte channel, byte note, byte velocity) : base(delta)
        {
            Channel = channel;
            Note = note;
            Velocity = velocity;
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b10010000 | Channel),
                Note,
                Velocity
            };
        }
    }
}
