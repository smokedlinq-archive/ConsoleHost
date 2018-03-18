using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    internal class ConsoleHostServiceProviderBuilder
    {
        private readonly List<Action<IServiceCollection>> _delegates = new List<Action<IServiceCollection>>();

        public void Add(Action<IServiceCollection> configure)
        {
            Debug.Assert(configure != null);
            _delegates.Add(configure);
        }

        public IServiceProvider Build(IConfiguration configuration)
        {
            Debug.Assert(configuration != null);

            var services = new ServiceCollection();

            services.AddSingleton(configuration);
            services.AddOptions();
            services.AddLogging();

            foreach (var configure in _delegates)
                configure(services);

            return GetProviderFromFactory(services);
        }

        private IServiceProvider GetProviderFromFactory(IServiceCollection collection)
        {
            var provider = collection.BuildServiceProvider();
            var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

            if (factory != null)
            {
                using (provider)
                {
                    return factory.CreateServiceProvider(factory.CreateBuilder(collection));
                }
            }

            return provider;
        }
    }
}
