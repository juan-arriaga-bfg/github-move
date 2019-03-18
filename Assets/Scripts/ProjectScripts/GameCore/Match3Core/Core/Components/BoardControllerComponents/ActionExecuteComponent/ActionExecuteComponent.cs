using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class ActionExecuteComponent : ECSEntity, IECSSystem, IActionHistoryComponent
{
    protected ActionHistoryComponent actionHistoryComponent;
    public ActionHistoryComponent ActionHistory
    {
        get
        {
            if (actionHistoryComponent == null)
            {
                actionHistoryComponent = GetComponent<ActionHistoryComponent>(ActionHistoryComponent.ComponentGuid);
            }
            return actionHistoryComponent;
        }
    }
    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }

    protected BoardController context;

    public virtual void Execute()
    {
        if (context == null) return;

        context.ActionExecutor.PerformStep();
        
    }

    public object GetDependency()
    {
        return null;
    }

    public bool IsExecuteable()
    {
        return true;
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as BoardController;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private int stepIndex;

    private SortedDictionary<int, BetterList<IBoardAction>> actionsQueue = new SortedDictionary<int, BetterList<IBoardAction>>();

    private BetterList<IBoardAction> actionsQueueStep = new BetterList<IBoardAction>();
    
    private Dictionary<int, BetterList<IBoardAction>> actionsQueueTypes = new Dictionary<int, BetterList<IBoardAction>>();


    public int StepIndex
    {
        get { return stepIndex; }
    }

    public BetterList<IBoardAction> ActionsQueueStep
    {
        get { return actionsQueueStep; }
    }

    public virtual bool IsHasActionInQueueByType<T>()
    {
        for (int i = 0; i < actionsQueueStep.size; i++)
        {
            if (actionsQueueStep[i] is T)
            {
                return true;
            }
        }

        return false;
    }

    public virtual void AddAction(IBoardAction action, BoardActionMode mode = BoardActionMode.MultiMode, int threadIndex = 0)
    {
        if (actionsQueue.ContainsKey(threadIndex) == false)
        {
            actionsQueue.Add(threadIndex, new BetterList<IBoardAction>());
        }

        BetterList<IBoardAction> actionsTypes;

        if (actionsQueueTypes.TryGetValue(action.Guid, out actionsTypes))
        {
            if (((int) mode & (int) BoardActionMode.SingleMode) == (int) BoardActionMode.SingleMode && actionsTypes.size > 0)
            {
                return;
            }
        }
        else
        {
            actionsQueueTypes.Add(action.Guid, new BetterList<IBoardAction>());
        }
        
        actionsQueueTypes[action.Guid].Add(action);

        actionsQueue[threadIndex].Add(action);
        
#if UNITY_EDITOR  
        ActionHistory.RegisterStackTrace(action);
#endif
    }

    public virtual void AddActionOnTop(IBoardAction action, BoardActionMode mode = BoardActionMode.MultiMode, int threadIndex = 0)
    {
        if (actionsQueue.ContainsKey(threadIndex) == false)
        {
            actionsQueue.Add(threadIndex, new BetterList<IBoardAction>());
        }

        BetterList<IBoardAction> actionsTypes;

        if (actionsQueueTypes.TryGetValue(action.Guid, out actionsTypes))
        {
            if (((int) mode & (int) BoardActionMode.SingleMode) == (int) BoardActionMode.SingleMode && actionsTypes.size > 0)
            {
                return;
            }
        }
        else
        {
            actionsQueueTypes.Add(action.Guid, new BetterList<IBoardAction>());
        }
        
        actionsQueueTypes[action.Guid].Add(action);

        actionsQueue[threadIndex].Insert(0, action);
    }

    public virtual BetterList<IBoardAction> PrepareActions(SortedDictionary<int, BetterList<IBoardAction>> actions)
    {
        actionsQueueStep.Clear();
        
        foreach (var actionDef in actions)
        {
            var list = actionDef.Value;
            for (int i = 0; i < list.size; i++)
            {
                var action = list[i];
                actionsQueueStep.Add(action);
                
                // Debug.LogWarning($"PrepareActions => threadIndex:{actionDef.Key} index:{i} type:{action.GetType()}");
            }
        }
        
        return actionsQueueStep;
    }

    public virtual bool PerformAction(IBoardAction action)
    {
        bool state = false;

        if (context.Logger.IsLoggingEnabled)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            UnityEngine.Profiling.Profiler.BeginSample("Action_" + action.GetType().ToString());
            state = action.PerformAction(context);
            UnityEngine.Profiling.Profiler.EndSample();

            sw.Stop();
        }
        else
        {
#if UNITY_EDITOR
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            
            UnityEngine.Profiling.Profiler.BeginSample("Action_" + action.GetType().ToString());
            state = action.PerformAction(context);
            UnityEngine.Profiling.Profiler.EndSample();
#if UNITY_EDITOR  
            ActionHistory.RegisterAction(action, stepIndex, sw.ElapsedTicks);
#endif
            
#if UNITY_EDITOR
            sw.Stop();
#endif
            
        }

        return state;
    }

    public virtual void PerformStep()
    {
        if (context.Session.IsProcessing == false) return;

        if (actionsQueue.Count > 0)
        {
            stepIndex++;

            PrepareActions(actionsQueue);
            
            foreach (var actionsQueueList in actionsQueue)
            {
                actionsQueueList.Value.Clear();
            }
            
            foreach (var actionsQueueTypeList in actionsQueueTypes)
            {
                actionsQueueTypeList.Value.Clear();
            }
    
            for (int i = 0; i < actionsQueueStep.size; i++)
            {
                var action = actionsQueueStep[i];

                PerformAction(action);
            }

            context.RendererContext.PerformAnimations();
        }
    }

    
}