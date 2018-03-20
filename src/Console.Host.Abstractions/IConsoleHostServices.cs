using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IConsoleHostServices
    {
        void Configure(IServiceCollection container);
    }
}
