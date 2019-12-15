using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public static class SequenceFunctions
    {
        public static IEnumerable<MIDIEvent> CancelTempoEvents(IEnumerable<MIDIEvent> sequence, double newTempo, bool returnTempos = false)
        {
            double extraTicks = 0;
            int tempo = 500000;
            double lastDiff = 0;

            foreach (var _e in sequence)
            {
                var e = _e.Clone();
                e.DeltaTime = e.DeltaTime / newTempo * tempo;
                if (e is TempoEvent)
                {
                    var ev = e as TempoEvent;
                    tempo = ev.Tempo;
                    extraTicks = e.DeltaTime + lastDiff;
                    lastDiff = 0;
                    if (returnTempos)
                    {
                        ev.Tempo = (int)newTempo;
                        yield return ev;
                    }
                    continue;
                }
                yield return e;
            }
        }

        public static IEnumerable<MIDIEvent> EventInjector(IEnumerable<MIDIEvent> sequence, Func<MIDIEvent> generator)
        {
            MIDIEvent nextGenerated = generator().Clone();
            foreach (var _e in sequence)
            {
                var e = _e.Clone();
                while (nextGenerated.DeltaTime < e.DeltaTime)
                {
                    e.DeltaTime -= nextGenerated.DeltaTime;
                    yield return nextGenerated;
                    nextGenerated = generator().Clone();
                }
                nextGenerated.DeltaTime -= e.DeltaTime;
                yield return e;
            }
        }

        public static IEnumerable<MIDIEvent> ExtractEvent<T>(IEnumerable<MIDIEvent> sequence)
            where T : MIDIEvent
        {
            foreach (var e in sequence)
            {
                if (e is T) yield return e;
            }
        }

        public static IEnumerable<MIDIEvent> PPQChange(IEnumerable<MIDIEvent> sequence, double startPPQ, double endPPQ)
        {
            var ppqRatio = endPPQ / startPPQ;
            foreach (var _e in sequence)
            {
                var e = _e.Clone();
                e.DeltaTime *= ppqRatio;
                yield return e;
            }
        }

        public static IEnumerable<MIDIEvent> RoundDeltas(IEnumerable<MIDIEvent> sequence)
        {
            double excess = 0;
            foreach (var _e in sequence)
            {
                var e = _e.Clone();
                double newDelta = e.DeltaTime + excess;
                double nextExcess = Math.Round(newDelta) - newDelta;
                e.DeltaTime = Math.Round(newDelta);
                excess = nextExcess;
                yield return e;
            }
        }
    }
}
