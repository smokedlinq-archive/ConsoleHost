using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.ApplicationInsights
{
    internal sealed class ApplicationInsightsConsoleAppObserver : IConsoleAppObserver, IDisposable
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ConcurrentDictionary<IConsoleApp, IOperationHolder<DependencyTelemetry>> _operations = new ConcurrentDictionary<IConsoleApp, IOperationHolder<DependencyTelemetry>>();

        public ApplicationInsightsConsoleAppObserver(TelemetryClient telemetryClient)
            => _telemetryClient = telemetryClient ?? new TelemetryClient();

        public void Dispose()
        {
            foreach (var item in _operations)
                item.Value.Dispose();

            _operations.Clear();

            _telemetryClient.Flush();
        }

        public void OnCompleted(IConsoleApp app)
        {
            if (_operations.TryRemove(app, out var operation))
                operation.Dispose();
        }

        public void OnException(IConsoleApp app, Exception ex)
        {
            if (_operations.TryGetValue(app, out var operation))
                operation.Telemetry.Success = false;
            
            _telemetryClient.TrackException(ex);
        }

        public void OnStarting(IConsoleApp app)
        {
            var operation = _telemetryClient.StartOperation<DependencyTelemetry>(app.GetType().FullName);

            operation.Telemetry.Type = "Console";
            operation.Telemetry.Data = $"Run {app.GetType().AssemblyQualifiedName}";

            _operations.TryAdd(app, operation);
        }
    }
}