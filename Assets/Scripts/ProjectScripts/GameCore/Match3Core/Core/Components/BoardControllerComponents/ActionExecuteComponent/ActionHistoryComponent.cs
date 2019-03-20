using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class ActionHistoryComponent : IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }

    public virtual void OnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public List<ActionHistoryStep> actionsHistory = new List<ActionHistoryStep>();
    
    public Dictionary<IBoardAction, string> stackTraceActions = new Dictionary<IBoardAction, string>();

    private int lastIndex = 0;

    public void RegisterStackTrace(IBoardAction action)
    {
#if IW_FULL_ACTION_LOG
        if (stackTraceActions.ContainsKey(action)) return;
        
        stackTraceActions.Add(action, StackTraceUtility.ExtractStackTrace());
#endif
    }

    public void RegisterAction(IBoardAction action, int stepIndex, long timeDuration)
    {
        var actionHistoryStep = new ActionHistoryStep
        {
            StepIndex = stepIndex,
            ActionGuid = action.Guid,
            TimeDuration = timeDuration,
            ActionType = action.GetType().ToString()
        };
        actionsHistory.Add(actionHistoryStep);
        
#if IW_FULL_ACTION_LOG
        var actionLog = Newtonsoft.Json.JsonConvert.SerializeObject(actionHistoryStep, Formatting.None);
        
        string stackTrace;
        if (stackTraceActions.TryGetValue(action, out stackTrace))
        {
            IW.Logger.LogWarning($" ==> {actionLog} \n ===> {stackTrace}");
            stackTraceActions.Remove(action);
        }
        else
        {
            IW.Logger.LogWarning($" ==> {actionLog}");
        }
#endif
    }


    public bool IsExecuteable()
    {
        return true;
    }

    public void Execute()
    {
//        if (Input.GetKeyUp(KeyCode.L) && Input.GetKey(KeyCode.LeftShift))
//        {
//            SaveActions();
//        }
//        SaveActions();
    }

    public object GetDependency()
    {
        return null;
    }

    protected virtual void SaveActions()
    {
#if UNITY_EDITOR
        var stringBuilder = new StringBuilder();

        for (int i = lastIndex; i < actionsHistory.Count; i++)
        {
            if (actionsHistory[i].TimeDuration > 1000) // 0.1 ms
            {
                var actionLog = Newtonsoft.Json.JsonConvert.SerializeObject(actionsHistory[i], Formatting.None);

                stringBuilder.AppendLine(actionLog);
            }
        }
        lastIndex = actionsHistory.Count;

        string path = Application.dataPath.Replace("Assets", "Logs");

        if (System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        string targetPath = path + "/actions.history.log";

        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(targetPath, true, System.Text.Encoding.UTF8);
        streamWriter.Write(stringBuilder);
        streamWriter.Close();
#endif
    }
}