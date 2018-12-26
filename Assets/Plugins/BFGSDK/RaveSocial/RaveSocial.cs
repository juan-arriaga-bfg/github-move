using UnityEngine;
using System;
using System.Runtime.InteropServices;

public struct RaveUnityError
{
	public int code;
	public string domain;
}

public enum RaveLoginStatus
{
	RaveLoggedIn,
	RaveLoggedOut,
	RaveLoginError,
}

public enum RaveCALState
{
	Enabled,
	NoPermission,
	OptedOut,
	None,
}

public class RaveConstants
{
	public static string CONNECT_PLUGIN_FACEBOOK = "Facebook";
	public static string CONNECT_PLUGIN_GPLUS = "Google+";
	public static string CONNECT_PLUGIN_TWITTER = "Twitter";
}

public delegate void RaveMergeDecisionCallback (bool shouldMerge);
public delegate void RaveMergePolicy (RaveMergeUser targetUser, RaveMergeDecisionCallback callback);

public class RaveSocial : MonoBehaviour
{
	public static readonly string moduleName = "RaveSocialManager";

    private static GameObject instance;

	// To receive callbacks from RaveSocial methods, call this method as early in the game's lifecycle as possible. 
	public static void Initialize (RaveCompletionCallback callback = null)
	{
		// Check to make sure RaveSocial doesn't already exist
		if (instance == null)
		{
            instance = new GameObject (moduleName);
            instance.AddComponent<RaveSocial> ();
            instance.AddComponent<RaveCallbackManager> ();
			DontDestroyOnLoad (instance);
		}
		// No callback, replace with empty delegate
		if (callback == null)
		{
			callback = delegate(string error) {
			};
		}

        // InitializeRave should not be called. The BFG SDK will Initialize Rave itself, this could lead to an unhandled crash.
        //string pid = RaveCallbackManager.SetCallback(callback);
        //InitializeRave (RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
    }

