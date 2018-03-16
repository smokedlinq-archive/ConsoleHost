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
        private readonly Assembly _configuringAssembly;
        private readonly List<Action<IConfigurationBuilder>> _configureAppConfigurationDelegates = new List<Action<IConfigurationBuilder>>();
        private readonly List<Action<ILoggerFactory>> _configureLoggingDelegates = new List<Action<ILoggerFactory>>();
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates = new List<Action<IServiceCollection>>();

        public ConsoleHostBuilder(string[] args = null)
        {
            _args = args ?? new string[0];
            _config = new ConfigurationBuilder()
                        .AddCommandLine(args)
                        .AddEnvironmentVariables()
                        .Build();

            _configuringAssembly = new StackTrace().GetFrames()[2].GetMethod().ReflectedType.Assembly;

            this.AddCommandLine(_configuringAssembly);
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

        public ConsoleHostBuilder AddCommandLine<T>(IDictionary<string, string> switchMappings)
            where T : class
            => AddCommandLine(typeof(T), switchMappings);

        public ConsoleHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
        {
            _configureAppConfigurationDelegates.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
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
            _configureLoggingDelegates.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public ConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configure)
        {
            _configureServicesDelegates.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }

        public ConsoleHost Build()
        {
            var config = BuildConfiguration();
            var services = ConfigureServices(config);

            EnsureConsoleApp(services);

            var hostProvider = services.BuildServiceProvider();

            ConfigureLogging(hostProvider);
            
            IServiceCollection appServices = new ServiceCollection();
            foreach (var service in services)
                appServices.Add(service);
            
            return new ConsoleHost(appServices, hostProvider);
        }

        private IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .AddInMemoryCollection(_config.AsEnumerable());
            
            foreach (var configure in _configureAppConfigurationDelegates)
                configure(builder);

            return builder.Build();
        }

        private IServiceCollection ConfigureServices(IConfiguration config)
        {
            Debug.Assert(config != null);

            var services = new ServiceCollection();

            services.AddOptions();
            services.AddLogging();
            services.AddSingleton<IConfiguration>(config);

            foreach (var configure in _configureServicesDelegates)
                configure(services);

            return services;
        }

        private void EnsureConsoleApp(IServiceCollection services)
        {
            Debug.Assert(services != null);

            if (services.Count(service => service.ServiceType == typeof(IConsoleApp)) == 0)
            {
                foreach (var type in _configuringAssembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(IConsoleApp).IsAssignableFrom(t)))
                    services.AddTransient(typeof(IConsoleApp), type);

                if (services.Count(service => service.ServiceType == typeof(IConsoleApp)) == 0)
                    throw new InvalidOperationException($"The ConsoleHostBuilder could not find a type that implements IConsoleApp; add a public class that implements IConsoleApp to the assembly '{_configuringAssembly.FullName}' or explicitly call UseApp<T>() to specify the type to use.");
            }
        }

        private void ConfigureLogging(IServiceProvider provider)
        {
            Debug.Assert(provider != null);

            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            if (_configureLoggingDelegates.Count == 0)
            {
                loggerFactory.AddConsole();
            }
            else
            {
                foreach (var configure in _configureLoggingDelegates)
                    configure(loggerFactory);
            }
        }
    }
}
