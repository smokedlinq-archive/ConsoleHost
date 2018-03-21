using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class DataflowContinuation
    {
        public static ISourceBlock<T> OnCompletion<T>(this ISourceBlock<T> source, Action<Task> continuationAction, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Completion.ContinueWith(continuationAction, cancellationToken);
            
            return source;
        }
    }
}
