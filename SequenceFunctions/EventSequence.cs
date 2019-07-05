using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public abstract class EventSequence : Sequence<MIDIEvent>
    {
        public abstract IEnumerable<IEnumerable<MIDIEvent>> SourceSequences { get; }
    }
}
