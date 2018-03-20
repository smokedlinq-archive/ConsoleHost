using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ApplicationInsights
{
    internal class ApplicationInsightsLoggerProvider : ILoggerProvider
    {
        private readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsLoggerProvider(TelemetryClient telemetryClient)
            => _telemetryClient = telemetryClient;

        public ILogger CreateLogger(string categoryName)
        {
            return new ApplicationInsightsLogger(categoryName, _telemetryClient);
        }

        public void Dispose()
        {
        }
    }
}
