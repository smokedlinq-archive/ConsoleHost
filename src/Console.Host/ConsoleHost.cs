using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public sealed class ConsoleHost : IConsoleHost
    {
        private readonly IServiceCollection _container;
        private readonly ILogger<ConsoleHost> _logger;
        
        public ConsoleHost(IServiceCollection container, ILogger<ConsoleHost> logger)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static ConsoleHostBuilder CreateBuilder(string[] args)
            => new ConsoleHostBuilder(args);

        public void Run(CancellationToken cancellationToken = default)
        {
            var services = _container.GetProviderFromFactory();
            var apps = services.GetServices<IConsoleApp>();
            
            if (!apps.Any())
                throw new InvalidOperationException($"No service for type '{typeof(IConsoleApp)}' has been registered.");

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var observers = services.GetServices<IConsoleHostObserver>();

                observers.OnStarting();

                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    var tasks = apps.Select(app => RunAsync(services, app, cts.Token));
                    Task.WhenAll(tasks).GetAwaiter().GetResult();
                }
                catch (TaskCanceledException)
                {
                    // NOOP
                }
                catch (Exception ex)
                {
                    cts.Cancel();
                    observers.OnException(ex);
                    _logger.LogCritical(ex, ex.Message);
                    throw;
                }
                finally
                {
                    observers.OnCompleted();

                    if (services is IDisposable dispoable)
                        dispoable.Dispose();
                }
            }
        }

        private async Task RunAsync(IServiceProvider services, IConsoleApp app, CancellationToken cancellationToken)
        {
            Debug.Assert(services != null);
            Debug.Assert(app != null);

            var observers = services.GetServices<IConsoleAppObserver>();

            observers.OnStarting(app);

            try
            {
                await app.RunAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                observers.OnException(app, ex);
                _logger.LogCritical(ex, "[{0}] {1}", app.GetType().FullName, ex.Message);
                throw;
            }
            finally
            {
                observers.OnCompleted(app);
            }
        }
    }
}
