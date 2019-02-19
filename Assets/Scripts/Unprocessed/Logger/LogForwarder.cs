namespace IW
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

#if UNITY_EDITOR
    using UnityEditor;

#endif

    public class LogForwarder
    {
        private int m_logsCount = 2;
        private readonly string m_path;
        private FileStream m_fileStream;
        private readonly UTF8Encoding m_encoding = new UTF8Encoding(true);
        private static readonly string s_destDir = Path.Combine(Application.persistentDataPath, "GameLogs");

        private static LogForwarder s_instance;

        public static LogForwarder Instance
        {
            get { return s_instance ?? (s_instance = new LogForwarder()); }
        }

        private LogForwarder()
        {
            Application.logMessageReceived += HandleLog;

#if UNITY_EDITOR
            EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
#endif

            var fileName = string.Format("Unity_{0:s}", DateTime.Now).Replace(":", "-").Replace("T", "_") + ".log";
            m_path = Path.Combine(s_destDir, fileName);

            if (!Directory.Exists(s_destDir))
            {
                Directory.CreateDirectory(s_destDir);
            }
            else
            {
                ClearOldLogs();
            }

            CreateStream();
        }

        private void CreateStream()
        {
            if (m_fileStream == null)
            {
                m_fileStream = new FileStream(m_path, FileMode.Append);
            }
        }

#if UNITY_EDITOR
        private void PlaymodeStateChanged()
        {
            if (!Application.isPlaying)
            {
                Destroy();
            }
        }
#endif

        private void ClearOldLogs()
        {
            List<string> filePaths = GetLogFiles();

            int countToDel = filePaths.Count - m_logsCount;

            for (; countToDel >= 0; countToDel--)
            {
                File.Delete(filePaths[0]);
                filePaths.RemoveAt(0);
            }
        }

        public static List<string> GetLogFiles()
        {
            var filePaths = Directory.GetFiles(s_destDir, "Unity_*.log").ToList();
            filePaths.Sort();

            return filePaths;
        }


        private void Destroy()
        {
            Application.logMessageReceived -= HandleLog;

#if UNITY_EDITOR
            EditorApplication.playmodeStateChanged -= PlaymodeStateChanged;
#endif

            CloseStream();
            s_instance = null;
        }

        public void CloseStream()
        {
            if (m_fileStream != null)
            {
                m_fileStream.Close();
                m_fileStream = null;
            }
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            CreateStream();

            string mess;
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Warning:
                case LogType.Exception:
                    mess = string.Format("[{0}] {1}\n{2}\n", type, logString, stackTrace);
                    break;

                default:
                    mess = string.Format("[{0}] {1}\n", type, logString);
                    break;
            }

            byte[] data = m_encoding.GetBytes(mess);
            m_fileStream.Write(data, 0, data.Length);
            m_fileStream.Flush();
        }
    }
}