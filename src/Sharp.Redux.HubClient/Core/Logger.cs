using System;
using System.Diagnostics;

namespace Sharp.Redux.HubClient.Core
{
    public static class Logger
    {
        const string name = "SharpReduxSender";
        public static void Log(LogLevel level, string message)
        {
            if (level >= SharpReduxSender.Settings.LogLevel)
            {
                Trace.WriteLine($"{name} {level}:{message}");
            }
        }
        public static void Log(LogLevel level, Exception ex, string message)
        {
            if (level > SharpReduxSender.Settings.LogLevel)
            {
                Trace.WriteLine($"{name} {level}:{message}");
                var current = ex;
                string tabs = "";
                do
                {
                    Trace.WriteLine($"{tabs}{ex.Message}");
                    Trace.WriteLine($"{tabs}{ex.StackTrace}");
                    current = current.InnerException;
                    tabs += "  ";
                } while (current != null);
            }
        }
    }
}
