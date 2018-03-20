using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    internal class MockConsoleAppThatWantsOperationTelemetry : IConsoleApp, IWantOperationTelemetry<DependencyTelemetry>
    {
        public DependencyTelemetry Telemetry { get; set; }

        public Task RunAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
