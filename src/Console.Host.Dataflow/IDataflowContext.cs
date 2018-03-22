using Microsoft.Extensions.Configuration;

namespace System.Threading.Tasks.Dataflow
{
    public interface IDataflowContext
    {
        IConfiguration Configuration { get; }
        IServiceProvider Services { get; }
    }
}