#if UNITY_IOS
namespace IW
{
    using System.Runtime.InteropServices;

    public class LoggerTransportNSLog : ILoggerTransport
    {
        [DllImport("__Internal")]
        private static extern void __LogMessageUsingNSLog(string messge);
        
        public void Log(object message)
        {
            __LogMessageUsingNSLog(message.ToString());
        }

        public void LogError(object message)
        {
            __LogMessageUsingNSLog("[Error] " + message);
        }

        public void LogWarning(object message)
        {
            __LogMessageUsingNSLog("[Warning] " + message);
        }
    }
}
#endif