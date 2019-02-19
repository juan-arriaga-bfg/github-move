namespace IW
{
    public class LoggerTransportUnityConsole : ILoggerTransport
    {
        public void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message.ToString());
        }
    }
}