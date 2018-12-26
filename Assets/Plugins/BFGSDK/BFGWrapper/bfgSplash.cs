using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Bfg splash. Deprecated as of 6.8 SDK. 
/// </summary>
public class bfgSplash
{
    [Obsolete("Method deprecated in BFG SDK")]
	public static void setNewsletterSent(bool sent)
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
            throw new NotImplementedException();
		#elif UNITY_ANDROID
            throw new NotImplementedException();
		#else
			return;
		#endif
	}

    [Obsolete("Method deprecated in BFG SDK")]
	public static bool getNewsletterSent()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
            throw new NotImplementedException();
		#elif UNITY_ANDROID
            throw new NotImplementedException();
		#else
			return false;
		#endif
	}

    [Obsolete("Method deprecated in BFG SDK")]
	public static string splashMenuImageNormal()
	{
		#if UNITY_EDITOR
			return null;
		#elif UNITY_IOS || UNITY_IPHONE
            throw new NotImplementedException();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return null;
		#endif
	}

    [Obsolete("Method deprecated in BFG SDK")]
	public static string splashMenuImageHighlighted()
	{
		#if UNITY_EDITOR
			return null;
		#elif UNITY_IOS || UNITY_IPHONE
            throw new NotImplementedException();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return null;
		#endif
	}

//	public static UIBUTTON createSplashButton()
//	{
//		#if UNITY_EDITOR
//			return null;
//		#elif UNITY_IOS || UNITY_IPHONE
//			return __bfgSplash__createSplashButton();
//		#elif UNITY_ANDROID
//			throw new NotImplementedException();
//		#endif
//	}
}
