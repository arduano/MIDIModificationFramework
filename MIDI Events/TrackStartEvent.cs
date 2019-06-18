using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class TrackStartEvent : MIDIEvent
    {
        public TrackStartEvent() : base(0) { }

        public override MIDIEvent Clone()
        {
            return new TrackStartEvent();
        }

        public override byte[] GetData()
        {
            return new byte[] { 0xFF, 0x00, 0x02 };
        }
    }
}
