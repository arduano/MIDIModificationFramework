using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using MIDIModificationFramework.MIDI_Events;

namespace MIDIModificationFramework
{
    public class ExtractEventSequence : EventSequence
    {
        class Iterator : IEnumerator<MIDIEvent>
        {
            IEnumerator<MIDIEvent> sequence;

            Type eventType;
            uint delta = 0;

            public Iterator(IEnumerable<MIDIEvent> sequence, Type eventType)
            {
                this.sequence = sequence.GetEnumerator();
                this.eventType = eventType;
            }

            public MIDIEvent Current { get; set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                sequence.Dispose();
            }
            double lastDiff = 0;
            public bool MoveNext()
            {
                while (true)
                {
                    if (!sequence.MoveNext()) return false;
                    if (eventType.IsInstanceOfType(sequence.Current))
                    {
                        var e = sequence.Current.Clone();
                        e.DeltaTime += delta;
                        delta = 0;
                        Current = e;
                        break;
                    }
                    else delta += sequence.Current.DeltaTime;
                }
                return true;
            }

            public void Reset()
            {
                sequence.Reset();
            }
        }

        IEnumerable<MIDIEvent> sequence;
        Type eventType;
        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => new IEnumerable<MIDIEvent>[] { sequence };

        public ExtractEventSequence(IEnumerable<MIDIEvent> sequence, Type eventType)
        {
            this.sequence = sequence;
            this.eventType = eventType;
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(sequence, eventType);
        }
    }
}
