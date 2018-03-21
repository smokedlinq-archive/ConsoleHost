using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IConsoleHostObserver
    {
        void OnStarting();
        void OnException(Exception ex);
        void OnCompleted();
    }
}
