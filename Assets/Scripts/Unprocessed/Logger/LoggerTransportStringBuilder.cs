using System.Text;

public class LoggerTransportStringBuilder : ILoggerTransport
{
    private StringBuilder m_sb = new StringBuilder();

    public string GetLog()
    {
        return m_sb.ToString();
    }

    public void Log(object message)
    {
        m_sb.AppendLine(message.ToString());
    }

    public void LogError(object message)
    {
        m_sb.AppendLine("[Error] " + message);
    }

    public void LogWarning(object message)
    {
        m_sb.AppendLine("[Warning] " + message);
    }
}