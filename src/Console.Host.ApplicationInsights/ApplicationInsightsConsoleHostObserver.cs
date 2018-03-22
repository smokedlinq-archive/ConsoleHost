using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.ApplicationInsights
{
    internal sealed class ApplicationInsightsConsoleHostObserver : IConsoleHostObserver, IDisposable
    {
        private readonly TelemetryClient _telemetryClient;
        private IOperationHolder<DependencyTelemetry> _operation;

        public ApplicationInsightsConsoleHostObserver(TelemetryClient telemetryClient)
            => _telemetryClient = telemetryClient ?? new TelemetryClient();

        public void Dispose()
        {
            _telemetryClient.Flush();
        }

        public void OnCompleted()
        {
            _operation.Dispose();
        }

        public void OnException(Exception ex)
        {
            _operation.Telemetry.Success = false;
            _telemetryClient.TrackException(ex);
        }

        public void OnStarting()
        {
            _operation = _telemetryClient.StartOperation<DependencyTelemetry>(typeof(IConsoleHost).FullName);
            _operation.Telemetry.Type = "ConsoleHost";
        }
    }
}