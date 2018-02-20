using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLoggerComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid { get { return ComponentGuid; } }
    
    protected BoardController context;
    public virtual void OnRegisterEntity(ECSEntity entity) { this.context = entity as BoardController; }
    public virtual void OnUnRegisterEntity(ECSEntity entity) { }

    private bool isLoggingEnabled = false;

	public bool IsLoggingEnabled { get { return isLoggingEnabled; } }

    public virtual void Log(string log)
    {
        if (isLoggingEnabled == false) return;

#if UNITY_EDITOR
        string targetLog = "[GBC] => " + log;

        string path = Application.dataPath.Replace("Assets", "Logs");

        if (System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        string targetPath = path + "/gameboard.logic.log";

        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(targetPath, true, System.Text.Encoding.UTF8);
        streamWriter.Write(targetLog);
        streamWriter.Close();
#endif
    }

    
}
