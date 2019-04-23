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
        public byte Key { get; set; }
        public byte Velocity { get; set; }

        public NoteOffEvent(uint delta, byte channel, byte key) : base(delta)
        {
            Channel = channel;
            Key = key;
        }

        public override MIDIEvent Clone()
        {
            return new NoteOffEvent(DeltaTime, Channel, Key);
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b10000000 | Channel),
                Key,
                0
            };
        }
    }
}
