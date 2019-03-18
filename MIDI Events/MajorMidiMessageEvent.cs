using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public class MajorMidiMessageEvent : MIDIEvent
    {
        byte[] data;

        public MajorMidiMessageEvent(uint delta, byte command) : base(delta)
        {
            data = new byte[] { command };
        }

        public MajorMidiMessageEvent(uint delta, byte command, byte var1) : base(delta)
        {
            data = new byte[] { command , var1 };
        }

        public MajorMidiMessageEvent(uint delta, byte command, byte var1, byte var2) : base(delta)
        {
            data = new byte[] { command, var1, var2 };
        }

        public override byte[] GetData()
        {
            return data;
        }
    }
}
