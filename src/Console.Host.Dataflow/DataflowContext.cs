using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace System.Threading.Tasks.Dataflow
{
    internal sealed class DataflowContext : IDataflowContext
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        public DataflowContext(IConfiguration configuration, IServiceProvider services)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(services != null);

            _configuration = configuration;
            _services = services;
        }

        public IConfiguration Configuration => _configuration;

        public IServiceProvider Services => _services;
    }
}