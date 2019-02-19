public interface ILoggerTransport
{
    void Log(object message);
    void LogError(object message);
	void LogWarning(object message);
}