using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            _telemetryClient.Flush();
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

            if (value is IWantOperationTelemetry<DependencyTelemetry> holder)
                holder.Telemetry = operation.Telemetry;
            
            _operations.Add(value, operation);
        }
    }
}