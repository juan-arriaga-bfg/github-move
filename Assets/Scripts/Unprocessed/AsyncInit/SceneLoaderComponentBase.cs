using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public abstract class SceneLoaderComponentBase : AsyncInitComponentBase
{
    private AsyncOperation loadingOperation;
    protected abstract string SceneName { get;}
    
    public override float Progress
    {
        get
        {
            if (isCompleted)
            {
                return 1;
            }
            
            if (loadingOperation == null)
            {
                return 0;
            }
            
            if (loadingOperation.isDone)
            {
                return 1;
            }

            return loadingOperation.progress;
        }
    }

    public override void Execute()
    {
        // Proxy GO to run coroutine from non-monobehaviour
        var go = new GameObject();
        go.name = "[SceneLoaderComponentCoroutineExecutor]"; 
        var mb = go.AddComponent<IWBaseMonoBehaviour>();
        Object.DontDestroyOnLoad(go);
        
        mb. StartCoroutine(LoadSceneCoroutine(() =>
        {
            Object.Destroy(go);
            
            loadingOperation = null;
            
            isCompleted = true;
            OnComplete(this);
        }));
    }
    
    IEnumerator LoadSceneCoroutine(Action onComplete)
    {
        loadingOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        onComplete();
    }
}