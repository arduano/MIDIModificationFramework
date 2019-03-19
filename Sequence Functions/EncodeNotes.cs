using MIDIModificationFramework.MIDI_Events;
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
            public bool removed = false;
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
                ulong smallestOff = 0;
                UnplacedNoteOff smallestOffObj = null;
                bool passedFirst = false;
                var iter = noteOffs.Iterate();
                UnplacedNoteOff n;
                while (iter.MoveNext(out n))
                {
                    if (n.removed)
                        iter.Remove();
                    if (n.time < smallestOff || !passedFirst)
                    {
                        passedFirst = true;
                        smallestOff = n.time;
                        smallestOffObj = n;
                    }
                }
                if (passedFirst)
                {
                    if (nextNote != null)
                    {
                        if (nextNote.Start < smallestOffObj.time)
                        {
                            Current = new NoteOnEvent((uint)(nextNote.Start - prevTime), nextNote.Channel, nextNote.Key, nextNote.Velocity);
                            noteOffs.Add(new UnplacedNoteOff() { e = new NoteOffEvent(0, nextNote.Channel, nextNote.Key), time = nextNote.End });
                            prevTime = nextNote.Start;
                            nextNote = null;
                            return true;
                        }
                        else
                        {
                            Current = smallestOffObj.e;
                            Current.DeltaTime = (uint)(smallestOffObj.time - prevTime);
                            prevTime = smallestOffObj.time;
                            smallestOffObj.removed = true;
                            return true;
                        }
                    }
                    else
                    {
                        Current = smallestOffObj.e;
                        Current.DeltaTime = (uint)(smallestOffObj.time - prevTime);
                        prevTime = smallestOffObj.time;
                        smallestOffObj.removed = true;
                        return true;
                    }
                }
                else
                {
                    if (nextNote != null)
                    {
                        Current = new NoteOnEvent((uint)(nextNote.Start - prevTime), nextNote.Channel, nextNote.Key, nextNote.Velocity);
                        noteOffs.Add(new UnplacedNoteOff() { e = new NoteOffEvent(0, nextNote.Channel, nextNote.Key), time = nextNote.End });
                        prevTime = nextNote.Start;
                        nextNote = null;
                        return true;
                    }
                    else { return false; }
                }
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

            public Iterator(IEnumerable<Note> sequence)
            {
                this.sequence = sequence.GetEnumerator();
            }
        }

        IEnumerable<Note> sequence;

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
