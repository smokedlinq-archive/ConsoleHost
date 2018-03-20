using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ApplicationInsights
{
    public static class ApplicationInsightsConsoleHostBuilderExtensions
    {
        public static IConsoleHostBuilder UseApplicationInsights(this IConsoleHostBuilder builder, string instrumentationKey = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((config, container) =>
            {
                if (!container.Any(service => service.ServiceType == typeof(DependencyTrackingTelemetryModule)))
                {
                    var configuration = TelemetryConfiguration.Active;

                    if (!string.IsNullOrEmpty(instrumentationKey))
                        configuration.InstrumentationKey = instrumentationKey;
                    if (string.IsNullOrEmpty(configuration.InstrumentationKey))
                        configuration.InstrumentationKey = config.GetSection("ApplicationInsights")?["InstrumentationKey"] ?? string.Empty;
                    if (string.IsNullOrEmpty(configuration.InstrumentationKey))
                        configuration.InstrumentationKey = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_INSTRUMENTATIONKEY") ?? string.Empty;

                    if (bool.TryParse(config.GetSection("ApplicationInsights")?.GetSection("TelemetryChannel")?["DeveloperMode"], out var developerMode))
                        configuration.TelemetryChannel.DeveloperMode = developerMode;

                    configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
                    configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

                    var module = new DependencyTrackingTelemetryModule();

                    module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.windows.net");
                    module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.chinacloudapi.cn");
                    module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.cloudapi.de");
                    module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.usgovcloudapi.net");
                    module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("localhost");
                    module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("127.0.0.1");

                    // enable known dependency tracking, note that in future versions, we will extend this list. 
                    // please check default settings in https://github.com/Microsoft/ApplicationInsights-dotnet-server/blob/develop/Src/DependencyCollector/NuGet/ApplicationInsights.config.install.xdt#L20
                    module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
                    module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");

                    module.Initialize(configuration);

                    container.AddSingleton<TelemetryClient>();
                    container.AddSingleton(module);

                    container.AddSingleton(typeof(IConsoleAppObserver), typeof(ConsoleAppObserver));
                }
            });

            builder.ConfigureLogging((services, logger) =>
            {
                var client = services.GetService<TelemetryClient>();
                logger.AddProvider(new ApplicationInsightsLoggerProvider(client));
            });

            return builder;
        }
    }
}
