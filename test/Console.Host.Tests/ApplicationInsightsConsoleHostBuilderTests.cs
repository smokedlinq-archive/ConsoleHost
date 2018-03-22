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
        public void ConsoleHostBuilderWithNoInstrumentationKeyShouldBeSuccessful()
        {
            var host = 
                ConsoleHost
                    .CreateBuilder(MockCommandLineArgs.Empty)
                    .UseApplicationInsights()
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(string.Empty, TelemetryConfiguration.Active.InstrumentationKey);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithExplicitInstrumentationKeyShouldBeSuccessful()
        {
            var host =
                ConsoleHost
                    .CreateBuilder(MockCommandLineArgs.Empty)
                    .UseApplicationInsights(InstrumentationKey)
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(InstrumentationKey, TelemetryConfiguration.Active.InstrumentationKey);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithEnvironmentInstrumentationKeyShouldBeSuccessful()
        {
            Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_INSTRUMENTATIONKEY", InstrumentationKey);

            var host =
                ConsoleHost
                    .CreateBuilder(MockCommandLineArgs.Empty)
                    .UseApplicationInsights()
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(InstrumentationKey, TelemetryConfiguration.Active.InstrumentationKey);
        }

        [TestMethod]
        public void ConsoleHostBuilderWithConfigurationInstrumentationKeyShouldBeSuccessful()
        {
            var host =
                ConsoleHost
                    .CreateBuilder(MockCommandLineArgs.Empty)
                    .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string> { { "ApplicationInsights:InstrumentationKey", InstrumentationKey }, { "ApplicationInsights:TelemetryChannel:DeveloperMode", "true" } }))
                    .UseApplicationInsights()
                    .Build();

            Assert.IsNotNull(host);
            Assert.AreEqual(InstrumentationKey, TelemetryConfiguration.Active.InstrumentationKey);
            Assert.AreEqual(true, TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode);
        }

        [TestMethod]
        public void MockConsoleAppThatLogsShouldBeSuccessful()
        {
            ConsoleHost
                    .CreateBuilder(MockCommandLineArgs.Empty)
                    .UseApplicationInsights(InstrumentationKey)
                    .UseApp<MockConsoleAppThatLogs>(replace: true)
                    .Build()
                    .Run();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MockConsoleAppThatLogsAndThrowsInvalidOperationExceptionShouldBeSuccessful()
        {
            ConsoleHost
                    .CreateBuilder(MockCommandLineArgs.Empty)
                    .UseApplicationInsights(InstrumentationKey)
                    .UseApp<MockConsoleAppThatThrowsAnInvalidOperationException>(replace: true)
                    .Build()
                    .Run();
        }
    }
}
