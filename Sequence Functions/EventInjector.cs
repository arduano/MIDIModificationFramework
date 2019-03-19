using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIDIModificationFramework.MIDI_Events;

namespace MIDIModificationFramework
{
    public class EventInjector : EventSequence
    {
        class Iterator : IEnumerator<MIDIEvent>
        {
            bool ended = false;

            public MIDIEvent Current { get; private set; }

            Func<MIDIEvent> getEvent;
            IEnumerator<MIDIEvent> sequence;
            MIDIEvent nextGenEvent;
            MIDIEvent nextSequenceEvent;
            ulong currentTime = 0;

            ulong sequenceTime = 0;
            ulong generatorTime = 0;

            object IEnumerator.Current => Current;

            bool hasEndOfTrack = false;

            public bool MoveNext()
            {
                if (ended) return false;
                if (sequenceTime <= generatorTime)
                {
                    if (nextSequenceEvent == null)
                    {
                        if (!hasEndOfTrack)
                        {
                            Current = new EndOfTrackEvent();
                            ended = true;
                            return true;
                        }
                        else
                            return false;
                    }
                    var e = nextSequenceEvent;
                    e.DeltaTime = (uint)(sequenceTime - currentTime);
                    currentTime = sequenceTime;
                    Current = e;
                    if (!sequence.MoveNext())
                    {
                        nextSequenceEvent = null;
                        ended = true;
                        return true;
                    }
                    else
                    {
                        nextSequenceEvent = sequence.Current;
                        sequenceTime += nextSequenceEvent.DeltaTime;
                    }
                }
                else
                {
                    if (nextGenEvent == null)
                    {
                        generatorTime = 0;
                        return true;
                    }
                    var e = nextGenEvent;
                    e.DeltaTime = (uint)(generatorTime - currentTime);
                    currentTime = generatorTime;
                    Current = e;
                    nextGenEvent = getEvent();
                    if(nextGenEvent != null)
                    {
                        generatorTime += nextGenEvent.DeltaTime;
                    }
                }
                return true;
            }

            public void Reset()
            {
                ended = false;
                sequence.Reset();
            }

            public void Dispose()
            {
                sequence.Dispose();
            }

            public Iterator(IEnumerable<MIDIEvent> sequence, Func<MIDIEvent> generator)
            {
                getEvent = generator;
                nextGenEvent = generator();
                generatorTime = nextGenEvent.DeltaTime;
                this.sequence = sequence.GetEnumerator();
                this.sequence.MoveNext();
                nextSequenceEvent = this.sequence.Current;
                sequenceTime = nextSequenceEvent.DeltaTime;
            }
        }

        Func<MIDIEvent> getEvent;
        IEnumerable<MIDIEvent> sequence;

        public EventInjector(IEnumerable<MIDIEvent> sequence, Func<MIDIEvent> generator)
        {
            this.sequence = sequence;
            this.getEvent = generator;
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(sequence, getEvent);
        }
    }
}
