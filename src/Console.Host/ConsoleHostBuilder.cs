using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace System
{
    public sealed class ConsoleHostBuilder : IConsoleHostBuilder
    {
        private readonly string[] _args;
        private readonly IConfiguration _config;
        private readonly ConsoleHostAppConfigurationBuilder _appConfigurationBuilder = new ConsoleHostAppConfigurationBuilder();
        private readonly ConsoleHostLoggingBuilder _loggingBuilder = new ConsoleHostLoggingBuilder();
        private readonly ConsoleHostServiceProviderBuilder _servicesBuilder = new ConsoleHostServiceProviderBuilder();

        public ConsoleHostBuilder(string[] args = null)
        {
            _args = args ?? new string[0];
            _config = new ConfigurationBuilder()
                        .AddCommandLine(args)
                        .AddEnvironmentVariables()
                        .Build();
            
            _appConfigurationBuilder.Add(builder => builder.AddInMemoryCollection(_config.AsEnumerable()));

            var configuringAssembly =
                new StackTrace()
                    .GetFrames()
                    .First(frame => frame.GetMethod().ReflectedType.Assembly != typeof(ConsoleHostBuilder).Assembly)
                    .GetMethod()
                    .ReflectedType
                    .Assembly;

            _servicesBuilder.Add(services =>
            {
                services.AddConsoleAppFrom(configuringAssembly);
                services.ConfigureServicesFrom(configuringAssembly);
            });

            this.AddCommandLine(configuringAssembly);
        }

        public IConsoleHostBuilder ConfigureCommandLine(Func<IDictionary<string, string>> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            _appConfigurationBuilder.Add(container => container.AddCommandLine(_args, configure()));
            return this;
        }

        public IConsoleHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
        {
            _appConfigurationBuilder.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public IConsoleHostBuilder ConfigureLogging(Action<ILoggerFactory> configure)
        {
            _loggingBuilder.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public IConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configure)
        {
            _servicesBuilder.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public IConsoleHost Build()
        {
            var config = _appConfigurationBuilder.Build(_config);
            var services = _servicesBuilder.Build(config);
            var logging = _loggingBuilder.Build(services);

            return new ConsoleHost(services, logging.CreateLogger<ConsoleHost>());
        }
    }
}
