using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public interface IConsoleApp
    {
        void ConfigureServices(IServiceCollection container);
        Task RunAsync(IServiceProvider factory, CancellationToken cancellationToken);
    }
}
