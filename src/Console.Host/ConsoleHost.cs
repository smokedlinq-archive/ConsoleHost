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
    public sealed class ConsoleHost : IConsoleHost
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ConsoleHost> _logger;
        
        public ConsoleHost(IServiceProvider services, ILogger<ConsoleHost> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static ConsoleHostBuilder CreateBuilder(string[] args)
            => new ConsoleHostBuilder(args);

        public void Run(CancellationToken cancellationToken = default)
        {
            var apps = _services.GetServices<IConsoleApp>();
            
            if (!apps.Any())
                throw new InvalidOperationException($"No service for type '{typeof(IConsoleApp)}' has been registered.");

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var observers = _services.GetServices<IConsoleAppObserver>();
                var tasks = new List<Task>(apps.Count());

                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    foreach (var app in apps)
                    {
                        observers.Start(app);

                        tasks.Add(
                            app.RunAsync(cts.Token)
                                .ContinueWith(task =>
                                {
                                    if (task.IsFaulted)
                                        observers.Exception(app, task.Exception);

                                    observers.Complete(app);

                                    if (task.IsFaulted)
                                        throw task.Exception;
                                }, cts.Token));
                    }

                    Task.WhenAll(tasks).GetAwaiter().GetResult();
                }
                catch (TaskCanceledException)
                {
                    // NOOP
                }
                catch (Exception ex)
                {
                    cts.Cancel();
                    _logger.LogCritical(ex, ex.Message);
                    throw;
                }
                finally
                {
                    if (_services is IDisposable dispoable)
                        dispoable.Dispose();
                }
            }
        }
    }
}
