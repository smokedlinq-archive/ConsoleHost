# ConsoleHost Library
Home of the library that makes writing simple and testable .NET console apps easier. 

The library is based on .NET Standard 2.0 compliance which enables the console app to run on any platform that supports .NET Standard 2.0 runtimes.

The basic usage of a ConsoleHost requires creating a ConsoleHostBuilder, building the ConsoleHost instance, and calling it's Run() method. 

The magic of command line parsing, dependency injection, and is handled by the ConsoleHost instance.

## Basic Usage Example
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    static void Main(string[] args)
    {
        ConsoleHost
            .CreateBuilder(args)
            .Build()
            .Run()
    }
}

public class ConsoleApp : IConsoleApp
{
    public void ConfigureServices(IServiceCollection container) {}

    public async Task RunAsync(IServiceProvider factory, CancellationToken cancellationToken)
        => await Console.Out.WriteLineAsync("Hello World!");
}
```

## Configuration with Command Line Arguments and Dependency Injection
The ConsoleHost leverages the Microsoft.Extensions.Configuration libraries to provide a simple CLI argument parsing experience. 

The Microsoft.Extensions.DependencyInjection libraries provide direct injection during instantiation of the IConsoleApp instances.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    static void Main(string[] args)
    {
        ConsoleHost
            .CreateBuilder(args)
            .Build()
            .Run()
    }
}

public class ConsoleApp : IConsoleApp
{
    private readonly AppConfig _config;

    public ConsoleApp(AppConfig config)
            => _config = config ?? throw new ArgumentNullException(nameof(config));

    public void ConfigureServices(IServiceCollection container) {}

    public async Task RunAsync(IServiceProvider factory, CancellationToken cancellationToken)
        => await Console.Out.WriteLineAsync($"Hello {_config.Name ?? "Unknown"}!");
}

public class AppConfig
{
    public string Name { get; set; }

    public AppConfig(IConfiguration config)
        => config?.GetSection(nameof(AppConfig)).Bind(this);

    public static IDictionary<string, string> SwitchMappings
        => new Dictionary<string, string>
        {
            { "--name", $"{nameof(AppConfig)}:{nameof(Name)}" }
        };
}
```