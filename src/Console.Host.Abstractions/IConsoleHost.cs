using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System
{
    public interface IConsoleHost
    {
        void Run(CancellationToken cancellationToken = default);
    }
}
