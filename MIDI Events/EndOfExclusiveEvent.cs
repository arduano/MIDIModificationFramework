using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class EndOfExclusiveEvent : MIDIEvent
    {
        public EndOfExclusiveEvent(uint delta) : base(delta) { }

        public override byte[] GetData()
        {
            return new byte[] { 0b11110111 };
        }
    }
}
