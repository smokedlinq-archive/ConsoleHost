using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly IServiceProvider _services;
        private readonly ILogger<ConsoleHost> _logger;

        public ConsoleHost(IServiceProvider services, ILogger<ConsoleHost> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static ConsoleHostBuilder CreateBuilder(string[] args)
        {
            return new ConsoleHostBuilder(args);
        }

        public void Run()
        {
            var apps = _services.GetServices<IConsoleApp>();
            
            if (!apps.Any())
                throw new InvalidOperationException($"No service for type '{typeof(IConsoleApp)}' has been registered.");

            var tasks = new List<Task>(apps.Count());
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                foreach (var app in apps)
                {
                    _logger.LogTrace($"[{app.GetType().FullName}] Starting");

                    tasks.Add(
                        app
                            .RunAsync(cts.Token)
                            .ContinueWith(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    _logger.LogCritical(task.Exception, $"[{app.GetType().FullName}] Failed: {task.Exception.InnerException.Message}");
                                    throw task.Exception;
                                }

                                _logger.LogTrace($"[{app.GetType().FullName}] Completed");
                            }, cts.Token));
                }

                Task.WhenAll(tasks).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                cts.Cancel();
                _logger.LogCritical(ex, ex.Message);
                throw;
            }
            finally
            {
                if (_services is IDisposable)
                    ((IDisposable)_services).Dispose();
            }
        }
    }
}
