Console.Host
=======
[![SonarCloud Status](https://sonarcloud.io/api/project_badges/measure?project=ConsoleHost&metric=alert_status)](https://sonarcloud.io/dashboard?id=ConsoleHost)
[![NuGet](https://img.shields.io/nuget/dt/Console.Host.svg)](https://www.nuget.org/packages/Console.Host)
[![NuGet](https://img.shields.io/nuget/vpre/Console.Host.svg)](https://www.nuget.org/packages/Console.Host)

The library is based on .NET Standard 2.0 compliance which enables the console app to run on any platform that supports .NET Standard 2.0 runtimes.

The basic usage of a ConsoleHost requires creating a ConsoleHostBuilder, building the ConsoleHost instance, and calling it's Run() method. 

The magic of command line parsing, dependency injection, and logging is handled by the ConsoleHost instance.

## Installing Console.Host
You should install [Console.Host](https://www.nuget.org/packages/Console.Host) with NuGet:
```powershell
Install-Package Console.Host
```

## Usage
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

static class Program
{
    static void Main(string[] args)
    {
        ConsoleHost
            .CreateBuilder(args)
            .Build()
            .Run();
    }
}

public class ConsoleApp : IConsoleApp
{
    public async Task RunAsync(CancellationToken cancellationToken)
        => await Console.Out.WriteLineAsync("Hello World!");
}
```

## Dependency Injection
The ConsoleHost leverages the Microsoft.Extensions.DependencyInjection libraries to provide a completely testable experience. 

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
            .ConfigureServices(container => container.AddTransient<MyService>())
            .Build()
            .Run();
    }
}

public class ConsoleApp : IConsoleApp
{
    private readonly MyService _service;

    public ConsoleApp(MyService service)
            => _service = service ?? throw new ArgumentNullException(nameof(service));

    public async Task RunAsync(CancellationToken cancellationToken)
        => await Console.Out.WriteLineAsync(_service.GetGreeting());
}

public class MyService
{
    public string GetGreeting() => "Hello World from MyService";
}
```

## Configuration with Command Line Arguments
The ConsoleHost leverages the Microsoft.Extensions.Configuration libraries to provide a simple CLI argument parsing experience.

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
            .ConfigureServices(container => container.AddTransient<MyService>())
            .Build()
            .Run();
    }
}

public class ConsoleApp : IConsoleApp
{
    private readonly MyService _service;
	private readonly AppConfig _config;

    public ConsoleApp(MyService service, AppConfig config)
    {
		_service = service ?? throw new ArgumentNullException(nameof(service));
		_config = config ?? throw new ArgumentNullException(nameof(config));
	}

    public async Task RunAsync(CancellationToken cancellationToken)
        => await Console.Out.WriteLineAsync(_service.GetGreeting(_config.Name));
}

public class MyService
{
    public string GetGreeting(string name) => $"Hello {name} from MyService";
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

## Logging
The ConsoleHost leverages the Microsoft.Extensions.Logging libraries to provide a simple logging framework. 

If logging is not configured by default it uses the Microsoft.Extensions.Logging.Console provider.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

static class Program
{
    static void Main(string[] args)
    {
        ConsoleHost
            .CreateBuilder(args)
            .ConfigureServices(container => container.AddTransient<MyService>())
            .ConfigureLogging(logging => logging.AddConsole().AddDebug())
            .Build()
            .Run();
    }
}

public class ConsoleApp : IConsoleApp
{
    private readonly MyService _service;
	private readonly AppConfig _config;
	private readonly ILogger<ConsoleApp> _logger;

    public ConsoleApp(MyService service, AppConfig config, ILogger<ConsoleApp> logger)
    {
		_service = service ?? throw new ArgumentNullException(nameof(service));
		_config = config ?? throw new ArgumentNullException(nameof(config));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

    public async Task RunAsync(CancellationToken cancellationToken)
	{
		var greeting = _service.GetGreeting(_config.Name);
		_logger.LogDebug(greeting);
        await Console.Out.WriteLineAsync(greeting);
	}
}

public class MyService
{
    public string GetGreeting(string name) => $"Hello {name} from MyService";
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

Console.Host.ApplicationInsights
=======
[![NuGet](https://img.shields.io/nuget/dt/Console.Host.ApplicationInsights.svg)](https://www.nuget.org/packages/Console.Host.ApplicationInsights)
[![NuGet](https://img.shields.io/nuget/vpre/Console.Host.ApplicationInsights.svg)](https://www.nuget.org/packages/Console.Host.ApplicationInsights)

The Console.Host.ApplicationInsights library adds support for Microsoft Application Insights telemetry through dependency injection and configuration.

In addition of having access to the TelemetryClient, the execution of the RunAsync method is automatically tracked (via DependencyTelemetry) and captures any unhandled exceptions as ExceptionTelemetry.

## Installing Console.Host.ApplicationInsights
You should install [Console.Host.ApplicationInsights](https://www.nuget.org/packages/Console.Host.ApplicationInsights) with NuGet:
```powershell
Install-Package Console.Host.ApplicationInsights
```

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

static class Program
{
    static void Main(string[] args)
    {
        ConsoleHost
            .CreateBuilder(args)
            .UseApplicationInsights(" *your key* ")
            .Build()
            .Run();
    }
}

public class ConsoleApp : IConsoleApp
{
    private readonly TelemetryClient _telemetryClient;
    
    public ConsoleApp(TelemetryClient telemetryClient)
        => _telemetryClient = telemetryClient ?? new TelemetryClient();

    public async Task RunAsync(CancellationToken cancellationToken)
        => await Console.Out.WriteLineAsync("Hello World!");
}
```


Console.Host.Dataflow
=======
[![NuGet](https://img.shields.io/nuget/dt/Console.Host.Dataflow.svg)](https://www.nuget.org/packages/Console.Host.Dataflow)
[![NuGet](https://img.shields.io/nuget/vpre/Console.Host.Dataflow.svg)](https://www.nuget.org/packages/Console.Host.Dataflow)

The Console.Host.Dataflow library adds support for System.Threading.Tasks.Dataflow pipelines as console apps.

## Installing Console.Host.Dataflow
You should install [Console.Host.Dataflow](https://www.nuget.org/packages/Console.Host.Dataflow) with NuGet:
```powershell
Install-Package Console.Host.Dataflow
```

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

static class Program
{
    static void Main(string[] args)
    {
        ConsoleHost
            .CreateBuilder(args)
            .UseDataflow(context =>
            {
                var config = context.Services.GetRequiredService<DataflowConfig>();
                var dataflowBlockOptions = new ExecutionDataflowBlockOptions
                                            {
                                                MaxDegreeOfParallelism = config.MaxDegreeOfParallelism
                                            };

                return Enumerable
                        .Range(1, 10)
                        .ToBufferBlock()
                        .Transform(i => i * 2, dataflowBlockOptions: dataflowBlockOptions)
                        .Action(i => Console.WriteLine(i))
            })
            .Build()
            .Run();
    }
}

public class DataflowConfig
{
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

    public DataflowConfig(IConfiguration config)
        => config?.GetSection(nameof(DataflowConfig)).Bind(this);

    public static IDictionary<string, string> SwitchMappings
        => new Dictionary<string, string>
        {
            { "--mdop", $"{nameof(DataflowConfig)}:{nameof(MaxDegreeOfParallelism)}" }
        };
}
```