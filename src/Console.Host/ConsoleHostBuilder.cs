using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public class ConsoleHostBuilder
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
                if (string.IsNullOrEmpty(this.GetSetting("CONSOLEHOSTBUILDER_EXPLICIT_CONSOLEAPP_ONLY")))
                    services.AddConsoleAppFrom(configuringAssembly);

                services.ConfigureServicesFrom(configuringAssembly);
            });

            this.AddCommandLine(configuringAssembly);
        }

        public ConsoleHostBuilder AddCommandLine(Type type, IDictionary<string, string> switchMappings)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (switchMappings == null)
                throw new ArgumentNullException(nameof(switchMappings));

            ConfigureAppConfiguration(config => config.AddCommandLine(_args, switchMappings));
            ConfigureServices(services => services.AddTransient(type));

            return this;
        }

        public ConsoleHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
        {
            _appConfigurationBuilder.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public ConsoleHostBuilder UseSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            _config[key] = value;

            return this;
        }

        public string GetSetting(string key)
            => _config[key];

        public ConsoleHostBuilder ConfigureLogging(Action<ILoggerFactory> configure)
        {
            _loggingBuilder.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public ConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configure)
        {
            _servicesBuilder.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public ConsoleHost Build()
        {
            var config = _appConfigurationBuilder.Build(_config);
            var services = _servicesBuilder.Build(config);
            var logging = _loggingBuilder.Build(services);

            return new ConsoleHost(services, logging.CreateLogger<ConsoleHost>());
        }
    }
}
