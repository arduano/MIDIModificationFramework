using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class EncodeNotes : EventSequence
    {
        class UnplacedNoteOff
        {
            public NoteOffEvent e;
            public ulong time;
        }

        class Iterator : IEnumerator<MIDIEvent>
        {
            bool ended = false;

            public MIDIEvent Current { get; private set; }

            IEnumerator<Note> sequence;

            FastList<UnplacedNoteOff> noteOffs = new FastList<UnplacedNoteOff>();

            object IEnumerator.Current => Current;

            Note nextNote = null;

            ulong prevTime = 0;

            public bool MoveNext()
            {
                if (ended) return false;
                if (nextNote == null)
                {
                    if (!sequence.MoveNext())
                    {
                        if (noteOffs.First == null) return false;
                    }
                    else
                    {
                        nextNote = sequence.Current;
                    }
                }

                if (noteOffs.ZeroLen)
                {
                    SendNote();
                    return true;
                }
                else
                {
                    if (nextNote != null && nextNote.Start < noteOffs.First.time)
                    {
                        SendNote();
                        return true;
                    }
                    else
                    {
                        SendEvent();
                        return true;
                    }
                }
            }

            void SendEvent()
            {
                var e = noteOffs.Pop();
                Current = e.e;
                Current.DeltaTime = (uint)(e.time - prevTime);
                prevTime = e.time;
            }

            void SendNote()
            {
                Current = new NoteOnEvent((uint)(nextNote.Start - prevTime), nextNote.Channel, nextNote.Key, nextNote.Velocity);
                var iter = noteOffs.Iterate();
                UnplacedNoteOff u;
                var time = nextNote.End;
                var off = new UnplacedNoteOff() { e = new NoteOffEvent(0, nextNote.Channel, nextNote.Key), time = time };
                bool notAdded = true;
                while (iter.MoveNext(out u))
                {
                    if(u.time > time)
                    {
                        iter.Insert(off);
                        notAdded = false;
                        break;
                    }
                }
                if (notAdded)
                {
                    noteOffs.Add(off);
                }
                prevTime = nextNote.Start;
                nextNote = null;
            }

            public void Reset()
            {
                ended = false;
                prevTime = 0;
                sequence.Reset();
            }

            public void Dispose()
            {
                sequence.Dispose();
            }

            public Iterator(IEnumerable<Note> sequence)
            {
                this.sequence = sequence.GetEnumerator();
            }
        }

        IEnumerable<Note> sequence;

        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => null;

        public EncodeNotes(IEnumerable<Note> sequence)
        {
            this.sequence = sequence;
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(sequence);
        }
    }
}
