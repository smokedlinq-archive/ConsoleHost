using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ConsoleHostTests
    {
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void MockConsoleAppThatThrowsAnException_ShouldNotBe_Successful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseApp<MockConsoleAppThatThrowsAnInvalidOperationException>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void MockConsoleApp_ShouldBe_Sucessful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void ConfiguredMockConsoleApp_ShouldBe_Sucessful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseSetting("SETTING", "VALUE")
                .AddCommandLine<MockConsoleAppConfig>(MockConsoleAppConfig.SwitchMappings)
                .Configure(x => { x.GetSetting("SETTING"); })
                .ConfigureAppConfiguration(_ => { })
                .ConfigureLogging(_ => { })
                .ConfigureServices(_ => { })
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void ImplicitlyConfiguredMockConsoleApp_ShouldBe_Successful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .Build()
                .Run();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoConsoleApp_ShouldNotBe_Successful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseSetting("CONSOLEHOSTBUILDER_EXPLICIT_CONSOLEAPP_ONLY", "true")
                .Build()
                .Run();
        }

        [TestMethod]
        public void MultipleConsoleApp_ShouldBe_Successful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseApp<MockConsoleApp>()
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine_ShouldBe_Successful()
        {
            ConsoleHost
                .CreateBuilder(MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine.Args)
                .UseApp<MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine>()
                .Build()
                .Run();
        }
    }
}
