using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngineInternal;

namespace Neskinsoft.Core
{
    public static class Logger
    {
        public static bool IsEnabled = true;

        private static List<ILoggerTransport> m_transports = new List<ILoggerTransport>();

        public static List<ILoggerTransport> Transports
        {
            get { return m_transports; }
        }

        public static void AddTransport(ILoggerTransport transport)
        {
            m_transports.Add(transport);
        }

        public static void RemoveTransport(ILoggerTransport transport)
        {
            m_transports.Remove(transport);
        }

        public static void RemoveAllTransports()
        {
            m_transports.Clear();
        }

        public static bool IsLoggerWithTypeAdded(Type loggerType)
        {
            return m_transports.Any(e => e.GetType() == loggerType);
        }

#region Simple  

        public static void Log(object message)
        { 
#if DEBUG
            if (!IsEnabled) { return; }
            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].Log(message);
            }
#endif
        }

        public static void LogError(object message)
        {
#if DEBUG
            if (!IsEnabled) { return; }
            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].LogError(message);
            }
#endif
        }

        public static void LogWarning(object message)
        {   
#if DEBUG
            if (!IsEnabled) { return; }
            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].LogWarning(message);
            }
#endif
        }

#endregion

#region Format

        public static void LogFormat(string format, params object[] args)
        {
#if DEBUG
            if (!IsEnabled) { return; }
            var mess = string.Format(format, args);
            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].Log(mess);
            }
#endif
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
#if DEBUG
            if (!IsEnabled) { return; }
            var mess = string.Format(format, args);
            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].LogWarning(mess);
            }
#endif
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
#if DEBUG
            if (!IsEnabled) { return; }
            var mess = string.Format(format, args);
            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].LogError(mess);
            }
#endif
        }

#endregion

#region Asserts

        public static void Assert(bool condition, string message)
        {
#if DEBUG
            if (!IsEnabled) { return; }
            if (condition) { return; }

            for (int i = 0; i < m_transports.Count; ++i)
            {
                m_transports[i].LogError("Assert: " + message);
            }
#endif
        }

#endregion

    }
}
