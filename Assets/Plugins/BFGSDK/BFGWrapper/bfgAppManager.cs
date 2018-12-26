using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// To enable debug message logging define "UNITY_DEBUG" as described in http://docs.unity3d.com/Manual/PlatformDependentCompilation.html

public class bfgAppManager
{
#if UNITY_EDITOR
	// Nothing to see here.

#elif UNITY_IOS || UNITY_IPHONE
	// Declarations for iOS only.
	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchApp(string bundleIdentifier);

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchAppWithParams(string bundleIdentifier, string parameterString);

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__isAppInstalled(string bundleIdentifier);

	[DllImport("__Internal")]
	private static extern void __bfgAppManager__launchStoreWithApp(string appID);

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__isBigFishGamesAppInstalled();

	[DllImport("__Internal")]
	private static extern void __bfgAppManager__launchStoreWithBigFishGamesApp();

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchOrInstallBigFishGamesApp();

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchBigFishGamesAppStrategyGuideWithWrappingID(string wrappingID);

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchBigFishGamesAppStrategyGuideWithWrappingIDChapterIndexPageIndex(string wrappingID, uint chapterIndex, uint pageIndex);

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__openReferralURL(string url);

	[DllImport("__Internal")]
	private static extern void __bfgAppManager__cancelCurrentReferral();

    // Big Fish iOS SDK 5.10

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchBigFishGamesAppWithForum();

	[DllImport("__Internal")]
	private static extern bool __bfgAppManager__launchBigFishGamesAppWithForumId(string id);

#endif

	//
	// ---------------------------------------
	//

	public static bool launchApp(string bundleIdentifier)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			bool result = __bfgAppManager__launchApp(bundleIdentifier);

			#if UNITY_DEBUG
				Debug.Log("bfgAppManager.__bfgAppManager__launchApp(" + bundleIdentifier + ") returned " + result);
			#endif

			return result;
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static bool launchAppWithParams(string bundleIdentifier, string parameterString)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			bool result = __bfgAppManager__launchAppWithParams(bundleIdentifier, parameterString);

			#if UNITY_DEBUG
				Debug.Log("bfgAppManager.__bfgAppManager__launchAppWithParams(" + bundleIdentifier + ", " + parameterString + ") returned " + result);
			#endif

			return result;
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static bool isAppInstalled(string bundleIdentifier)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__isAppInstalled(bundleIdentifier);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static void launchStoreWithApp(string appID)
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgAppManager__launchStoreWithApp(appID);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

	public static bool isBigFishGamesAppInstalled()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__isBigFishGamesAppInstalled();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static void launchStoreWithBigFishGamesApp()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgAppManager__launchStoreWithBigFishGamesApp();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

	public static bool launchOrInstallBigFishGamesApp()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__launchOrInstallBigFishGamesApp();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static bool launchBigFishGamesAppStrategyGuideWithWrappingID(string wrappingID)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__launchBigFishGamesAppStrategyGuideWithWrappingID(wrappingID);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static bool launchBigFishGamesAppStrategyGuideWithWrappingIDChapterIndexPageIndex(string wrappingID, uint chapterIndex, uint pageIndex)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__launchBigFishGamesAppStrategyGuideWithWrappingIDChapterIndexPageIndex(wrappingID, chapterIndex, pageIndex);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static bool openReferralURL(string url)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__openReferralURL(url);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static void cancelCurrentReferral()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgAppManager__cancelCurrentReferral();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

    // Big Fish iOS SDK 5.10

	public static bool launchBigFishGamesAppWithForum()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__launchBigFishGamesAppWithForum();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	public static bool launchBigFishGamesAppWithForum(string id)
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgAppManager__launchBigFishGamesAppWithForumId(id);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}
}
