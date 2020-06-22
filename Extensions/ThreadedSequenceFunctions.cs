using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public static class ThreadedSequenceFunctions
    {
        public static IEnumerable<T> ConstantThreadedBuffer<T>(this IEnumerable<T> seq, int maxSize)
        {
            BlockingCollection<T> buffer = new BlockingCollection<T>(maxSize);

            Task.Run(() =>
            {
                foreach (var t in seq)
                {
                    buffer.Add(t);
                }
                buffer.CompleteAdding();
            });

            foreach (var t in buffer.GetConsumingEnumerable())
            {
                yield return t;
            }
        }
        public static IEnumerable<T> ConstantThreadedBuffer<T>(this IEnumerable<T> seq, int maxSize, int batchSize)
        {
            BatchBlockingCollection<T> buffer = new BatchBlockingCollection<T>(batchSize);

            Task.Run(() =>
            {
                foreach (var t in seq)
                {
                    buffer.Add(t);
                }
                buffer.Complete();
            });

            foreach (var t in buffer)
            {
                yield return t;
            }
        }

        public static IEnumerable<T> TaskedThreadedBuffer<T>(this IEnumerable<T> seq, int maxSize)
        {
            BlockingCollection<T> buffer = new BlockingCollection<T>();

            Task readerTask = null;

            bool completed = false;

            var en = seq.GetEnumerator();

            Action runReader = () =>
            {
                if (readerTask != null) readerTask.GetAwaiter().GetResult();
                if (completed) return;
                readerTask = Task.Run(() =>
                {
                    while (buffer.Count < maxSize)
                    {
                        if (!en.MoveNext())
                        {
                            completed = true;
                            buffer.CompleteAdding();
                            return;
                        }
                        buffer.Add(en.Current);
                    }
                    readerTask = null;
                });
            };

            runReader();

            foreach (var t in buffer.GetConsumingEnumerable())
            {
                yield return t;
                if (buffer.Count < maxSize / 2 || buffer.Count == 0)
                {
                    if (!completed) runReader();
                }
            }
        }
    }
}
