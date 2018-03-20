using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace Microsoft.ApplicationInsights
{
    internal class ConsoleAppObserver : IConsoleAppObserver, IDisposable
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly Dictionary<IConsoleApp, IOperationHolder<DependencyTelemetry>> _operations = new Dictionary<IConsoleApp, IOperationHolder<DependencyTelemetry>>();

        public ConsoleAppObserver(TelemetryClient telemetryClient)
            => _telemetryClient = telemetryClient ?? new TelemetryClient();

        public void Dispose()
        {
            foreach (var item in _operations)
                item.Value.Dispose();

            _operations.Clear();
        }

        public void OnCompleted(IConsoleApp app)
        {
            _operations[app].Dispose();
            _operations.Remove(app);
        }

        public void OnException(IConsoleApp app, Exception ex)
        {
            _operations[app].Telemetry.Success = false;
            _telemetryClient.TrackException(ex);
        }

        public void OnStarting(IConsoleApp value)
        {
            var operation = _telemetryClient.StartOperation<DependencyTelemetry>(value.GetType().FullName);

            operation.Telemetry.Type = "Console";
            operation.Telemetry.Data = $"Run {value.GetType().AssemblyQualifiedName}";

            _operations.Add(value, operation);
        }
    }
}