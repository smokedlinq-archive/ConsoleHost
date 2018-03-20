using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ApplicationInsights
{
    public interface IWantOperationTelemetry<T>
        where T : OperationTelemetry
    {
        T Telemetry { set; }
    }
}
