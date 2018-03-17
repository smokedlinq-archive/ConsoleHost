using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    internal class MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine : IConsoleApp
    {
        internal static readonly string[] Args =
            new string[]
            {
                "--s", "string",
                "--i32", "5",
                "--bool", "true",
                "--dt", "8/28/2001 12:01 AM-04:00",
                "--ts", "5.16:34:53.363"
            };

        private readonly MockConsoleAppConfig _config;

        public MockConsoleAppThatValidatesMockConsoleAppConfigFromCommandLine(MockConsoleAppConfig config)
            => _config = config ?? throw new ArgumentNullException(nameof(config));

        public Task RunAsync(CancellationToken cancellationToken)
        {
            Assert.AreEqual(Args[1], _config.StringValue);
            Assert.AreEqual(Int32.Parse(Args[3]), _config.Int32Value);
            Assert.AreEqual(Boolean.Parse(Args[5]), _config.BooleanValue);
            Assert.AreEqual(DateTime.Parse(Args[7]), _config.DateTimeValue);
            Assert.AreEqual(TimeSpan.Parse(Args[9]), _config.TimeSpanValue);

            return Task.CompletedTask;
        }
    }
}
