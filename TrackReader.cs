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

            public double Progress => reader.Progress;

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
                reader.Dispose();
            }

            public Iterator(EventParser reader)
            {
                this.reader = reader;
            }
        }

        Func<EventParser> GetParser;

        bool singleUse = false;
        bool used = true;
        Iterator lastIter = null;

        public double Progress
        {
            get
            {
                if (singleUse)
                    if (lastIter == null)
                        return 0;
                    else
                        return lastIter.Progress;
                throw new Exception("Progress can only be accessed on single use streams");
            }
        }

        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => null;

        internal TrackReader(Func<EventParser> parserfunc)
        {
            GetParser = parserfunc;
        }

        public TrackReader GetSingleUse()
        {
            return new TrackReader(GetParser) { singleUse = true };
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            if (singleUse)
            {
                lastIter = new Iterator(GetParser());
                return lastIter;
            }
            return new Iterator(GetParser());
        }
    }
}
