using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class ConsoleHostBuilderExtensions
    {
        public static IConsoleHostBuilder UseApp<T>(this IConsoleHostBuilder builder, bool replace = false)
            where T : class, IConsoleApp
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (replace)
            { 
                builder.ConfigureServices(container =>
                {
                    var services = container.Where(service => service.ServiceType == typeof(IConsoleApp)).ToArray();
                    foreach (var service in services)
                        container.Remove(service);
                });
            }

            return builder.ConfigureServices(container => container.AddTransient<IConsoleApp, T>());
        }

        public static IConsoleHostBuilder Configure(this IConsoleHostBuilder builder, Action<IConsoleHostBuilder> configure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            configure(builder);
            return builder;
        }

        public static IConsoleHostBuilder ConfigureServices(this IConsoleHostBuilder builder, Action<IServiceCollection> configure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return builder.ConfigureServices((_, container) => configure(container));
        }

        public static IConsoleHostBuilder ConfigureLogging(this IConsoleHostBuilder builder, Action<ILoggerFactory> configure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return builder.ConfigureLogging((_, loggerFactory) => configure(loggerFactory));
        }

        public static IConsoleHostBuilder ConfigureCommandLine(this IConsoleHostBuilder builder, Type type, IDictionary<string, string> switchMappings)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (switchMappings == null)
                throw new ArgumentNullException(nameof(switchMappings));

            return builder
                    .ConfigureCommandLine(() => switchMappings)
                    .ConfigureServices(services => services.AddTransient(type));
        }

        public static IConsoleHostBuilder ConfigureCommandLine<T>(this IConsoleHostBuilder builder, IDictionary<string, string> switchMappings)
            where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.ConfigureCommandLine(typeof(T), switchMappings);
        }

        public static IConsoleHostBuilder ConfigureCommandLine(this IConsoleHostBuilder builder, Assembly assembly)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract))
            {
                PropertyInfo switchMappingsProperty = null;

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    if (property.Name == "SwitchMappings" && typeof(IDictionary<string, string>).IsAssignableFrom(property.PropertyType))
                    {
                        switchMappingsProperty = property;
                        break;
                    }
                }

                if (switchMappingsProperty != null)
                {
                    var switchMappings = (IDictionary<string, string>)switchMappingsProperty.GetValue(null);
                    builder.ConfigureCommandLine(type, switchMappings);
                }
            }

            return builder;
        }
    }
}
