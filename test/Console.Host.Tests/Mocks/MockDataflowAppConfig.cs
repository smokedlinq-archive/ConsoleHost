using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Tests
{
    public sealed class MockDataflowAppConfig
    {
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        public MockDataflowAppConfig(IConfiguration config)
            => config?.GetSection(nameof(MockDataflowAppConfig)).Bind(this);

        public static IDictionary<string, string> SwitchMappings
            => new Dictionary<string, string>
            {
            { "--mdop", $"{nameof(MockDataflowAppConfig)}:{nameof(MaxDegreeOfParallelism)}" }
            };
    }
}