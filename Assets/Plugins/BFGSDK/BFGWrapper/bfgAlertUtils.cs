//
//  bfgAlertUtils.cs
//
//  Created by Zira Cook on 10/12/18.
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class bfgAlertUtils
{
    //@Nullable final String title, @Nullable final DialogInterface.OnClickListener listener
    public static void showGenericErrorAlert(String title)
    {
#if UNITY_EDITOR
        // Nothing to see here!
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
			throw new NotImplementedException();
#elif UNITY_ANDROID && !(UNITY_IOS || UNITY_IPHONE || UNITY_EDITOR)
			throw new NotImplementedException();
			//using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgAlerUtilsUnityWrapper")) {ajc.CallStatic("showGenericErrorAlert", title);}
#else
    		// Nothing to see here!
#endif
    }

    //@Nullable final String title,@NonNull final String message,@Nullable final DialogInterface.OnClickListener listener
    public static void showCustomAlert  (String title, String message)
	{
#if UNITY_EDITOR
        // Nothing to see here!
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
			throw new NotImplementedException();
#elif UNITY_ANDROID
			throw new NotImplementedException();
			//using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgAlerUtilsUnityWrapper")) {ajc.CallStatic("showCustomAlert", title, message);}
#else
        // Nothing to see here!
#endif
    }

    //@NonNull final Activity activity,@NonNull final AlertDialog dialog
    public static void showWithFixedSizeText ()
	{
#if UNITY_EDITOR
        // Nothing to see here!
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
			throw new NotImplementedException();
#elif UNITY_ANDROID
			throw new NotImplementedException();
			//using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgAlerUtilsUnityWrapper")) {ajc.CallStatic("showWithFixedSizeText");}
#else
			// Nothing to see here!
#endif
    }

    //@NonNull final Activity activity,@NonNull final AlertDialog dialog
    public static void fixFontSize ()
	{
		#if UNITY_EDITOR
			// Void return
		#elif UNITY_IOS || UNITY_IPHONE
			throw new NotImplementedException();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
			//using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgAlerUtilsUnityWrapper")) {ajc.CallStatic("fixFontSize");}
		#else
			// Void return
		#endif
	}
}
