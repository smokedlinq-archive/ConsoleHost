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
        public void MockConsoleApp_ConfiguredWithExplicitServiceProviderFactory_ShouldBe_Sucessful()
        {
            ConsoleHost
                .CreateBuilder(new string[0])
                .ConfigureServices(container => container.AddSingleton<IServiceProviderFactory<IServiceCollection>>(_ => new DefaultServiceProviderFactory()))
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

        [TestMethod]
        public void MockConsoleAppThatWaitsForCancellation_ShouldBe_Successful()
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
