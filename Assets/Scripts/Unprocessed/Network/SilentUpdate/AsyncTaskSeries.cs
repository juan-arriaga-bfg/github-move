using System;
using System.Collections.Generic;

public class AsyncTaskSeries<T>
{
    private readonly List<T> failed = new List<T>();
    private readonly List<T> successful = new List<T>();

    private List<Action<Action<bool, T>>> actions = new List<Action<Action<bool, T>>>();

    private int remaining;

    private bool started;

    private bool completed;
     
    private ExecuteCallback callback;

    public int TasksCount => actions.Count;
     
    public delegate void ExecuteCallback(bool isOk, List<T> successful, List<T> failed);

    public void AddTask(Action<Action<bool, T>> action)
    {
        actions.Add(action);
    }
     
    public void Execute(ExecuteCallback callback)
    {
        if (completed || started)
        {
            IW.Logger.LogError($"[AsyncTaskSeries] => Execute: Already executed");
            return;
        }

        this.callback = callback;
         
        remaining = actions.Count;
        started = true;
         
        foreach (var action in actions)
        {
            action.Invoke(OnTaskCompleted);
        }
    }

    private void OnTaskCompleted(bool isOk, T result)
    {
        remaining--;

        if (isOk)
        {
            successful.Add(result);
        }
        else
        {
            failed.Add(result);
        }

        if (remaining == 0)
        {
            completed = true;
            callback.Invoke(failed.Count == 0, successful, failed);
        }
    }
}