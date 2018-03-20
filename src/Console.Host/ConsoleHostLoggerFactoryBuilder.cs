using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    internal class ConsoleHostLoggerFactoryBuilder
    {
        private readonly List<Action<IServiceProvider, ILoggerFactory>> _delegates = new List<Action<IServiceProvider, ILoggerFactory>>();

        public void Add(Action<IServiceProvider, ILoggerFactory> configure)
        {
            Debug.Assert(configure != null);
            _delegates.Add(configure);
        }

        public ILoggerFactory Build(IServiceProvider services)
        {
            Debug.Assert(services != null);

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            if (_delegates.Count == 0)
            {
                loggerFactory.AddConsole();
            }
            else
            {
                foreach (var configure in _delegates)
                    configure(services, loggerFactory);
            }

            return loggerFactory;
        }
    }
}
