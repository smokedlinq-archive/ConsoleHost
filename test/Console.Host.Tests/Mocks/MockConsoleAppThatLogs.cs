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
    internal class MockConsoleAppThatLogs : IConsoleApp
    {
        private readonly TelemetryClient _telemetryClient;

        public MockConsoleAppThatLogs(ILogger<MockConsoleAppThatLogs> logger, TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
            _telemetryClient.TrackTrace("Mock log data from MockConsoleAppThatLogs", SeverityLevel.Information, new Dictionary<string, string> { { "MockConsoleAppThatLogs", "true" } });
            _telemetryClient.TrackException(new Exception("Mock exception MockConsoleAppThatLogs"), new Dictionary<string, string> { { "MockConsoleAppThatLogs", "true" } });
            logger.LogTrace("Mock log data from MockConsoleAppThatLogs");
            logger.LogError(new Exception("Mock exception from MockConsoleAppThatLogs"), "Mock log exception from MockConsoleAppThatLogs");
        }

        public Task RunAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
