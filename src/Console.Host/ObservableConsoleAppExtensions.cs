using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    internal static class ObservableConsoleAppExtensions
    {
        public static void Start(this IEnumerable<IConsoleAppObserver> observers, IConsoleApp app)
        {
            Debug.Assert(observers != null);
            Debug.Assert(app != null);

            foreach (var observer in observers)
                observer.OnStarting(app);
        }

        public static void Exception(this IEnumerable<IConsoleAppObserver> observers, IConsoleApp app, Exception ex)
        {
            Debug.Assert(observers != null);
            Debug.Assert(app != null);

            foreach (var observer in observers)
                observer.OnException(app, ex);
        }

        public static void Complete(this IEnumerable<IConsoleAppObserver> observers, IConsoleApp app)
        {
            Debug.Assert(observers != null);

            foreach (var observer in observers)
                observer.OnCompleted(app);
        }
    }
}
