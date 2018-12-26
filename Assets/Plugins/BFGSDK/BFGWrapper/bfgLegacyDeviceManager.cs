using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class bfgLegacyDeviceManager
{
#if UNITY_EDITOR
	// Nothing to see here.
#elif UNITY_IOS || UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern bool __bfgLegacyDeviceManager__checkForDeviceGeneration();

    [DllImport("__Internal")]
    private static extern bool __bfgLegacyDeviceManager__checkForDeviceGenerationDisplayAlert( bool displayAlert, string title, string message, string dismissButtonTitle );

    [DllImport("__Internal")]
    private static extern int __bfgLegacyDeviceManager__commonDeviceNameForIdentifier( string returnDeviceName, int bufferSize);

#endif


//
// ---------------------------------------
//

    public static bool checkForDeviceGeneration()
    {
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
	        return __bfgLegacyDeviceManager__checkForDeviceGeneration();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
    }

    public static bool checkForDeviceGenerationDisplayAlert(
        bool displayAlert,
        string title,
        string message,
        string dismissButtonTitle )
    {
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
	        return __bfgLegacyDeviceManager__checkForDeviceGenerationDisplayAlert( displayAlert, title, message, dismissButtonTitle );
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
    }

    public static int commonDeviceNameForIdentifier(
        string returnDeviceName,
        int bufferSize )
    {
		#if UNITY_EDITOR
			return 0;
		#elif UNITY_IOS || UNITY_IPHONE
	        return __bfgLegacyDeviceManager__commonDeviceNameForIdentifier( returnDeviceName, bufferSize );
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return 0;
		#endif
    }
}
