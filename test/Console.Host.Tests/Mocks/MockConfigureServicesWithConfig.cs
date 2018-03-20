using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class MockConfigureServicesWithConfig : IConsoleHostServices
    {
        public MockConfigureServicesWithConfig(MockConsoleAppConfig config)
        {
        }

        public void Configure(IServiceCollection container)
        {
        }
    }
}
