using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public sealed class MockConfigureServicesWithConfig : IConfigureConsoleHostServices
    {
        public MockConfigureServicesWithConfig(MockConsoleAppConfig config)
        {
        }

        public void Configure(IServiceCollection container)
        {
            // MOCK FOR UNIT TESTS
        }
    }
}
