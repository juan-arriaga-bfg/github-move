//#define USE_COROUTINE // Enable to wait for end of frame after every item

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AsyncInitManager :  IAsyncInitManager
{
#if DEBUG
    private readonly Dictionary<AsyncInitComponentBase, Stopwatch> stopwatches = new Dictionary<AsyncInitComponentBase, Stopwatch>();
#endif
    
    private readonly List<AsyncInitComponentBase> components = new List<AsyncInitComponentBase>();
    private readonly List<AsyncInitComponentBase> componentsInProgress = new List<AsyncInitComponentBase>();
    private readonly List<AsyncInitComponentBase> initedComponents = new List<AsyncInitComponentBase>();

    private Action onComplete;

#if USE_COROUTINE
    private MonoBehaviour coroutinesExecutor;
#endif
    
    public AsyncInitManager AddComponent(AsyncInitComponentBase component)
    {
        Type t = component.GetType();
        var existingComponent = GetComponent(t);
        if (existingComponent != null)
        {
            if (IsCompleted(t))
            {
                components.Remove(existingComponent);
                initedComponents.Remove(existingComponent);
            }
            else
            {
                Debug.LogError($"[AsyncInitManager] => Can't AddComponent({t}) - already in progress!");
                return this;
            }
        }

        components.Add(component);
        return this;
    }

    public void Run(Action onComplete)
    {
        this.onComplete = onComplete;
#if USE_COROUTINE        
        // Proxy GO to run coroutine from non-monobehaviour
        var go = new GameObject();
        coroutinesExecutor = go.AddComponent<IWBaseMonoBehaviour>();
        GameObject.DontDestroyOnLoad(go);

        coroutinesExecutor.StartCoroutine(ExecuteNext());
#else
        ExecuteNext();
#endif
    }

#if USE_COROUTINE
    private IEnumerator ExecuteNext()
#else
    private void ExecuteNext()
#endif
    {
        if (components.Count == 0)
        {
            onComplete?.Invoke();
            onComplete = null;

#if USE_COROUTINE
            GameObject.Destroy(coroutinesExecutor.gameObject);
            yield break;  
        }

        yield return new WaitForEndOfFrame();
#else
        }
#endif         
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

        
#if USE_COROUTINE        
        coroutinesExecutor.StartCoroutine(ExecuteNext());
#else
        ExecuteNext();
#endif
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

#if DEBUG
        Debug.Log($"[Loading] => Total progress: {initedComponents.Count}/{components.Count} - {(int)(totalProgress * 100)}%");
#endif
        
        return totalProgress;
    }

    public bool IsAllComponentsInited()
    {
        return components.Count == initedComponents.Count;
    }

    public AsyncInitComponentBase GetComponent(Type t)
    {
        var cmp = components.FirstOrDefault(e => e.GetType() == t);
        return cmp;
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