using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public static class NoteSequenceFunctions
    {
        public static IEnumerable<MIDIEvent> ExtractEvents(this IEnumerable<Note> seq)
        {
            return NoteConversion.EncodeNotes(seq);
        }

        public static IEnumerable<MIDIEvent> ExtractEvents(this IEnumerable<Note> seq, FastList<MIDIEvent> buffer)
        {
            return Mergers.MergeWithBuffer(NoteConversion.EncodeNotes(seq), buffer);
        }

        public static IEnumerable<TrackNote> ToTrackNotes(this IEnumerable<Note> seq, int track)
        {
            return NoteConversion.ToTrackNotes(seq, track);
        }
    }
}
