using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public enum RaveContactsFilter {
	RaveContactsFilterIncludeAll,
	RaveContactsFilterIncludeOnlyUsingApp,
	RaveContactsFilterExcludeUsingApp,
}

public class RaveContactsManager : MonoBehaviour {
	public RaveContactsManager() {
	}
	
	public static List<RaveContact> All { get {
			return RaveObject.DeserializeList<RaveContact>(RaveContactsManagerAll());
		} }
	#if UNITY_EDITOR
	private static string RaveContactsManagerAll() {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveContactsManagerAll();	
	#elif UNITY_ANDROID
	private static string RaveContactsManagerAll() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveContactsManagerAll");
			return s;
		}
	}
	#endif

	public static void UpdateAll(RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerUpdateAll(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerUpdateAll(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerUpdateAll(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerUpdateAll(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerUpdateAll",args);
			
		}
	}
	#endif

	public static List<RaveContact> AllUsingThisApplication { get {
			return RaveObject.DeserializeList<RaveContact>(RaveContactsManagerAllUsingThisApplication());
		} }
	#if UNITY_EDITOR
	private static string RaveContactsManagerAllUsingThisApplication() {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveContactsManagerAllUsingThisApplication();	
	#elif UNITY_ANDROID
	private static string RaveContactsManagerAllUsingThisApplication() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveContactsManagerAllUsingThisApplication");
			return s;
		}
	}
	#endif

	public static void UpdateAllUsingThisApplication(RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerUpdateAllUsingThisApplication(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerUpdateAllUsingThisApplication(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerUpdateAllUsingThisApplication(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerUpdateAllUsingThisApplication(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerUpdateAllUsingThisApplication",args);
			
		}
	}
	#endif

	public static List<RaveContact> Facebook { get {
			return RaveObject.DeserializeList<RaveContact>(RaveContactsManagerFacebook());
		} }
	#if UNITY_EDITOR
	private static string RaveContactsManagerFacebook() {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveContactsManagerFacebook();	
	#elif UNITY_ANDROID
	private static string RaveContactsManagerFacebook() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveContactsManagerFacebook");
			return s;
		}
	}
	#endif

	public static void UpdateFacebook(RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerUpdateFacebook(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerUpdateFacebook(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerUpdateFacebook(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerUpdateFacebook(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerUpdateFacebook",args);
			
		}
	}
	#endif

	public static void AddContactsByUsername(string usernames, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerAddContactsByUsername(usernames, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerAddContactsByUsername(string usernames, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerAddContactsByUsername(string usernames, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerAddContactsByUsername(string usernames, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = usernames;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerAddContactsByUsername",args);
			
		}
	}
	#endif

	public static void DeleteContact(string userUuid, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerDeleteContact(userUuid, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerDeleteContact(string userUuid, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerDeleteContact(string userUuid, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerDeleteContact(string userUuid, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = userUuid;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerDeleteContact",args);
			
		}
	}
	#endif

	public static List<RaveContact> GetAllUsingApplication(string appId) {
		return RaveObject.DeserializeList<RaveContact>(RaveContactsManagerGetAllUsingApplication(appId));
	}
	#if UNITY_EDITOR
	private static string RaveContactsManagerGetAllUsingApplication(string appId) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveContactsManagerGetAllUsingApplication(string appId);	
	#elif UNITY_ANDROID
	private static string RaveContactsManagerGetAllUsingApplication(string appId) {
		object[] args = new object[1];
		args[0] = appId;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveContactsManagerGetAllUsingApplication",args);
			return s;
		}
	}
	#endif

	public static void UpdateAllUsingApplication(string appId, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerUpdateAllUsingApplication(appId, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerUpdateAllUsingApplication(string appId, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerUpdateAllUsingApplication(string appId, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerUpdateAllUsingApplication(string appId, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = appId;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerUpdateAllUsingApplication",args);
			
		}
	}
	#endif

	public static void FetchAllExternal(RaveContactsFilter filter, RaveContactsCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerFetchAllExternal(filter, RaveSocial.moduleName, "ParseRaveContactsCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerFetchAllExternal(RaveContactsFilter filter, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerFetchAllExternal(RaveContactsFilter filter, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerFetchAllExternal(RaveContactsFilter filter, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = (float)filter;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerFetchAllExternal",args);
			
		}
	}
	#endif

	public static void FetchExternalFrom(string pluginKeyName, RaveContactsFilter filter, RaveContactsCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveContactsManagerFetchExternalFrom(pluginKeyName, filter, RaveSocial.moduleName, "ParseRaveContactsCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveContactsManagerFetchExternalFrom(string pluginKeyName, RaveContactsFilter filter, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveContactsManagerFetchExternalFrom(string pluginKeyName, RaveContactsFilter filter, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveContactsManagerFetchExternalFrom(string pluginKeyName, RaveContactsFilter filter, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = pluginKeyName;
		args[1] = (float)filter;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveContactsManagerFetchExternalFrom",args);
			
		}
	}
	#endif
}
