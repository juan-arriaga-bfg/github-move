using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoaderComponent : AsyncInitComponent
{
    private AsyncOperation loadingOperation;
    
    public override bool IsCompleted => true;

    public override float Progress
    {
        get
        {
            if (IsCompleted)
            {
                return 1;
            }

            if (loadingOperation == null)
            {
                return 0;
            }

            return loadingOperation.progress;
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        loadingOperation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
    }
}