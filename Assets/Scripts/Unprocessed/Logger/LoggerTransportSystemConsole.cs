public class LoggerTransportSystemConsole : ILoggerTransport
{
    public void Log(object message)
    {
        System.Console.WriteLine(message);
    }

    public void LogError(object message)
    {
        System.Console.WriteLine("[Error] " + message);
    }

    public void LogWarning(object message)
    {
        System.Console.WriteLine("[Warning] " + message);
    }
}