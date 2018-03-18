using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class ConsoleHostBuilderExtensions
    {
        public static IConsoleHostBuilder UseApp<T>(this IConsoleHostBuilder builder)
            where T : class, IConsoleApp
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            return builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d => d.ServiceType == typeof(IConsoleApp)).ToArray();

                foreach (var descriptor in descriptors)
                    services.Remove(descriptor);

                services.AddTransient<IConsoleApp, T>();
            });
        }

        public static IConsoleHostBuilder AddApp<T>(this IConsoleHostBuilder builder)
            where T : class, IConsoleApp
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.ConfigureServices(services => services.AddTransient<IConsoleApp, T>());
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

        public static IConsoleHostBuilder AddCommandLine(this IConsoleHostBuilder builder, Type type, IDictionary<string, string> switchMappings)
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

        public static IConsoleHostBuilder AddCommandLine<T>(this IConsoleHostBuilder builder, IDictionary<string, string> switchMappings)
            where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.AddCommandLine(typeof(T), switchMappings);
        }

        public static IConsoleHostBuilder AddCommandLine(this IConsoleHostBuilder builder, Assembly assembly)
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
                    builder.AddCommandLine(type, switchMappings);
                }
            }

            return builder;
        }
    }
}
