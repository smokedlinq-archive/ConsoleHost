using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientFrom<T>(this IServiceCollection container, Assembly assembly)
        {
            Debug.Assert(container != null);
            Debug.Assert(assembly != null);

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(T).IsAssignableFrom(t)))
                container.AddTransient(typeof(T), type);

            return container;
        }

        public static IServiceCollection ConfigureFrom(this IServiceCollection container, Assembly assembly)
        {
            Debug.Assert(container != null);
            Debug.Assert(assembly != null);

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(IConfigureConsoleHostServices).IsAssignableFrom(t)))
                container.AddTransient(typeof(IConfigureConsoleHostServices), type);

            using (var provider = container.BuildServiceProvider())
                foreach (var service in provider.GetServices<IConfigureConsoleHostServices>())
                    service.Configure(container);

            return container;
        }

        public static IServiceProvider GetProviderFromFactory(this IServiceCollection container)
        {
            Debug.Assert(container != null);

            var provider = container.BuildServiceProvider();
            var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

            if (factory != null)
            {
                using (provider)
                {
                    return factory.CreateServiceProvider(factory.CreateBuilder(container));
                }
            }

            return provider;
        }
    }
}