	private static void InitializeRave (string callbackModule, string callbackName, string pid)
	{
		BigFishInitialize (RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void BigFishInitialize (string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void BigFishInitialize(string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	private static void BigFishInitialize(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using(AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.BigFishStatics")) {
			rsClass.CallStatic("BigFishInitialize",args);
		}
	}
	#endif

	public static void OnStart ()
	{
		Debug.Log ("RaveSocial.OnStart");
		#if UNITY_ANDROID
                new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics").CallStatic("RaveSocialOnStart");
		#endif
	}

	public static void OnStop ()
	{
		Debug.Log ("RaveSocial.OnStop");
		#if UNITY_ANDROID
                new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics").CallStatic("RaveSocialOnStop");
		#endif
	}

	public static void EnableDefaultMergePolicy ()
	{
		EnableBigFishMergePolicy ();
	}

	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void EnableBigFishMergePolicy ();
	#else
		private static void EnableBigFishMergePolicy() {

#if UNITY_ANDROID
				using(AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.BigFishStatics")) {
					rsClass.CallStatic("EnableBigFishMergePolicy");
				}
			#endif
		}
	#endif

	public static bool IsInitialized ()
	{
		return RaveSocialIsInitialized ();
	}
	#if UNITY_EDITOR
	static bool RaveSocialIsInitialized ()
	{
		return false;
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool RaveSocialIsInitialized ();

#elif UNITY_ANDROID
	public static bool RaveSocialIsInitialized() {
		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<bool>("RaveSocialIsInitialized");
	}
	#endif

	public static bool IsLoggedIn ()
	{
		return RaveSocialIsLoggedIn ();
	}
	#if UNITY_EDITOR
	static bool RaveSocialIsLoggedIn ()
	{
		return false;
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool RaveSocialIsLoggedIn ();

#elif UNITY_ANDROID
	public static bool RaveSocialIsLoggedIn() {
		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<bool>("RaveSocialIsLoggedIn");
	}
	#endif

	public static bool IsLoggedInAsGuest ()
	{
		return RaveSocialIsLoggedInAsGuest ();
	}
	#if UNITY_EDITOR
	static bool RaveSocialIsLoggedInAsGuest ()
	{
		return false;
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool RaveSocialIsLoggedInAsGuest ();

#elif UNITY_ANDROID
	public static bool RaveSocialIsLoggedInAsGuest() {
		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<bool>("RaveSocialIsLoggedInAsGuest");
	}
	#endif

	public static bool IsPersonalized ()
	{
		return RaveSocialIsPersonalized ();
	}
	#if UNITY_EDITOR
	static bool RaveSocialIsPersonalized ()
	{
		return false;
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool RaveSocialIsPersonalized ();

#elif UNITY_ANDROID
	public static bool RaveSocialIsPersonalized() {
		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<bool>("RaveSocialIsPersonalized");
	}
	#endif

	public static bool IsAuthenticated ()
	{
		return RaveSocialIsAuthenticated ();
	}
	#if UNITY_EDITOR
	static bool RaveSocialIsAuthenticated ()
	{
		return false;
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool RaveSocialIsAuthenticated ();

#elif UNITY_ANDROID
	public static bool RaveSocialIsAuthenticated() {
		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<bool>("RaveSocialIsAuthenticated");
	}
	#endif

	public static void LoginAsGuest (RaveCompletionCallback callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialLoginAsGuest (RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	public static void RaveSocialLoginAsGuest (string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialLoginAsGuest(string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	public static void RaveSocialLoginAsGuest(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSocialLoginAsGuest",args);
		}
	}
	#endif

	public static void LogOut (RaveCompletionCallback callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialLogOut (RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveSocialLogOut (string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSocialLogOut (string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	public static void RaveSocialLogOut (string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSocialLogOut", args);
		}
	}
	#endif

	public static void LoginWith (string pluginKeyName, RaveCompletionCallback callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialLoginWith (pluginKeyName, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	public static void RaveSocialLoginWith (string pluginKeyName, string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialLoginWith(string pluginKeyName, string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	public static void RaveSocialLoginWith(string pluginKeyName, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = pluginKeyName;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSocialLoginWith",args);
		}
	}
	#endif

	public static void ConnectTo (string pluginKeyName, RaveCompletionCallback callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialConnectTo (pluginKeyName, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	public static void RaveSocialConnectTo (string pluginKeyName, string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialConnectTo(string pluginKeyName, string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	public static void RaveSocialConnectTo(string pluginKeyName, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = pluginKeyName;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSocialConnectTo",args);
		}
	}
	#endif

	public static bool IsPluginReady (string pluginKeyName)
	{
		return RaveSocialIsPluginReady (pluginKeyName);
	}
	#if UNITY_EDITOR
	public static bool RaveSocialIsPluginReady (string pluginKeyName)
	{
		return false;
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern bool RaveSocialIsPluginReady(string pluginKeyName);

#elif UNITY_ANDROID
	public static bool RaveSocialIsPluginReady(string pluginKeyName) {
		object[] args = new object[1];
		args[0] = pluginKeyName;

		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<bool>("RaveSocialIsPluginReady",args);
	}
	#endif


	public static void CheckReadinessOf (string pluginKeyName, RaveReadinessCallback callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialCheckReadinessOf (pluginKeyName, RaveSocial.moduleName, "ParseRaveReadinessCallback", pid);
	}
	#if UNITY_EDITOR
	public static void RaveSocialCheckReadinessOf (string pluginKeyName, string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialCheckReadinessOf(string pluginKeyName, string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	public static void RaveSocialCheckReadinessOf(string pluginKeyName, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = pluginKeyName;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSocialCheckReadinessOf",args);
		}
	}
	#endif

	public static void DisconnectFrom (string pluginKeyName, RaveCompletionCallback callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialDisconnectFrom (pluginKeyName, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	public static void RaveSocialDisconnectFrom (string pluginKeyName, string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialDisconnectFrom(string pluginKeyName, string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	public static void RaveSocialDisconnectFrom(string pluginKeyName, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = pluginKeyName;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSocialDisconnectFrom",args);
		}
	}
	#endif

	public static bool HasError (string data)
	{
		return !String.IsNullOrEmpty (data);
	}

	public static bool LogError (string data)
	{
		if (HasError (data))
		{
			Debug.Log (data);
			return true;
		}
		else
			return false;
	}

	public static void SetIgnoreFacebookUrls (bool ignore)
	{
		RaveSocialSetIgnoreFacebookUrls (RaveObject.BoolToString (ignore));
	}
	#if UNITY_EDITOR
	public static void RaveSocialSetIgnoreFacebookUrls (string flag)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialSetIgnoreFacebookUrls(string flag);

#elif UNITY_ANDROID
	public static void RaveSocialSetIgnoreFacebookUrls(string flag) {
		// this is a nop on android
	}
	#endif

	public static RaveCALState GetCALState ()
	{
		#if UNITY_ANDROID
			AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
			string calString = rsClass.CallStatic<string>("RaveSocialGetCALState");
			switch (calString) {
				case "Enabled":
					return RaveCALState.Enabled;
				case "NoPermission":
					return RaveCALState.NoPermission;
				case "OptedOut":
					return RaveCALState.OptedOut;
				default:
					return RaveCALState.None;

			}
		#else
		return RaveCALState.Enabled;
		#endif
	}

	public static void NativeLog (string tag, string message)
	{
		#if UNITY_ANDROID
			AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
			rsClass.CallStatic("RaveSocialNativeLog", new object[] {tag, message});
		#else
		#endif
	}

	public static void SetMergePolicy (RaveMergePolicy callback)
	{
		string pid = RaveCallbackManager.SetCallback (callback);
		RaveSocialSetMergePolicy (RaveSocial.moduleName, "ParseRaveMergePolicyCallback", pid);
	}
	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RaveSocialSetMergePolicy (string callbackModule, string callbackName, string pid);
	#else
		public static void RaveSocialSetMergePolicy(string callbackModule, string callbackName, string pid) {

#if UNITY_ANDROID
				using (AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics")) {
					rsClass.CallStatic("RaveSocialSetMergePolicy", new object[] {callbackModule, callbackName, pid});
				}
			#endif
		}
	#endif

	public static string VolatileRaveId {
		get { return RaveSocialVolatileRaveId (); }
	}
	#if UNITY_EDITOR
	static string RaveSocialVolatileRaveId ()
	{
		return "";
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveSocialVolatileRaveId ();

#elif UNITY_ANDROID
	public static string RaveSocialVolatileRaveId() {
		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<string>("RaveSocialVolatileRaveId");
	}
	#endif

	// Alternate version of Show Login Scene that supports the Big Fish Sign Up callback instead
	public static void ShowLoginScene (BigFishSignUpSceneCallback callback)
	{
		// No callback, replace with empty delegate
		if (callback == null)
		{
			callback = delegate(RaveCallbackResult result, BigFishSignUpData data, string error) {
			};
		}
		string pid = RaveCallbackManager.SetCallback (callback);
		BigFishShowLoginScene (RaveSocial.moduleName, "ParseBigFishSignUpCallback", pid);
	}

	private static void BigFishShowLoginScene (string callbackModule, string callbackName, string pid)
	{
		Debug.Log ("WARNING - RaveSocial.BigFishShowLoginScene() is deprecated with this version of the BFG SDK. Using bfgRave.presentSignIn() instead. ");
		bfgRave.presentSignIn ();
	}

	// Alternate version of Show Account Info Scene that supports the Big Fish Sign Up callback instead
	public static void ShowAccountInfoScene (BigFishSignUpSceneCallback callback)
	{
		// No callback, replace with empty delegate
		if (callback == null)
		{
			callback = delegate(RaveCallbackResult result, BigFishSignUpData data, string error) {
			};
		}
		string pid = RaveCallbackManager.SetCallback (callback);
		BigFishShowAccountInfoScene (RaveSocial.moduleName, "ParseBigFishSignUpCallback", pid);
	}

	private static void BigFishShowAccountInfoScene (string callbackModule, string callbackName, string pid)
	{
		Debug.Log ("WARNING - RaveSocial.ShowAccountInfoScene() is deprecated with this version of the BFG SDK. Please use bfgRave.presentProfile() instead");
		bfgRave.presentProfile ();
	}


	[Obsolete]
	public static void ShowFindFriendsScreen (RaveSceneCallback callback = null)
	{
		ShowFindFriendsScene (callback);
	}

	public static void ShowFindFriendsScene (RaveSceneCallback callback = null)
	{
		// No callback, replace with empty delegate
		if (callback == null)
		{
			callback = delegate(string error) {
			};
		}
		string pid = RaveCallbackManager.SetCallback (callback);
		BigFishShowFindFriendsScene (RaveSocial.moduleName, "ParseRaveSceneCallback", pid);
	}
	#if UNITY_EDITOR
	private static void BigFishShowFindFriendsScene (string callbackModule, string callbackName, string pid)
	{
	}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void BigFishShowFindFriendsScene (string callbackModule, string callbackName, string pid);

#elif UNITY_ANDROID
	private static void BigFishShowFindFriendsScene(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.BigFishStatics") )
		{
			rsClass.CallStatic("BigFishShowFindFriendsScene",args);
		}
	}
	#endif

	public static void ShowSignUpEmailScene (BigFishSignUpSceneCallback callback = null)
	{
		// No callback, replace with empty delegate
		if (callback == null)
		{
			callback = delegate(RaveCallbackResult result, BigFishSignUpData data, string error) {

			};
		}
		string pid = RaveCallbackManager.SetCallback (callback);
		BigFishShowSignUpEmailScene (RaveSocial.moduleName, "ParseBigFishSignUpCallback", pid);
	}

	private static void BigFishShowSignUpEmailScene (string callbackModule, string callbackName, string pid)
	{
		Debug.Log ("WARNING - RaveSocial.BigFishShowSignUpEmailScene() is deprecated with this version of the BFG SDK. Please use bfgRave.presentSignIn() instead");
		bfgRave.presentSignIn ();
	}

	public static void RegisterCALEventListener (BigFishCALListener callback)
	{
		#if UNITY_ANDROID
			string pid = RaveCallbackManager.SetCallback(callback);
			RaveSocialRegisterCALEventListener(RaveSocial.moduleName, "ParseBigFishCALListener", pid);
		#endif
	}

	#if UNITY_ANDROID
	public static void RaveSocialRegisterCALEventListener(string callbackModule, string callbackName, string pid) {
		object[] args = new object[] {
			callbackModule,
			callbackName,
			pid
		};

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.BigFishStatics") )
		{
			rsClass.CallStatic("RaveSocialRegisterCALEventListener",args);
		}
	}
	#endif

	public static void CheckCrossAppLogin (RaveCompletionCallback callback)
	{
		#if UNITY_ANDROID
			string pid = RaveCallbackManager.SetCallback(callback);
			object[] args = new object[] {
				RaveSocial.moduleName,
				"ParseRaveCompletionCallback",
				pid
			};

			using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.BigFishStatics") )
			{
				rsClass.CallStatic("RaveSocialCheckCrossAppLogin",args);
			}
		#else
		callback (null);
		#endif
	}
}
