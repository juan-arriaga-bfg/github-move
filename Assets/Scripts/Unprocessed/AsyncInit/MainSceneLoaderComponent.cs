using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class MainSceneLoaderComponent : AsyncInitComponent
{
    private AsyncOperation loadingOperation;
    
    public override bool IsCompleted => loadingOperation != null && loadingOperation.isDone;

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

    public override void OnRegisterEntity(ECSEntity entity)
    {
        // Proxy GO to run coroutine from non-monobehaviour
        var go = new GameObject();
        var mb = go.AddComponent<IWBaseMonoBehaviour>();
        Object.DontDestroyOnLoad(go);
        
        mb. StartCoroutine(LoadSceneCoroutine(() => Object.Destroy(go)));
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