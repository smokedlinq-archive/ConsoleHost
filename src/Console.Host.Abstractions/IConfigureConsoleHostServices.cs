using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IConfigureConsoleHostServices
    {
        void Configure(IServiceCollection container);
    }
}
