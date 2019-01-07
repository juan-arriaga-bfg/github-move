using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class AsyncInitManager :  IAsyncInitManager
{
#if DEBUG
    private readonly Dictionary<AsyncInitItemBase, Stopwatch> stopwatches = new Dictionary<AsyncInitItemBase, Stopwatch>();
#endif
    
    private readonly List<AsyncInitItemBase> components = new List<AsyncInitItemBase>();
    private readonly List<AsyncInitItemBase> componentsInProgress = new List<AsyncInitItemBase>();
    private readonly List<AsyncInitItemBase> initedComponents = new List<AsyncInitItemBase>();

    public AsyncInitManager AddItem(AsyncInitItemBase component)
    {
        components.Add(component);
        return this;
    }

    public void Run()
    {
        ExecuteNext();
    }

    private void ExecuteNext()
    {
        if (components.Count == 0)
        {
            return;
        }

        List<AsyncInitItemBase> componentsToExecute = new List<AsyncInitItemBase>();
        
        foreach (var cmp in components)
        {
            if (initedComponents.Contains(cmp))
            {
                continue;
            }            
            
            if (componentsInProgress.Contains(cmp))
            {
                continue;
            }

            if (IsDependenciesExecuted(cmp))
            {
                componentsToExecute.Add(cmp); 
            }
        }

        foreach (var cmp in componentsToExecute)
        {
            componentsInProgress.Add(cmp);
            cmp.OnComplete += OnComponentComplete;
        }

        // One more loop to handle immediate call ExecuteNext from cmp.Execute()
        foreach (var cmp in componentsToExecute)
        {
            Debug.Log($"[Loading] => {cmp.GetType()} load...");
            
#if DEBUG
            var sw = new Stopwatch();
            stopwatches.Add(cmp, sw);
            sw.Start();
#endif
            
            cmp.Execute();
        }
    }

    private bool IsDependenciesExecuted(AsyncInitItemBase cmp)
    {
        foreach (var dependency in cmp.Dependencies)
        {
            if (initedComponents.All(e => e.GetType() != dependency))
            {
                return false;
            }
        }

        return true;
    }

    private void OnComponentComplete(AsyncInitItemBase cmp)
    {
#if DEBUG
        var sw = stopwatches[cmp];
        sw.Stop();

        Debug.Log($"[Loading] => {cmp.GetType()} loaded in {sw.ElapsedMilliseconds}ms");

        stopwatches.Remove(cmp);
#endif

        initedComponents.Add(cmp);

        cmp.OnComplete -= OnComponentComplete;
        componentsInProgress.Remove(cmp);

        ExecuteNext();
    }

    public float GetTotalProgress()
    {
        float total = 0;
        float progress = 0;
        foreach (AsyncInitItemBase item in components)
        {
            total += item.WeightForProgressbar;
            progress += (item.Progress * item.WeightForProgressbar);

            if ((int) (item.Progress) < 1)
            {
                Debug.Log($"[Loading] => Component progress: [{item.GetType()}]: {(int)(item.Progress * 100)}%");
            }
        }

        float totalProgress = progress / total;

        Debug.Log($"[Loading] => Total progress: {(int)(totalProgress * 100)}%");
        
        return totalProgress;
    }

    public bool IsAllComponentsInited()
    {
        return components.Count == initedComponents.Count;
    }
}