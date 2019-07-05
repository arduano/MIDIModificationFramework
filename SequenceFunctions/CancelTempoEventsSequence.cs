using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using MIDIModificationFramework.MIDIEvents;

namespace MIDIModificationFramework
{
    public class CancelTempoEventsSequence : EventSequence
    {
        class Iterator : IEnumerator<MIDIEvent>
        {
            IEnumerator<MIDIEvent> sequence;

            double newTempo;
            double extraTicks = 0;
            int tempo = 500000;

            public Iterator(IEnumerable<MIDIEvent> sequence, double newTempo)
            {
                this.sequence = sequence.GetEnumerator();
                this.newTempo = newTempo;
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
                    var e = sequence.Current.Clone();
                    double time = lastDiff + e.DeltaTime / newTempo * tempo + extraTicks;
                    extraTicks = 0;
                    uint delta = (uint)Math.Round(time);
                    lastDiff = time - delta;
                    e.DeltaTime = delta;
                    if (e is TempoEvent)
                    {
                        var ev = e as TempoEvent;
                        tempo = ev.Tempo;
                        extraTicks = e.DeltaTime + lastDiff;
                        lastDiff = 0;
                    }
                    else
                    {
                        Current = e;
                        break;
                    }
                }
                return true;
            }

            public void Reset()
            {
                sequence.Reset();
            }
        }

        IEnumerable<MIDIEvent> sequence;
        double newTempo;
        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => new IEnumerable<MIDIEvent>[] { sequence };

        public CancelTempoEventsSequence(IEnumerable<MIDIEvent> sequence, double newTempo)
        {
            this.sequence = sequence;
            this.newTempo = newTempo;
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(sequence, newTempo);
        }
    }
}
