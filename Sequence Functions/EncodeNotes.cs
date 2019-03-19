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
        }

        class Iterator : IEnumerator<MIDIEvent>
        {
            bool ended = false;

            public MIDIEvent Current { get; private set; }
            
            IEnumerator<Note> sequence;
            ulong currentTime = 0;

            FastList<UnplacedNoteOff> noteOffs = new FastList<UnplacedNoteOff>();

            ulong sequenceTime = 0;
            ulong generatorTime = 0;

            object IEnumerator.Current => Current;

            bool hasEndOfTrack = false;

            public bool MoveNext()
            {
                if (ended) return false;

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
