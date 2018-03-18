using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace System
{
    public interface IConsoleHostBuilder
    {
        IConsoleHostBuilder ConfigureCommandLine(Func<IDictionary<string, string>> configure);
        IConsoleHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure);
        IConsoleHostBuilder ConfigureLogging(Action<ILoggerFactory> configure);
        IConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configure);
        IConsoleHost Build();
    }
}