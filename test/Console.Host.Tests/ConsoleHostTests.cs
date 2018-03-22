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
        [ExpectedException(typeof(InvalidOperationException))]
        public void MockConsoleAppThatThrowsAnExceptionShouldNotBeSuccessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .UseApp<MockConsoleAppThatThrowsAnInvalidOperationException>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void MockConsoleAppConfiguredWithExplicitServiceProviderFactoryShouldBeSucessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .ConfigureServices(container => container.AddSingleton<IServiceProviderFactory<IServiceCollection>>(_ => new DefaultServiceProviderFactory()))
                .Build()
                .Run();
        }

        [TestMethod]
        public void MockConsoleAppShouldBeSucessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void ConfiguredMockConsoleAppShouldBeSucessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .ConfigureCommandLine<MockConsoleAppConfig>(MockConsoleAppConfig.SwitchMappings)
                .Configure(x => { })
                .ConfigureAppConfiguration(_ => { })
                .ConfigureLogging(_ => { })
                .ConfigureServices(_ => { })
                .UseApp<MockConsoleApp>()
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void ImplicitlyConfiguredMockConsoleAppShouldBeSuccessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .Build()
                .Run();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoConsoleAppShouldNotBeSuccessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .ConfigureServices(container =>
                {
                    var descriptors = container.Where(d => d.ServiceType == typeof(IConsoleApp)).ToArray();
                    foreach (var descriptor in descriptors)
                        container.Remove(descriptor);
                })
                .Build()
                .Run();
        }

        [TestMethod]
        public void MultipleConsoleAppShouldBeSuccessful()
        {
            ConsoleHost
                .CreateBuilder(MockCommandLineArgs.Empty)
                .UseApp<MockConsoleApp>()
                .UseApp<MockConsoleApp>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLineShouldBeSuccessful()
        {
            ConsoleHost
                .CreateBuilder(MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine.Args)
                .UseApp<MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine>()
                .Build()
                .Run();
        }

        [TestMethod]
        public void MockConsoleAppThatWaitsForCancellationShouldBeSuccessful()
        {
            var cts = new CancellationTokenSource();

            var host =
                ConsoleHost
                    .CreateBuilder(MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine.Args)
                    .UseApp<MockConsoleAppThatWaitsForCancellation>()
                    .Build();

            var task = Task.Run(() => host.Run(cts.Token));

            cts.Cancel();

            Task.WaitAny(task, Task.Delay(1000));

            Assert.AreEqual(true, task.IsCompleted);
        }
    }
}
