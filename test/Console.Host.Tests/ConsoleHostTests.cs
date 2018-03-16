using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ConsoleHostTests
    {
        [TestMethod]
        public void SimpleMockConsoleApp_ShouldBe_Sucessful()
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
        public void ConsoleHostShouldRunImplicitMockConsoleApp()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .Build()
                .Run();
        }

        [TestMethod]
        public void ConsoleHostShouldRunMultipleExplicitMockConsoleApp()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .UseApp<MockConsoleApp>()
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void ConsoleHostShouldRunExplicitNonPublicConsoleAppWithConfigValidationFromCommandLineArgs()
        {
            ConsoleHost
                .CreateBuilder(MockConsoleAppThatValidatesMockConsoleAppConfig.Args)
                .UseApp<MockConsoleAppThatValidatesMockConsoleAppConfig>()
                .Build()
                .Run();
        }
    }
}
