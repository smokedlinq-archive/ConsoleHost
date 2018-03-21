using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class BufferBlockFromEnumerable
    {
        public static ISourceBlock<T> ToBufferBlock<T>(this Task<IEnumerable<T>> source, CancellationToken cancellationToken = default(CancellationToken), DataflowBlockOptions dataflowBlockOptions = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new BufferBlock<T>(dataflowBlockOptions ?? new DataflowBlockOptions());

            Task.Run(async () =>
            {
                var items = await source;

                foreach (var item in items)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await block.SendAsync(item, cancellationToken);
                }

                block.Complete();
            }, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        ((IDataflowBlock)block).Fault(t.Exception);
                }, cancellationToken);

            return block;
        }

        public static ISourceBlock<T> ToBufferBlock<T>(this IEnumerable<T> source, CancellationToken cancellationToken = default(CancellationToken), DataflowBlockOptions dataflowBlockOptions = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Task.FromResult(source).ToBufferBlock(cancellationToken, dataflowBlockOptions);
        }
    }
}
