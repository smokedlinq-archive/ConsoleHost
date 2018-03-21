using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class ActionBlockDataflow
    {
        public static ActionBlock<T> Action<T>(this ISourceBlock<T> source, Func<T, Task> action, ExecutionDataflowBlockOptions dataflowBlockOptions = null, DataflowLinkOptions linkOptions = null, Predicate<T> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new ActionBlock<T>(action, dataflowBlockOptions ?? new ExecutionDataflowBlockOptions());

            source.Next(block, linkOptions, predicate);

            return block;
        }

        public static ActionBlock<T> Action<T>(this ISourceBlock<T> source, Action<T> action, ExecutionDataflowBlockOptions dataflowBlockOptions = null, DataflowLinkOptions linkOptions = null, Predicate<T> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new ActionBlock<T>(action, dataflowBlockOptions ?? new ExecutionDataflowBlockOptions());

            source.Next(block, linkOptions, predicate);

            return block;
        }
    }
}
