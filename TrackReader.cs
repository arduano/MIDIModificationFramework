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

            bool ended = false;

            public MIDIEvent Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (ended) return false;
                MIDIEvent next = reader.ParseNextEvent();
                if (next is EndOfTrackEvent)
                {
                    ended = true;
                }
                Current = next;
                return true;
            }

            public void Reset()
            {
                ended = false;
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

        EventParser parser;

        Func<EventParser> GetParser;

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
