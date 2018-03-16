using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public class ConsoleHost
    {
        private readonly IServiceCollection _appServiceCollection;
        private readonly IServiceProvider _hostServices;

        public ConsoleHost(IServiceCollection appServiceCollection, IServiceProvider hostServices)
        {
            _appServiceCollection = appServiceCollection ?? throw new ArgumentNullException(nameof(appServiceCollection));
            _hostServices = hostServices ?? throw new ArgumentNullException(nameof(hostServices));
        }

        public static ConsoleHostBuilder CreateBuilder(string[] args)
        {
            return new ConsoleHostBuilder(args);
        }

        public void Run()
        {
            var apps = _hostServices.GetServices<IConsoleApp>();

            foreach(var app in apps)
                app.ConfigureServices(_appServiceCollection);

            var services = _appServiceCollection.BuildServiceProvider();
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                var tasks = new List<Task>(apps.Count());

                foreach (var app in apps)
                    tasks.Add(app.RunAsync(services, cts.Token));

                Task.WhenAll(tasks).GetAwaiter().GetResult();
            }
            catch
            {
                cts.Cancel();
                throw;
            }
            finally
            {
                services.Dispose();
            }
        }
    }
}
