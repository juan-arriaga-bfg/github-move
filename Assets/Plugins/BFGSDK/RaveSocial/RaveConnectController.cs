using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RAVESDK
{
    public class RaveConnectController
    {
        private string plugin;
        private string processId;

        public RaveConnectController(string pluginName)
        {
            plugin = pluginName;
            RaveConnectControllerControllerWithPlugin(pluginName);
        }
#if UNITY_EDITOR
        private static void RaveConnectControllerControllerWithPlugin(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectControllerControllerWithPlugin(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectControllerControllerWithPlugin(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectControllerControllerWithPlugin",args);
		}
	}
#endif

        public void SetObserver(RaveConnectControllerCallback callback)
        {
            processId = RaveCallbackManager.SetCallback(callback);
            RaveConnectControllerSetObserver(plugin, RaveSocial.moduleName, "ParseRaveConnectControllerCallback", processId);
        }
#if UNITY_EDITOR
        private static void RaveConnectControllerSetObserver(string pluginName, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectControllerSetObserver(string pluginName,string callbackModule, string callbackName, string pid);
#elif UNITY_ANDROID
	private static void RaveConnectControllerSetObserver(string pluginName,string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = pluginName;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectControllerSetObserver",args);
		}
	}
#endif

        public void AttemptConnect()
        {
            RaveConnectControllerAttemptConnect(plugin);
        }
#if UNITY_EDITOR
        private static void RaveConnectControllerAttemptConnect(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectControllerAttemptConnect(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectControllerAttemptConnect(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectControllerAttemptConnect",args);
		}
	}
#endif

        public void AttemptDisconnect()
        {
            RaveConnectControllerAttemptDisconnect(plugin);
        }
#if UNITY_EDITOR
        private static void RaveConnectControllerAttemptDisconnect(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectControllerAttemptDisconnect(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectControllerAttemptDisconnect(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectControllerAttemptDisconnect",args);
		}
	}
#endif

        public void OnDestroy()
        {
            if (processId != null)
            {
                RaveCallbackManager.DeleteCallback(processId);
                RaveConnectControllerDelete(plugin);
            }
        }
#if UNITY_EDITOR
        private static void RaveConnectControllerDelete(string pluginName) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveConnectControllerDelete(string pluginName);
#elif UNITY_ANDROID
	private static void RaveConnectControllerDelete(string pluginName) {
		object[] args = new object[1];
		args[0] = pluginName;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveConnectControllerDelete",args);
		}
	}
#endif
    }
}