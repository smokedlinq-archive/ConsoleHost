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
        public static ConsoleHostBuilder UseApp<T>(this ConsoleHostBuilder builder)
            where T : class, IConsoleApp
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            builder.ConfigureServices(services => services.AddTransient<IConsoleApp, T>());

            return builder;
        }

        public static ConsoleHostBuilder Configure(this ConsoleHostBuilder builder, Action<ConsoleHostBuilder> configure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            configure(builder);
            return builder;
        }

        public static ConsoleHostBuilder AddCommandLine<T>(this ConsoleHostBuilder builder, IDictionary<string, string> switchMappings)
            where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.AddCommandLine(typeof(T), switchMappings);
        }

        public static ConsoleHostBuilder AddCommandLine(this ConsoleHostBuilder builder, Assembly assembly)
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
