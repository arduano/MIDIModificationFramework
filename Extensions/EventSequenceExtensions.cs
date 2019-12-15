using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public static class EventSequenceExtensions
    {
        public static IEnumerable<MIDIEvent> CancelTempoEvents(this IEnumerable<MIDIEvent> seq, double newTempo)
        {
            return SequenceFunctions.CancelTempoEvents(seq, newTempo);
        }

        public static IEnumerable<MIDIEvent> CancelTempoEvents(this IEnumerable<MIDIEvent> seq, double newTempo, bool returnTempos)
        {
            return SequenceFunctions.CancelTempoEvents(seq, newTempo, returnTempos);
        }

        public static IEnumerable<MIDIEvent> InjectEvents(this IEnumerable<MIDIEvent> seq, Func<MIDIEvent> generator)
        {
            return SequenceFunctions.EventInjector(seq, generator);
        }

        public static IEnumerable<MIDIEvent> ExtractEvents(this IEnumerable<MIDIEvent> seq, Type type)
        {
            double delta = 0;
            foreach (var e in seq)
            {
                if (type.IsInstanceOfType(e))
                {
                    var ev = e.Clone();
                    ev.DeltaTime += delta;
                    delta = 0;
                    yield return ev;
                }
                else
                {
                    delta += e.DeltaTime;
                }
            }
        }

        public static IEnumerable<MIDIEvent> ExtractEvents(this IEnumerable<MIDIEvent> seq, IEnumerable<Type> types)
        {
            double delta = 0;
            foreach (var e in seq)
            {
                bool extends = false;
                foreach (var t in types)
                {
                    if (t.IsInstanceOfType(e))
                    {
                        extends = true;
                        break;
                    }
                }
                if (extends)
                {
                    var ev = e.Clone();
                    ev.DeltaTime += delta;
                    delta = 0;
                    yield return ev;
                }
                else
                {
                    delta += e.DeltaTime;
                }
            }
        }

        public static IEnumerable<MIDIEvent> ChangePPQ(this IEnumerable<MIDIEvent> seq, double startPPQ, double endPPQ)
        {
            return SequenceFunctions.PPQChange(seq, startPPQ, endPPQ);
        }

        public static IEnumerable<MIDIEvent> ChangePPQ(this IEnumerable<MIDIEvent> seq, double ppqMultiplier)
        {
            return SequenceFunctions.PPQChange(seq, 1, ppqMultiplier);
        }

        public static IEnumerable<MIDIEvent> MergeWith(this IEnumerable<MIDIEvent> seq, IEnumerable<MIDIEvent> seq2)
        {
            return Mergers.MergeSequences(seq, seq2);
        }

        public static IEnumerable<MIDIEvent> MergeBuffer(this IEnumerable<MIDIEvent> seq, FastList<MIDIEvent> buffer)
        {
            return Mergers.MergeWithBuffer(seq, buffer);
        }

        public static IEnumerable<Note> ExtractNotes(this IEnumerable<MIDIEvent> seq)
        {
            return NoteConversion.ExtractNotes(seq);
        }

        public static IEnumerable<Note> ExtractNotes(this IEnumerable<MIDIEvent> seq, FastList<MIDIEvent> otherEvents)
        {
            return NoteConversion.ExtractNotes(seq, otherEvents);
        }

        public static IEnumerable<Note> WriteTo(this IEnumerable<MIDIEvent> seq, IEventWriter writer)
        {
            foreach(var e in seq)
            {
                writer.WriteEvent(e);
            }
        }
    }
}
