using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class MockConfigureServices : IConfigureConsoleHostServices
    {
        public void Configure(IServiceCollection container)
        {
            // MOCK FOR UNIT TESTS
        }
    }
}
