
using UnityEngine;

public static class TackleBoxEvents
{
    private static void Send(string id)
    {
        Debug.Log("TackleBoxEvents: Send: " + id);
        
#if UNITY_EDITOR
        // Nothing here
#else
        bfgGameReporting.logCustomPlacement(id);
#endif  
    }

    public static void SendLevelUp()
    {
        Send("level_up");
    }
}