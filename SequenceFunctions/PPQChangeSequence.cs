﻿using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class PPQChangeSequence : EventSequence
    {
        class Iterator : IEnumerator<MIDIEvent>
        {
            IEnumerator<MIDIEvent> sequence;

            double ppqRatio;

            public Iterator(IEnumerable<MIDIEvent> sequence, double ppqRatio)
            {
                this.sequence = sequence.GetEnumerator();
                this.ppqRatio = ppqRatio;
            }

            public MIDIEvent Current { get; set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                sequence.Dispose();
            }
            double lastDiff = 0;
            public bool MoveNext()
            {
                if (!sequence.MoveNext()) return false;
                var e = sequence.Current.Clone();
                double time = lastDiff + e.DeltaTime * ppqRatio;
                uint delta = (uint)Math.Round(time);
                lastDiff = time - delta;
                e.DeltaTime = delta;
                Current = e;
                return true;
            }

            public void Reset()
            {
                sequence.Reset();
            }
        }

        IEnumerable<MIDIEvent> sequence;
        double ppqRatio;
        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => new IEnumerable<MIDIEvent>[] { sequence };

        public PPQChangeSequence(IEnumerable<MIDIEvent> sequence, double startPPQ, double endPPQ)
        {
            this.sequence = sequence;
            ppqRatio = endPPQ / startPPQ;
        }

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return new Iterator(sequence, ppqRatio);
        }
    }
}
