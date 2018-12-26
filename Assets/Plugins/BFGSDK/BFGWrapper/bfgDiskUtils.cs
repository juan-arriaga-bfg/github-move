using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class bfgDiskUtils {

    #if UNITY_EDITOR
        // Nothing to see here.
    #elif UNITY_IOS || UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern long __bfgDiskUtils__availableDiskSpace();
    #endif

    public static long availableDiskSpace()
    {
        long availableSpace;
        #if UNITY_EDITOR
            availableSpace = 0;
        #elif UNITY_IOS || UNITY_IPHONE
            availableSpace = __bfgDiskUtils__availableDiskSpace();
        #elif UNITY_ANDROID                           
            using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgDiskUtilsUnityWrapper")) {availableSpace = ajc.CallStatic<long>("availableDiskSpace");}
        #else
            availableSpace = 0;
        #endif
        return availableSpace;
    }

}
