using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    internal static class ObservableConsoleAppExtensions
    {
        public static void OnStarting(this IEnumerable<IConsoleAppObserver> observers, IConsoleApp app)
        {
            Debug.Assert(observers != null);
            Debug.Assert(app != null);
            observers.OnEvent(observer => observer.OnStarting(app));
        }

        public static void OnException(this IEnumerable<IConsoleAppObserver> observers, IConsoleApp app, Exception ex)
        {
            Debug.Assert(observers != null);
            Debug.Assert(app != null);
            Debug.Assert(ex != null);
            observers.OnEvent(observer => observer.OnException(app, ex));
        }

        public static void OnCompleted(this IEnumerable<IConsoleAppObserver> observers, IConsoleApp app)
        {
            Debug.Assert(observers != null);
            Debug.Assert(app != null);
            observers.OnEvent(observer => observer.OnCompleted(app));
        }

        private static void OnEvent(this IEnumerable<IConsoleAppObserver> observers, Action<IConsoleAppObserver> callback)
        {
            Debug.Assert(observers != null);
            Debug.Assert(callback != null);

            foreach (var observer in observers)
                callback(observer);
        }
    }
}
