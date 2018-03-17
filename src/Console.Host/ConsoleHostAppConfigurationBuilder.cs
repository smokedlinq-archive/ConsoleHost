using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    internal class ConsoleHostAppConfigurationBuilder
    {
        private readonly List<Action<IConfigurationBuilder>> _delegates = new List<Action<IConfigurationBuilder>>();

        public void Add(Action<IConfigurationBuilder> configure)
        {
            Debug.Assert(configure != null);
            _delegates.Add(configure);
        }

        public IConfiguration Build(IConfiguration config)
        {
            var builder = new ConfigurationBuilder();

            foreach (var configure in _delegates)
                configure(builder);

            return builder.Build();
        }
    }
}
