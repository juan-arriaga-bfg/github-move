using System.Collections.Generic;

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

    private List<IBoardAction> actionsQueue = new List<IBoardAction>();

    private List<IBoardAction> actionsQueueStep = new List<IBoardAction>();


    public int StepIndex
    {
        get { return stepIndex; }
    }

    public List<IBoardAction> ActionsQueueStep
    {
        get { return actionsQueueStep; }
    }

    public virtual bool IsHasActionInQueueByType<T>()
    {
        for (int i = 0; i < actionsQueueStep.Count; i++)
        {
            if (actionsQueueStep[i] is T)
            {
                return true;
            }
        }

        return false;
    }

    public virtual void AddAction(IBoardAction action, BoardActionMode mode = BoardActionMode.MultiMode)
    {
        if (((int) mode & (int) BoardActionMode.SingleMode) == (int) BoardActionMode.SingleMode)
        {
            for (int i = 0; i < actionsQueue.Count; i++)
            {
                if (actionsQueue[i].GetType() == action.GetType())
                {
                    return;
                }
            }
        }

        actionsQueue.Add(action);
    }

    public virtual void AddActionOnTop(IBoardAction action, BoardActionMode mode = BoardActionMode.MultiMode)
    {
        if (((int) mode & (int) BoardActionMode.SingleMode) == (int) BoardActionMode.SingleMode)
        {
            for (int i = 0; i < actionsQueue.Count; i++)
            {
                if (actionsQueue[i].GetType() == action.GetType())
                {
                    return;
                }
            }
        }

        actionsQueue.Insert(0, action);
    }

    public virtual List<IBoardAction> PrepareActions(List<IBoardAction> actions)
    {
        var actionsQueueCopy = new List<IBoardAction>(actions);

        return actionsQueueCopy;
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

            var actionsQueueCopy = PrepareActions(actionsQueue);
            this.actionsQueueStep = actionsQueueCopy;
            actionsQueue.Clear();

            for (int i = 0; i < actionsQueueCopy.Count; i++)
            {
                var action = actionsQueueCopy[i];

                PerformAction(action);
            }

            context.RendererContext.PerformAnimations();
        }
    }

    
}