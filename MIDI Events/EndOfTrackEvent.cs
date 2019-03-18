using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class EndOfTrackEvent : MIDIEvent
    {
        public EndOfTrackEvent(uint delta) : base(delta) { }
        public EndOfTrackEvent() : base(0) { }

        public override byte[] GetData()
        {
            return new byte[] { 0xFF, 0x2F, 0x00 };
        }
    }
}
