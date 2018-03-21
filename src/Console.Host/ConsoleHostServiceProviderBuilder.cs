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
        private readonly List<Action<IConfiguration, IServiceCollection>> _delegates = new List<Action<IConfiguration, IServiceCollection>>();

        public void Add(Action<IConfiguration, IServiceCollection> configure)
        {
            Debug.Assert(configure != null);
            _delegates.Add(configure);
        }

        public IServiceProvider Build(IConfiguration configuration, out IServiceCollection container)
        {
            Debug.Assert(configuration != null);

            container = new ServiceCollection();

            container.AddSingleton(configuration);
            container.AddOptions();
            container.AddLogging();

            foreach (var configure in _delegates)
                configure(configuration, container);

            return container.GetProviderFromFactory();
        }
    }
}
