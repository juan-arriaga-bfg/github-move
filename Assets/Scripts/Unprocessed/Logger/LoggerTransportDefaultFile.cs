using System;
using System.IO;

public class LoggerTransportDefaultFile : LoggerTransportCustomFile
{
    public LoggerTransportDefaultFile() 
    {
        string personalDir = Environment.SpecialFolder.MyDocuments.ToString();
        string pathToLog = Path.Combine(personalDir, "GameLogs");
        string fullPath  = Path.Combine(pathToLog, "Unity.log");

        if (!Directory.Exists(pathToLog))
        {
            Directory.CreateDirectory(pathToLog);
        }

        Init(fullPath, false);
    }
}