using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class MainSceneLoaderComponent : AsyncInitComponentBase
{
    private AsyncOperation loadingOperation;

    public override float Progress
    {
        get
        {
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
        var mb = go.AddComponent<IWBaseMonoBehaviour>();
        Object.DontDestroyOnLoad(go);
        
        mb. StartCoroutine(LoadSceneCoroutine(() =>
        {
            Object.Destroy(go);
            isCompleted = true;
            OnComplete(this);
        }));
    }
    
    IEnumerator LoadSceneCoroutine(Action onComplete)
    {
        loadingOperation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        onComplete();
    }
}