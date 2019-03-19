using MIDIModificationFramework.MIDI_Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class ExtractNotes : IEnumerable<Note>
    {
        class UnendedNote : Note
        {
            public UnendedNote(byte key, byte vel, ulong start, ulong end) : base(key, vel, start, end) { }
            public bool ended = false;
        }

        class Iterator : IEnumerator<Note>
        {
            IEnumerator<MIDIEvent> events;

            public Iterator(IEnumerable<MIDIEvent> sequence)
            {
                events = sequence.GetEnumerator();
                for (int i = 0; i < unendedNotes.Length; i++) unendedNotes[i] = new FastList<UnendedNote>();
            }
            public Note Current { get; set; }

            object IEnumerator.Current => Current;

            FastList<UnendedNote>[] unendedNotes = new FastList<UnendedNote>[256 * 16];
            FastList<UnendedNote> notesQueue = new FastList<UnendedNote>();

            bool ended = false;

            public void Dispose()
            {
                events.Dispose();
            }

            ulong time = 0;

            public bool MoveNext()
            {
                if (ended) return false;
                while (notesQueue.First == null || notesQueue.First.ended == false)
                {
                    if (!events.MoveNext()) return false;
                    var e = events.Current;
                    time += e.DeltaTime;
                    if (e is NoteOnEvent)
                    {
                        var n = e as NoteOnEvent;
                        var note = new UnendedNote(n.Key, n.Velocity, time, 0);
                        unendedNotes[n.Key * 16 + n.Channel].Add(note);
                        notesQueue.Add(note);
                    }
                    else if(e is NoteOffEvent)
                    {
                        var n = e as NoteOffEvent;
                        var note = unendedNotes[n.Key * 16 + n.Channel].Pop();
                        note.ended = true;
                        note.End = time;
                    }
                }
                Current = notesQueue.Pop();
                return true;
            }

            public void Reset()
            {
                events.Reset();
                time = 0;
                foreach (var l in unendedNotes) l.Unlink();
            }
        }

        IEnumerable<MIDIEvent> sequence;

        public ExtractNotes(IEnumerable<MIDIEvent> sequence) => this.sequence = sequence;

        public IEnumerator<Note> GetEnumerator()
        {
            return new Iterator(sequence);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
