using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIDIModificationFramework.MIDIEvents;

namespace MIDIModificationFramework
{
    public class TreeTrackMerger : EventSequence
    {
        IEnumerable<IEnumerable<MIDIEvent>> sequences;
        public override IEnumerable<IEnumerable<MIDIEvent>> SourceSequences => sequences;

        public override IEnumerator<MIDIEvent> GetEnumerator()
        {
            return finalMerger.GetEnumerator();
        }

        IEnumerable<MIDIEvent> finalMerger;

        public TreeTrackMerger(IEnumerable<IEnumerable<MIDIEvent>> sequences)
        {
            this.sequences = sequences;
            var batch1 = new List<IEnumerable<MIDIEvent>>();
            var batch2 = new List<IEnumerable<MIDIEvent>>();
            foreach (var s in sequences) batch1.Add(s);
            while (batch1.Count > 1)
            {
                int pos = 0;
                while(pos < batch1.Count)
                {
                    if(batch1.Count - pos == 3)
                    {
                        batch2.Add(new TrackMerger(batch1[pos], batch1[pos + 1], batch1[pos + 2]));
                        pos += 3;
                    }
                    else
                    {
                        batch2.Add(new TrackMerger(batch1[pos], batch1[pos + 1]));
                        pos += 2;
                    }
                }
                batch1 = batch2;
                batch2 = new List<IEnumerable<MIDIEvent>>();
            }
            finalMerger = batch1[0];
        }
    }
}
