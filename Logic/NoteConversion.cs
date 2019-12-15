using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public static class NoteConversion
    {
        class DecodedNote
        {
            public Note note;
            public bool ended = false;
        }

        class UnplacedNoteOff
        {
            public NoteOffEvent e;
            public double time;
        }

        public static IEnumerable<Note> ExtractNotes(IEnumerable<MIDIEvent> sequence, FastList<MIDIEvent> otherEvents = null)
        {
            double time = 0;
            FastList<DecodedNote>[] unendedNotes = new FastList<DecodedNote>[256 * 16];
            FastList<DecodedNote> notesQueue = new FastList<DecodedNote>();
            for (int i = 0; i < unendedNotes.Length; i++) unendedNotes[i] = new FastList<DecodedNote>();
            double delta = 0;
            foreach (var e in sequence)
            {
                time += e.DeltaTime;
                if (e is NoteOnEvent)
                {
                    var n = e as NoteOnEvent;
                    var note = new DecodedNote()
                    {
                        note = new Note(n.Channel, n.Key, n.Velocity, time, 0)
                    };
                    unendedNotes[n.Key * 16 + n.Channel].Add(note);
                    notesQueue.Add(note);
                    delta += e.DeltaTime;
                }
                else if (e is NoteOffEvent)
                {
                    var n = e as NoteOffEvent;
                    var note = unendedNotes[n.Key * 16 + n.Channel].Pop();
                    note.ended = true;
                    note.note.End = time;
                    delta += e.DeltaTime;
                }
                else
                {
                    if (otherEvents != null)
                    {
                        var ev = e.Clone();
                        ev.DeltaTime += delta;
                        delta = 0;
                        otherEvents.Add(ev);
                    }
                }
                if (notesQueue.First != null && notesQueue.First.ended)
                {
                    yield return notesQueue.Pop().note;
                }
            }
            foreach (DecodedNote un in notesQueue)
            {
                if (!un.ended)
                {
                    un.note.End = time;
                }
                yield return un.note;
            }
            notesQueue.Unlink();
            foreach (var s in unendedNotes) s.Unlink();
        }

        public static IEnumerable<MIDIEvent> EncodeNotes(IEnumerable<Note> sequence)
        {
            FastList<UnplacedNoteOff> noteOffs = new FastList<UnplacedNoteOff>();
            double prevTime = 0;

            foreach (var n in sequence)
            {
                while (!noteOffs.ZeroLen && noteOffs.First.time <= n.Start)
                {
                    var e = noteOffs.Pop();
                    e.e.DeltaTime = e.time - prevTime;
                    yield return e.e;
                    prevTime = e.time;
                }

                yield return new NoteOnEvent(n.Start - prevTime, n.Channel, n.Key, n.Velocity);
                prevTime = n.Start;
                var iter = noteOffs.Iterate();
                var time = n.End;
                var off = new UnplacedNoteOff() { e = new NoteOffEvent(0, n.Channel, n.Key), time = time };
                bool notAdded = true;
                UnplacedNoteOff u;
                while (iter.MoveNext(out u))
                {
                    if (u.time > time)
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
            }

            while (!noteOffs.ZeroLen)
            {
                var e = noteOffs.Pop();
                e.e.DeltaTime = e.time - prevTime;
                yield return e.e;
                prevTime = e.time;
            }
        }
    }
}
