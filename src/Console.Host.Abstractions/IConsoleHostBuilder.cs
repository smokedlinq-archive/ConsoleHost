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
        IConsoleHostBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> configure);
        IConsoleHostBuilder ConfigureLogging(Action<IServiceProvider, ILoggerFactory> configure);
        IConsoleHost Build();
    }
}