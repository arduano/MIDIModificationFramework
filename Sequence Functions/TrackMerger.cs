using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIDIModificationFramework.MIDI_Events;

namespace MIDIModificationFramework
{
    public class TrackMerger : EventSequence
    {
        class Iterator : IEnumerator<MIDIEvent>
        {
            public MIDIEvent Current { get; private set; }

            IEnumerator<MIDIEvent>[] iterators;
            MIDIEvent[] nextEvents;
            ulong[] trackTimes;
            bool[] finishedTracks;
            ulong currentTime = 0;
            int tracks;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                int unended = 0;
                ulong smallest = 0;
                int smallestid = 0;
                bool first = true;
                for (int i = 0; i < tracks; i++)
                {
                    if (finishedTracks[i]) continue;
                    unended++;
                    if (trackTimes[i] < smallest || first)
                    {
                        smallest = trackTimes[i];
                        smallestid = i;
                    }
                    first = false;
                }
                if (unended == 0)
                {
                    Current = null;
                    return false;
                }
                var e = nextEvents[smallestid];
                e.DeltaTime = (uint)(trackTimes[smallestid] - currentTime);
                currentTime = trackTimes[smallestid];
                Current = e;
                StepTrack(smallestid);
                return true;
            }

            public void Reset()
            {
                for (int i = 0; i < tracks; i++)
                {
                    finishedTracks[i] = false;
                    iterators[i].Reset();
                    trackTimes[i] = 0;
                    StepTrack(i);
                }
            }

            public void Dispose()
            {
                for (int i = 0; i < tracks; i++)
                {
                    iterators[i].Dispose();
                }
            }

            void StepTrack(int i)
            {
                if (!iterators[i].MoveNext())
                {
                    finishedTracks[i] = true;
                    return;
                }
                var e = iterators[i].Current;
                if (e == null)
                {
                    finishedTracks[i] = true;
                    return;
                }
                nextEvents[i] = e;
                trackTimes[i] += e.DeltaTime;
            }

            public Iterator(IEnumerable<MIDIEvent>[] sequences)
            {
                int len = sequences.Length;
                tracks = len;
                iterators = new IEnumerator<MIDIEvent>[len];
                trackTimes = new ulong[len];
                finishedTracks = new bool[len];
                nextEvents = new MIDIEvent[len];
                for (int i = 0; i < len; i++)
                {
                    finishedTracks[i] = false;
                    iterators[i] = sequences[i].GetEnumerator();
                    trackTimes[i] = 0;
                    StepTrack(i);
                }
            }
        }


        IEnumerable<MIDIEvent>[] Sequences;

        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => Sequences;

        public TrackMerger(params IEnumerable<MIDIEvent>[] sequences)
        {
            Sequences = sequences;
        }

        public TrackMerger(IEnumerable<IEnumerable<MIDIEvent>> sequences)
        {
            Sequences = sequences.ToArray();
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(Sequences);
        }
    }
}
