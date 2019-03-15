using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDI_Events
{
    public interface IMIDIEvent
    {
        long DeltaTime { get; set; }
        byte[] GetData();
    }
}
