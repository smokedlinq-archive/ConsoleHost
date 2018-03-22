using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class BufferBlockFromEnumerable
    {
        public static ISourceBlock<T> ToBufferBlock<T>(this Task<IEnumerable<T>> source, DataflowBlockOptions dataflowBlockOptions = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new BufferBlock<T>(dataflowBlockOptions ?? new DataflowBlockOptions());

            Task.Run(async () =>
            {
                var items = await source.ConfigureAwait(false);

                foreach (var item in items)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await block.SendAsync(item, cancellationToken).ConfigureAwait(false);
                }

                block.Complete();
            }, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        ((IDataflowBlock)block).Fault(t.Exception);
                }, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);

            return block;
        }

        public static ISourceBlock<T> ToBufferBlock<T>(this IEnumerable<T> source, DataflowBlockOptions dataflowBlockOptions = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Task.FromResult(source).ToBufferBlock(dataflowBlockOptions, cancellationToken);
        }
    }
}
