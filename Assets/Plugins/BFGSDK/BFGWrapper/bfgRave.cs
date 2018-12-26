using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;



public class bfgRave
{
#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
    private static readonly int RAVE_ID = 32;
   	private static readonly int EMAIL_ADDR = 320;
    private static readonly int DISPLAY_NAME = 32;
#endif

#if UNITY_EDITOR
    // Nothing to see here.

#elif UNITY_IOS || UNITY_IPHONE
    [DllImport("__Internal")]
	private static extern void __bfgRave__currentRaveId( StringBuilder returnCurrentRaveId);

    [DllImport("__Internal")]
	private static extern void __bfgRave__lastRaveId( StringBuilder returnLastRaveId);

    [DllImport("__Internal")]
    private static extern bool __bfgRave__isLastGuest();

    [DllImport("__Internal")]
    private static extern void __bfgRave__logoutCurrentUser();

    [DllImport("__Internal")]
    private static extern void  __bfgRave__presentSignIn();

    [DllImport("__Internal")]
    private static extern void __bfgRave__presentProfile();

    [DllImport("__Internal")]
    private static extern bool __bfgRave__isRaveInitialized();

	[DllImport("__Internal")]
	private static extern bool __bfgRave__isAuthenticated();

	[DllImport("__Internal")]
	private static extern void __bfgRave__currentRaveEmail(StringBuilder returnCurrentRaveEmail);

    [DllImport("__Internal")]
    private static extern void __bfgRave__currentRaveDisplayName(StringBuilder returnCurrentRaveDisplayName);

	[DllImport("__Internal")]
	private static extern void __bfgRave__changeRaveDisplayName(string raveDisplayName);

	[DllImport("__Internal")]
	private static extern void __bfgRave__fetchCurrentAppDataKey();

	[DllImport("__Internal")]
	private static extern void __bfgRave__selectRaveAppDataKey(string key);

	[DllImport("__Internal")]
	private static extern void __bfgRave__fetchCurrentRaveProfilePicture();

	[DllImport("__Internal")]
	private static extern bool __bfgRave__isCurrentAuthenticated();

	[DllImport("__Internal")]
	private static extern bool __bfgRave__isCurrentPersonalized();

	[DllImport("__Internal")]
	private static extern void __bfgRave__presentNewsletterSignup();

	[DllImport("__Internal")]
	private static extern void __bfgRave__presentNewsletterSignupWithOrigin(string origin);

	[DllImport("__Internal")]
	private static extern void __bfgRave__presentProfileWithOrigin(string origin);

	[DllImport("__Internal")]
	private static extern void __bfgRave__presentSignInWithOrigin(string origin);

#endif


    //
    // ---------------------------------------
    //

