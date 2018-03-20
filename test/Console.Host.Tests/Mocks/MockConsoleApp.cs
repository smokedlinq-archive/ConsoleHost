using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    public class MockConsoleApp : IConsoleApp
    {
        public Task RunAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
