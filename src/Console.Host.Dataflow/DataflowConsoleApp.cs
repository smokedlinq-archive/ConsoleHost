using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    internal sealed class DataflowConsoleApp : IConsoleApp
    {
        private readonly IDataflowContext _context;
        private readonly Func<IDataflowContext, IDataflowBlock> _factory;

        public DataflowConsoleApp(IDataflowContext context, Func<IDataflowContext, IDataflowBlock> factory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
            => await _factory(_context).Completion.ConfigureAwait(false);
    }
}
