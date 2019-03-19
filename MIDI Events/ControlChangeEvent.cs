using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class ControlChangeEvent : MIDIEvent
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
        public byte Controller { get; set; }
        public byte Value { get; set; }

        public ControlChangeEvent(uint delta, byte channel, byte controller, byte value) : base(delta)
        {
            Channel = channel;
            Controller = controller;
            Value = value;
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b10110000 | Channel),
                Controller,
                Value
            };
        }
    }
}
