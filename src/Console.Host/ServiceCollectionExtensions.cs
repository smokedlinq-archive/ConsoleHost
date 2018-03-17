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
        public static IServiceCollection AddConsoleAppFrom(this IServiceCollection services, Assembly assembly)
        {
            Debug.Assert(services != null);
            Debug.Assert(assembly != null);

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(IConsoleApp).IsAssignableFrom(t)))
                services.AddTransient(typeof(IConsoleApp), type);

            return services;
        }

        public static IServiceCollection ConfigureServicesFrom(this IServiceCollection services, Assembly assembly)
        {
            Debug.Assert(services != null);
            Debug.Assert(assembly != null);

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(IConfigureServices).IsAssignableFrom(t)))
                ((IConfigureServices)Activator.CreateInstance(type)).Configure(services);

            return services;
        }
    }
}
