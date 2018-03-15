using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class MockConsoleAppConfig
    {
        public string StringValue { get; set; }
        public int Int32Value { get; set; }
        public bool BooleanValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public TimeSpan TimeSpanValue { get; set; }

        public MockConsoleAppConfig(IConfiguration config)
            => config?.GetSection(nameof(MockConsoleAppConfig)).Bind(this);

        public static IDictionary<string, string> SwitchMappings
            => new Dictionary<string, string>
            {
                { "--s", $"{nameof(MockConsoleAppConfig)}:{nameof(StringValue)}" },
                { "--i32", $"{nameof(MockConsoleAppConfig)}:{nameof(Int32Value)}" },
                { "--bool", $"{nameof(MockConsoleAppConfig)}:{nameof(BooleanValue)}" },
                { "--dt", $"{nameof(MockConsoleAppConfig)}:{nameof(DateTimeValue)}" },
                { "--ts", $"{nameof(MockConsoleAppConfig)}:{nameof(TimeSpanValue)}" },
            };
    }
}
