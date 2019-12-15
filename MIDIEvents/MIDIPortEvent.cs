using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDIEvents
{
    public class MIDIPortEvent : MIDIEvent
    {
        public byte Channel { get; set; }
        public MIDIPortEvent(double delta, byte channel) : base(delta)
        {
            Channel = channel;
        }

        public override MIDIEvent Clone()
        {
            return new MIDIPortEvent(DeltaTime, Channel);
        }

        public override byte[] GetData()
        {
            return new byte[] { 0xFF, 0x20, 0x01, Channel };
        }
    }
}
