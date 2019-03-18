using MIDIModificationFramework.MIDI_Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class MidiTrack
    {
        public LinkedList<MIDIEvent> Events { get; } = new LinkedList<MIDIEvent>();

        public MidiTrack()
        {

        }
    }
}
