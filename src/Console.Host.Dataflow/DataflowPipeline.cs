using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class DataflowPipeline
    {
        public static readonly DataflowLinkOptions DefaultLinkOptions = new DataflowLinkOptions() { PropagateCompletion = true };

        public static TTarget Next<T, TSource, TTarget>(this TSource source, TTarget target, DataflowLinkOptions linkOptions = null, Predicate<T> predicate = null)
            where TSource : ISourceBlock<T>
            where TTarget : ITargetBlock<T>
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            source.LinkTo(target, linkOptions ?? DefaultLinkOptions, predicate ?? (_ => true));

            return target;
        }
    }
}
