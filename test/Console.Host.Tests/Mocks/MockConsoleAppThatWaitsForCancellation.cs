using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    internal class MockConsoleAppThatWaitsForCancellation : IConsoleApp
    {
        public Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne();
            return Task.CompletedTask;
        }
    }
}
