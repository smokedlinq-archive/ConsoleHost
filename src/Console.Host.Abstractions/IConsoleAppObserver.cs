using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IConsoleAppObserver
    {
        void OnStarting(IConsoleApp app);
        void OnException(IConsoleApp app, Exception ex);
        void OnCompleted(IConsoleApp app);
    }
}
