using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class BatchBlockDataflow
    {
        public static BatchBlock<T> Batch<T>(this ISourceBlock<T> source, int batchSize, GroupingDataflowBlockOptions dataflowBlockOptions = null, DataflowLinkOptions linkOptions = null, Predicate<T> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var block = new BatchBlock<T>(batchSize, dataflowBlockOptions ?? new GroupingDataflowBlockOptions());

            source.Next(block, linkOptions, predicate);

            return block;
        }
    }

}
