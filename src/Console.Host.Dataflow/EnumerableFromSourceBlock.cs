using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class EnumerableFromSourceBlock
    {
        public static IEnumerable<T> AsEnumerable<T>(this ISourceBlock<T> source, CancellationToken cancellationToken = default)
        {
            while (!source.Completion.IsCompleted && source.OutputAvailableAsync(cancellationToken).GetAwaiter().GetResult() && !cancellationToken.IsCancellationRequested)
                yield return source.Receive();
        }
    }
}
