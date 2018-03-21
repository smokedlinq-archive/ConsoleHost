using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    internal static class ObservableConsoleHostExtensions
    {
        public static void OnStarting(this IEnumerable<IConsoleHostObserver> observers)
        {
            Debug.Assert(observers != null);
            observers.OnEvent(observer => observer.OnStarting());
        }

        public static void OnException(this IEnumerable<IConsoleHostObserver> observers, Exception ex)
        {
            Debug.Assert(observers != null);
            Debug.Assert(ex != null);
            observers.OnEvent(observer => observer.OnException(ex));
        }

        public static void OnCompleted(this IEnumerable<IConsoleHostObserver> observers)
        {
            Debug.Assert(observers != null);
            observers.OnEvent(observer => observer.OnCompleted());
        }

        private static void OnEvent(this IEnumerable<IConsoleHostObserver> observers, Action<IConsoleHostObserver> callback)
        {
            Debug.Assert(observers != null);
            Debug.Assert(callback != null);

            foreach (var observer in observers)
                callback(observer);
        }
    }
}
