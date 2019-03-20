using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIDIModificationFramework.MIDI_Events;

namespace MIDIModificationFramework
{
    public class ForeachItem<T> : Sequence<T>
    {
        class Iterator : IEnumerator<T>
        {
            IEnumerator<T> sequence;
            Func<T, T> func;

            public Iterator(IEnumerable<T> sequence, Func<T, T> func)
            {
                this.sequence = sequence.GetEnumerator();
                this.func = func;
            }

            public T Current { get; set; }

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                sequence.Dispose();
            }

            public bool MoveNext()
            {
                if (!sequence.MoveNext()) return false;
                Current = func(sequence.Current);
                return true;
            }

            public void Reset()
            {
                sequence.Reset();
            }
        }

        IEnumerable<T> sequence;
        Func<T, T> func;

        public ForeachItem(IEnumerable<T> sequence, Func<T, T> function)
        {
            this.sequence = sequence;
            this.func = function;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new Iterator(sequence, func);
        }
    }
}
