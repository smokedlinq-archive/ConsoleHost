using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.ApplicationInsights
{
    public class ApplicationInsightsLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsLogger(string categoryName, TelemetryClient telemetryClient)
        {
            _categoryName = categoryName;
            _telemetryClient = telemetryClient;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _telemetryClient != null && _telemetryClient.IsEnabled();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var stateDictionary = state as IReadOnlyList<KeyValuePair<string, object>>;

                if (exception == null)
                {
                    var traceTelemetry = new TraceTelemetry(formatter(state, exception), GetSeverityLevel(logLevel));
                    PopulateTelemetry(traceTelemetry, stateDictionary, eventId);
                    _telemetryClient.TrackTrace(traceTelemetry);
                }
                else
                {
                    var exceptionTelemetry = new ExceptionTelemetry(exception)
                    {
                        Message = formatter(state, exception),
                        SeverityLevel = GetSeverityLevel(logLevel)
                    };
                    exceptionTelemetry.Context.Properties["Exception"] = exception.ToString();
                    PopulateTelemetry(exceptionTelemetry, stateDictionary, eventId);
                    _telemetryClient.TrackException(exceptionTelemetry);
                }
            }
        }

        private void PopulateTelemetry(ITelemetry telemetry, IReadOnlyList<KeyValuePair<string, object>> stateDictionary, EventId eventId)
        {
            IDictionary<string, string> dict = telemetry.Context.Properties;
            dict["CategoryName"] = _categoryName;

            if (eventId.Id != 0)
            {
                dict["EventId"] = eventId.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(eventId.Name))
            {
                dict["EventName"] = eventId.Name;
            }

            if (stateDictionary != null)
            {
                foreach (KeyValuePair<string, object> item in stateDictionary)
                {
                    dict[item.Key] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                }
            }
        }

        private static SeverityLevel GetSeverityLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return SeverityLevel.Critical;
                case LogLevel.Error:
                    return SeverityLevel.Error;
                case LogLevel.Warning:
                    return SeverityLevel.Warning;
                case LogLevel.Information:
                    return SeverityLevel.Information;
                case LogLevel.Debug:
                case LogLevel.Trace:
                default:
                    return SeverityLevel.Verbose;
            }
        }
    }
}
