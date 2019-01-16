using UnityEngine;

public static class NetworkUtils
{
    public static bool CheckInternetConnection(bool showErrorDialogAutomatically = false)
    {
#if !UNITY_EDITOR
        if (!bfgManager.checkForInternetConnectionAndAlert(false))
#else
        if (Application.internetReachability == NetworkReachability.NotReachable)
#endif
        {
            if (showErrorDialogAutomatically)
            {
                UIMessageWindowController.CreateNoInternetMessage();
            }

            return false;
        }

        return true;
    } 
}