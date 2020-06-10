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

            newTempo = tempo * (tempo / newTempo);

            foreach (var _e in sequence)
            {
                var e = _e.Clone();
                e.DeltaTime = e.DeltaTime / newTempo * tempo + extraTicks;
                extraTicks = 0;
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
            double time = 0;
            double roundedtime = 0;
            foreach (var _e in sequence)
            {
                var e = _e.Clone();
                time += e.DeltaTime;
                var round = Math.Round(time);
                e.DeltaTime = round - roundedtime;
                roundedtime = round;
                yield return e;
            }
        }

        public static IEnumerable<MIDIEvent> FilterEvents(IEnumerable<MIDIEvent> sequence, Func<MIDIEvent, bool> func)
        {
            double extraDelta = 0;
            foreach (var _e in sequence)
            {
                if (func(_e))
                {
                    var e = _e.Clone();
                    e.DeltaTime += extraDelta;
                    extraDelta = 0;
                    yield return e;
                }
                else
                {
                    extraDelta += _e.DeltaTime;
                }
            }
        }
    }
}
