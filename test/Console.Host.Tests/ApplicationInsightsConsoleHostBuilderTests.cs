using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ApplicationInsightsConsoleHostBuilderTests
    {
        private const string InstrumentationKey = "00000000-0000-0000-0000-000000000000";

        [TestInitialize]
        public void Initialize()
        {
            TelemetryConfiguration.Active.TelemetryInitializers.Clear();
            TelemetryConfiguration.Active.InstrumentationKey = string.Empty;
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = false;
            Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_INSTRUMENTATIONKEY", string.Empty);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithNoInstrumentationKey_ShouldBe_Successful()
        {
            var host = 
                ConsoleHost
                    .CreateBuilder(new string[0])
                    .UseApplicationInsights()
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(string.Empty, TelemetryConfiguration.Active.InstrumentationKey);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithExplicitInstrumentationKey_ShouldBe_Successful()
        {
            var host =
                ConsoleHost
                    .CreateBuilder(new string[0])
                    .UseApplicationInsights(InstrumentationKey)
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(InstrumentationKey, TelemetryConfiguration.Active.InstrumentationKey);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithEnvironmentInstrumentationKey_ShouldBe_Successful()
        {
            Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_INSTRUMENTATIONKEY", InstrumentationKey);

            var host =
                ConsoleHost
                    .CreateBuilder(new string[0])
                    .UseApplicationInsights()
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(InstrumentationKey, TelemetryConfiguration.Active.InstrumentationKey);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithConfigurationInstrumentationKey_ShouldBe_Successful()
        {
            var host =
                ConsoleHost
                    .CreateBuilder(new string[0])
                    .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string> { { "ApplicationInsights:InstrumentationKey", InstrumentationKey }, { "ApplicationInsights:TelemetryChannel:DeveloperMode", "true" } }))
                    .UseApplicationInsights()
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(InstrumentationKey, TelemetryConfiguration.Active.InstrumentationKey);
            Assert.AreEqual(true, TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode);
        }

        [TestMethod]
        public void MockConsoleAppThatLogs_ShouldBe_Successful()
        {
            ConsoleHost
                    .CreateBuilder(new string[0])
                    .UseApplicationInsights(InstrumentationKey)
                    .UseApp<MockConsoleAppThatLogs>(replace: true)
                    .Build()
                    .Run();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void MockConsoleAppThatLogsAndThrowsInvalidOperationException_ShouldBe_Successful()
        {
            ConsoleHost
                    .CreateBuilder(new string[0])
                    .UseApplicationInsights(InstrumentationKey)
                    .UseApp<MockConsoleAppThatThrowsAnInvalidOperationException>(replace: true)
                    .Build()
                    .Run();
        }

        [TestMethod]
        public void MockConsoleAppThatWantsOperationTelemetry_ShouldBe_Successful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseApplicationInsights(InstrumentationKey)
                .UseApp<MockConsoleAppThatWantsOperationTelemetry>(replace: true)
                .Build()
                .Run();
        }
    }
}
