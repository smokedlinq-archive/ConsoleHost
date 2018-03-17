using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IConfigureServices
    {
        void Configure(IServiceCollection container);
    }
}
