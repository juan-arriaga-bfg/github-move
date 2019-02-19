using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IW
{
    public static class Logger
    {
        public const string ENABLE_DEFINE = "LOG_ENABLED";
        
        public static bool IsEnabled = true;

        private static readonly List<ILoggerTransport> transports = new List<ILoggerTransport>();

        public static List<ILoggerTransport> Transports => transports;

        static Logger()
        {
            AddTransport(new LoggerTransportUnityConsole());
        }
        
        public static void AddTransport(ILoggerTransport transport)
        {
            transports.Add(transport);
        }

        public static void RemoveTransport(ILoggerTransport transport)
        {
            transports.Remove(transport);
        }

        public static void RemoveAllTransports()
        {
            transports.Clear();
        }

        public static bool IsTransportWithTypeAdded(Type loggerType)
        {
            return transports.Any(e => e.GetType() == loggerType);
        }
        
        public static ILoggerTransport GetTransport<T>()
        {
            return transports.FirstOrDefault(e => e is T);
        }

#region Simple  

        [Conditional(ENABLE_DEFINE)]
        public static void Log(object message)
        { 
            if (!IsEnabled) { return; }
            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].Log(message);
            }
        }

        [Conditional(ENABLE_DEFINE)]
        public static void LogError(object message)
        {
            if (!IsEnabled) { return; }
            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].LogError(message);
            }
        }

        [Conditional(ENABLE_DEFINE)]
        public static void LogWarning(object message)
        {   
            if (!IsEnabled) { return; }
            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].LogWarning(message);
            }
        }

#endregion

#region Format

        [Conditional(ENABLE_DEFINE)]
        public static void LogFormat(string format, params object[] args)
        {
            if (!IsEnabled) { return; }
            var mess = string.Format(format, args);
            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].Log(mess);
            }
        }

        [Conditional(ENABLE_DEFINE)]
        public static void LogWarningFormat(string format, params object[] args)
        {
            if (!IsEnabled) { return; }
            var mess = string.Format(format, args);
            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].LogWarning(mess);
            }
        }

        [Conditional(ENABLE_DEFINE)]
        public static void LogErrorFormat(string format, params object[] args)
        {
            if (!IsEnabled) { return; }
            var mess = string.Format(format, args);
            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].LogError(mess);
            }
        }

#endregion

#region Asserts

        [Conditional(ENABLE_DEFINE)]
        public static void Assert(bool condition, string message)
        {
            if (!IsEnabled) { return; }
            if (condition) { return; }

            for (int i = 0; i < transports.Count; ++i)
            {
                transports[i].LogError("Assert: " + message);
            }
        }

#endregion

    }
}
