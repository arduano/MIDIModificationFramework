using MIDIModificationFramework.MIDI_Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class TrackReader : EventSequence
    {
        class Iterator : IEnumerator<MIDIEvent>
        {
            EventParser reader;

            public MIDIEvent Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                MIDIEvent next = reader.ParseNextEvent();
                if (next == null)
                {
                    return false;
                }
                Current = next;
                return true;
            }

            public void Reset()
            {
                reader.Reset();
            }

            public void Dispose()
            {

            }

            public Iterator(EventParser reader)
            {
                this.reader = reader;
            }
        }

        Func<EventParser> GetParser;

        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => null;

        internal TrackReader(Func<EventParser> parserfunc)
        {
            GetParser = parserfunc;
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(GetParser());
        }
    }
}
