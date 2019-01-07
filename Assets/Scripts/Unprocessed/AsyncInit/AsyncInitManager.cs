using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class AsyncInitManager :  IAsyncInitManager
{
#if DEBUG
    private readonly Dictionary<AsyncInitComponentBase, Stopwatch> stopwatches = new Dictionary<AsyncInitComponentBase, Stopwatch>();
#endif
    
    private readonly List<AsyncInitComponentBase> components = new List<AsyncInitComponentBase>();
    private readonly List<AsyncInitComponentBase> componentsInProgress = new List<AsyncInitComponentBase>();
    private readonly List<AsyncInitComponentBase> initedComponents = new List<AsyncInitComponentBase>();

    public AsyncInitManager AddComponent(AsyncInitComponentBase component)
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

        List<AsyncInitComponentBase> componentsToExecute = new List<AsyncInitComponentBase>();
        
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

    private bool IsDependenciesExecuted(AsyncInitComponentBase cmp)
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

    private void OnComponentComplete(AsyncInitComponentBase cmp)
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
        foreach (AsyncInitComponentBase item in components)
        {
            total += item.WeightForProgressbar;
            progress += (item.Progress * item.WeightForProgressbar);

#if DEBUG
            if ((int) (item.Progress) < 1)
            {
                if (!IsDependenciesExecuted(item))
                {
                    string deps = string.Join(",", item.Dependencies
                                                       .Where(e => !IsCompleted(e))
                                                       .Select(e => e.ToString())
                                                       .ToList());
                    
                    Debug.Log($"[Loading] => Component: [{item.GetType()}]: Wait for dependencies: {deps}");
                }
                else
                {
                    Debug.Log($"[Loading] => Component progress: [{item.GetType()}]: {(int)(item.Progress * 100)}%");
                }
            }
#endif
        }

        float totalProgress = progress / total;

        Debug.Log($"[Loading] => Total progress: {initedComponents.Count}/{components.Count} - {(int)(totalProgress * 100)}%");
        
        return totalProgress;
    }

    public bool IsAllComponentsInited()
    {
        return components.Count == initedComponents.Count;
    }

    public T GetComponent<T>() where T : AsyncInitComponentBase
    {
        var cmp = components.FirstOrDefault(e => e.GetType() == typeof(T));
        return cmp as T;
    }
    
    public bool IsCompleted<T>() where T : AsyncInitComponentBase
    {
        var cmp = initedComponents.FirstOrDefault(e => e.GetType() == typeof(T));
        return cmp != null;
    }
    
    public bool IsCompleted(Type t)
    {
        var cmp = initedComponents.FirstOrDefault(e => e.GetType() == t);
        return cmp != null;
    }
}