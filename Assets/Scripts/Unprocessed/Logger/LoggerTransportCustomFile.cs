using System.IO;

namespace IW
{
    public class LoggerTransportCustomFile : ILoggerTransport
    {
        private StreamWriter m_sw;
        private string m_filePath;

        public string FilePath
        {
            get { return m_filePath; }
        }

        protected LoggerTransportCustomFile()
        {

        }

        public LoggerTransportCustomFile(string fullPath, bool append = false)
        {
            Init(fullPath, append);
        }

        protected void Init(string fullPath, bool append)
        {
            m_filePath = fullPath;
            var dir = Path.GetDirectoryName(m_filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            m_sw = append ? File.AppendText(m_filePath) : File.CreateText(m_filePath);
        }

        private bool CheckInit()
        {
            return m_sw != null;
        }

        private void KillStreamWriter(StreamWriter sw)
        {
            if (sw != null)
            {
                sw.Flush();
                sw.Close();
            }
        }

        ~LoggerTransportCustomFile()
        {
            KillStreamWriter(m_sw);
        }

        public void Log(object message)
        {
            if (!CheckInit())
            {
                return;
            }

            m_sw.WriteLine(message);
            m_sw.Flush();
        }

        public void LogError(object message)
        {
            if (!CheckInit())
            {
                return;
            }

            m_sw.WriteLine("[Error] " + message);
            m_sw.Flush();
        }

        public void LogWarning(object message)
        {
            if (!CheckInit())
            {
                return;
            }

            m_sw.WriteLine("[Warning] " + message);
            m_sw.Flush();
        }
    }
}