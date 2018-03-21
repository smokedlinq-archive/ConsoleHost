using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class TransformBlockDataflow
    {
        public static TransformBlock<TInput, TOutput> Transform<TInput, TOutput>(this ISourceBlock<TInput> source, Func<TInput, Task<TOutput>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions = null, DataflowLinkOptions linkOptions = null, Predicate<TInput> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new TransformBlock<TInput, TOutput>(transform, dataflowBlockOptions ?? new ExecutionDataflowBlockOptions());

            source.Next(block, linkOptions, predicate);

            return block;
        }

        public static TransformBlock<TInput, TOutput> Transform<TInput, TOutput>(this ISourceBlock<TInput> source, Func<TInput, TOutput> transform, ExecutionDataflowBlockOptions dataflowBlockOptions = null, DataflowLinkOptions linkOptions = null, Predicate<TInput> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new TransformBlock<TInput, TOutput>(transform, dataflowBlockOptions ?? new ExecutionDataflowBlockOptions());

            source.Next(block, linkOptions, predicate);

            return block;
        }
    }
}