    public static string currentRaveId ()
	{
		#if UNITY_EDITOR
		return "";
		#elif UNITY_IOS || UNITY_IPHONE
			StringBuilder returnCurrentRaveId = new StringBuilder(bfgRave.RAVE_ID);	
			__bfgRave__currentRaveId( returnCurrentRaveId );
			return returnCurrentRaveId.ToString();
		#elif UNITY_ANDROID
    		string result = "";
      		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<string>("currentRaveId");}
	    	return result;
		#else
      		return "";
		#endif
	}

	public static string currentRaveEmail ()
	{
		#if UNITY_EDITOR
		return "";
		#elif UNITY_IOS || UNITY_IPHONE
			StringBuilder returnCurrentRaveEmail = new StringBuilder(bfgRave.EMAIL_ADDR);
       		__bfgRave__currentRaveEmail( returnCurrentRaveEmail );
			return returnCurrentRaveEmail.ToString();
		#elif UNITY_ANDROID
    		throw new NotImplementedException();
		#else
      		return "";
		#endif
	}

	public static bool isAuthenticated ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgRave__isAuthenticated();
		#elif UNITY_ANDROID
			bool result = false;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<bool>("isCurrentAuthenticated");}
			return result;
		#else
			return false;
		#endif
	}

	public static bool isCurrentGuest ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
		//warning deprecated and removed method as of iOS SDK 6.3
			throw new NotImplementedException();
		#elif UNITY_ANDROID
       		 bool result = false;
			 using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<bool>("isCurrentGuest");}
        	return result;
		#else
			return false;
		#endif
	}

	public static string lastRaveId ()
	{
		#if UNITY_EDITOR
		return "";
		#elif UNITY_IOS || UNITY_IPHONE
			StringBuilder returnLastRaveId = new StringBuilder(bfgRave.RAVE_ID);
            __bfgRave__lastRaveId( returnLastRaveId);
			return returnLastRaveId.ToString();
		#elif UNITY_ANDROID
            string result = "";
            using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<string>("lastRaveId");}
            return result;
		#else
		return "";
		#endif
	}

	public static bool isLastGuest ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
	        return __bfgRave__isLastGuest();
		#elif UNITY_ANDROID
      bool result = false;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<bool>("isLastGuest");}
      return result;
		#else
			return false;
		#endif
	}

	public static void logoutCurrentUser ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	        __bfgRave__logoutCurrentUser();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("logoutCurrentUser");}
		#else
			return;
		#endif
	}


	public static void presentSignIn ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	        __bfgRave__presentSignIn();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("presentSignIn");}
		#else
			return;
		#endif
	}

	public static void presentProfile ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	        __bfgRave__presentProfile();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("presentProfile");}
		#else
			return;
		#endif
	}

	public static bool isRaveInitialized ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
	        return __bfgRave__isRaveInitialized();
		#elif UNITY_ANDROID
      bool result = false;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<bool>("isRaveInitialized");}
      return result;
		#else
			return false;
		#endif
	}

	public static void performCrossAppLogin ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
          throw new NotImplementedException();
		#elif UNITY_ANDROID
      using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("performCrossAppLogin");}
		#else
      return;
		#endif
	}

    public static string currentRaveDisplayName()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IOS || UNITY_IPHONE
        StringBuilder returnCurrentRaveDisplayName = new StringBuilder(bfgRave.DISPLAY_NAME);
        __bfgRave__currentRaveDisplayName(returnCurrentRaveDisplayName);
        return returnCurrentRaveDisplayName.ToString();
#elif UNITY_ANDROID
        string result = "";
        using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<string>("currentRaveDisplayName");}
        return result;
#else
      return "";
#endif
    }

    public static void changeRaveDisplayName (string raveDisplayName)
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
		__bfgRave__changeRaveDisplayName(raveDisplayName);
		#elif UNITY_ANDROID
      using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("changeRaveDisplayName", raveDisplayName);}
		#else
      return;
		#endif
	}

	public static void fetchCurrentAppDataKey ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgRave__fetchCurrentAppDataKey();
		#elif UNITY_ANDROID
        using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("fetchCurrentAppDataKey");}
		#else
			return;
		#endif
	}

	public static void selectRaveAppDataKey (string key)
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgRave__selectRaveAppDataKey(key);
		#elif UNITY_ANDROID
        using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("selectRaveAppDataKey", key);}
		#else
			return;
		#endif
	}         

	public static void fetchCurrentRaveProfilePicture ()
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

	public static bool isCurrentAuthenticated ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
	return __bfgRave__isCurrentAuthenticated();
#elif UNITY_ANDROID
	bool result = false;
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<bool>("isCurrentAuthenticated");}
	return result;
#else
	return false;
		#endif
	}

	public static bool isCurrentPersonalized ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
	return __bfgRave__isCurrentPersonalized();
#elif UNITY_ANDROID
	bool result = false;
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {result = ajc.CallStatic<bool>("isCurrentPersonalized");}
	return result;
#else
	return false;
		#endif
	}

	public static void presentNewsletterSignup ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	__bfgRave__presentNewsletterSignup();
	return;
#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("presentNewsletterSignup");}
#else
	return;
		#endif
	}

	public static void presentNewsletterSignupWithOrigin (string origin)
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	__bfgRave__presentNewsletterSignupWithOrigin(origin);
		return;
#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("presentNewsletterSignupWithOrigin", origin);}
#else
	return;
		#endif
	}

	public static void presentProfileWithOrigin (string origin)
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	__bfgRave__presentProfileWithOrigin(origin);
	return;
#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("presentProfileWithOrigin", origin);}
#else
	return;
		#endif
	}

	public static void presentSignInWithOrigin (string origin)
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
	__bfgRave__presentSignInWithOrigin(origin);
	return;
#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRaveUnityWrapper")) {ajc.CallStatic("presentSignInWithOrigin", origin);}
#else
	return;
		#endif
	}
}
