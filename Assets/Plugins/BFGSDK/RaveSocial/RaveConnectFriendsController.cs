using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RAVESDK
{
    public class RaveConnectFriendsController
    {
        private string plugin;
        private string processId;

        public RaveConnectFriendsController(string pluginName)
        {
            plugin = pluginName;
            RaveConnectFriendsControllerControllerWithPlugin(pluginName);
        }
#if UNITY_EDITOR
        private static void RaveConnectFriendsControllerControllerWithPlugin(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectFriendsControllerControllerWithPlugin(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectFriendsControllerControllerWithPlugin(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectFriendsControllerControllerWithPlugin",args);
		}
	}
#endif

        public void SetObserver(RaveConnectFriendsControllerCallback callback)
        {
            processId = RaveCallbackManager.SetCallback(callback);
            RaveConnectFriendsControllerSetObserver(plugin, RaveSocial.moduleName, "ParseRaveConnectFriendsControllerCallback", processId);
        }
#if UNITY_EDITOR
        private static void RaveConnectFriendsControllerSetObserver(string pluginName, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectFriendsControllerSetObserver(string pluginName,string callbackModule, string callbackName, string pid);
#elif UNITY_ANDROID
	private static void RaveConnectFriendsControllerSetObserver(string pluginName,string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = pluginName;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectFriendsControllerSetObserver",args);
		}
	}
#endif

        public void AttemptGetFriends()
        {
            RaveConnectFriendsControllerAttemptGetFriends(plugin);
        }
#if UNITY_EDITOR
        private static void RaveConnectFriendsControllerAttemptGetFriends(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectFriendsControllerAttemptGetFriends(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectFriendsControllerAttemptGetFriends(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectFriendsControllerAttemptGetFriends",args);
		}
	}
#endif

        public void AttemptForgetFriends()
        {
            RaveConnectFriendsControllerAttemptForgetFriends(plugin);
        }
#if UNITY_EDITOR
        private static void RaveConnectFriendsControllerAttemptForgetFriends(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectFriendsControllerAttemptForgetFriends(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectFriendsControllerAttemptForgetFriends(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectFriendsControllerAttemptForgetFriends",args);
		}
	}
#endif

        public void OnDestroy()
        {
            if (processId != null)
            {
                RaveCallbackManager.DeleteCallback(processId);
                RaveConnectFriendsControllerDelete(plugin);
            }
        }
#if UNITY_EDITOR
        private static void RaveConnectFriendsControllerDelete(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectFriendsControllerDelete(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectFriendsControllerDelete(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectFriendsControllerDelete",args);
		}
	}
#endif
    }
}