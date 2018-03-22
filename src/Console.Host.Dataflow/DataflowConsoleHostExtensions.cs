using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks.Dataflow
{
    public static class DataflowConsoleHostExtensions
    {
        public static IConsoleHostBuilder UseDataflow(this IConsoleHostBuilder builder, Func<IDataflowContext, IDataflowBlock> factory)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            builder.ConfigureServices(container =>
            {
                container.AddSingleton<IConsoleApp>(services =>
                {
                    var context = new DataflowContext(services.GetRequiredService<IConfiguration>(), services);
                    return new DataflowConsoleApp(context, factory);
                });
            });

            return builder;
        }
    }
}
