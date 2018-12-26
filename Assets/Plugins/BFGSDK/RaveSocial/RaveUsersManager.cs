using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class RaveUsersManager : MonoBehaviour {
	public RaveUsersManager() {
	}

	public static RaveUser GetUserById(string raveId) {
		RaveUser output = new RaveUser();
		output.Deserialize(RaveUsersManagerGetUserById(raveId));
		return output;
	}
	#if UNITY_EDITOR
	private static string RaveUsersManagerGetUserById(string raveId) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveUsersManagerGetUserById(string raveId);	
	#elif UNITY_ANDROID
	private static string RaveUsersManagerGetUserById(string raveId) {
		object[] args = new object[1];
		args[0] = raveId;
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveUsersManagerGetUserById",args);
			return s;
		}
	}
	#endif

	public static void UpdateUserById(string raveId, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerUpdateUserById(raveId, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerUpdateUserById(string raveId, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerUpdateUserById(string raveId, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerUpdateUserById(string raveId, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = raveId;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerUpdateUserById",args);	
		}
	}
	#endif

	public static RaveUser Current {
		get {
			RaveUser output = new RaveUser();
			String s = RaveUsersManagerCurrent();
			if (s == "NULL" || s == "") return null;
			output.Deserialize(s);
			return output;
		}
	}
	#if UNITY_EDITOR
	private static string RaveUsersManagerCurrent() {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveUsersManagerCurrent();	
	#elif UNITY_ANDROID
	private static string RaveUsersManagerCurrent() {
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveUsersManagerCurrent");
			return s;
		}
	}
	#endif

	public static void UpdateCurrent(RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerUpdateCurrent(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerUpdateCurrent(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerUpdateCurrent(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerUpdateCurrent(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerUpdateCurrent",args);
			
		}
	}
	#endif

	public static void PushUserChanges(RaveUserChanges changes, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerPushUserChanges(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid, changes.Serialize());
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerPushUserChanges(string callbackModule, string callbackName, string pid, string changes) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerPushUserChanges(string callbackModule, string callbackName, string pid, string changes);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerPushUserChanges(string callbackModule, string callbackName, string pid, string changes) {
		object[] args = new object[4];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;
		args[3] = changes;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerPushUserChanges",args);
			
		}
	}
	#endif

	public static void AddCurrentUserObserver(RaveCurrentUserObserver observer) {
		RaveUserChangedCallback callback = delegate(List<string> changes) {
			observer.UserChanged(changes);
		};

		string pid = RaveCallbackManager.SetCallback(callback);
		observer.CallbackId = pid;
		RaveUsersManagerAddCurrentUserObserver(RaveSocial.moduleName, "ParseRaveUserChangedCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerAddCurrentUserObserver(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerAddCurrentUserObserver(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerAddCurrentUserObserver(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerAddCurrentUserObserver",args);	
		}
	}
	#endif

	public static void RemoveCurrentUserObserver(RaveCurrentUserObserver observer) {
		RaveUsersManagerRemoveCurrentUserObserver(observer.CallbackId);
		RaveCallbackManager.DeleteCallback(observer.CallbackId);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerRemoveCurrentUserObserver(string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerRemoveCurrentUserObserver(string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerRemoveCurrentUserObserver(string pid) {
		object[] args = new object[1];
		args[0] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerRemoveCurrentUserObserver",args);
		}
	}
	#endif

	// Keys to push
	public static readonly String displayName = "displayName";
	public static readonly String realName = "realName";
	public static readonly String username = "username";
	public static readonly String email = "email";
	public static readonly String gender = "gender";
	public static readonly String birthdate = "birthdate";

	public static void PushChangesToCurrentUser(RaveUser user, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		// Turn rave user into dictionary for pushing
		Dictionary<string, string> userData = new Dictionary<string, string>();
		userData[displayName] = user.displayName;
		userData[realName] = user.realName;
		userData[username] = user.username;
		userData[email] = user.email;
		userData[gender] = user.gender;
		userData[birthdate] = user.birthdate;

		RavePushChangesToCurrentUser(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid, RaveObject.SerializeDict(userData));
	}
	#if UNITY_EDITOR
	private static void RavePushChangesToCurrentUser(string callbackModule, string callbackName, string pid, string changesDict) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RavePushChangesToCurrentUser(string callbackModule, string callbackName, string pid, string changesDict);	
	#elif UNITY_ANDROID
	private static void RavePushChangesToCurrentUser(string callbackModule, string callbackName, string pid, string changesDict) {
		object[] args = new object[4];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;
		args[3] = changesDict;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RavePushChangesToCurrentUser",args);
			
		}
	}
	#endif

	public static void CheckAccountExists(string email, RaveAccountExistsCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerCheckAccountExists(email, RaveSocial.moduleName, "ParseRaveAccountExistsCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerCheckAccountExists(string email, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerCheckAccountExists(string email, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerCheckAccountExists(string email, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = email;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerCheckAccountExists",args);
			
		}
	}
	#endif

	public static void CheckThirdPartyAccountExists(string email, RaveAccountExistsCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerCheckThirdPartyAccountExists(email, RaveSocial.moduleName, "ParseRaveAccountExistsCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerCheckThirdPartyAccountExists(string email, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerCheckThirdPartyAccountExists(string email, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerCheckThirdPartyAccountExists(string email, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = email;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerCheckThirdPartyAccountExists",args);
			
		}
	}
	#endif

	public static void FetchAccessToken(RaveAccessTokenCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerFetchAccessToken(RaveSocial.moduleName, "ParseRaveAccessTokenCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerFetchAccessToken(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerFetchAccessToken(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerFetchAccessToken(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerFetchAccessToken",args);
			
		}
	}
	#endif

	public static void FetchRandomUsersForApplication(RaveUsersCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerFetchRandomUsersForApplication(RaveSocial.moduleName, "ParseRaveUsersCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerFetchRandomUsersForApplication(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerFetchRandomUsersForApplication(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerFetchRandomUsersForApplication(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerFetchRandomUsersForApplication",args);
			
		}
	}
	#endif

	public static void FetchRandomUsersForApplication2(string appUuid, RaveUsersCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerFetchRandomUsersForApplication2(appUuid, RaveSocial.moduleName, "ParseRaveUsersCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerFetchRandomUsersForApplication2(string appUuid, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerFetchRandomUsersForApplication2(string appUuid, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerFetchRandomUsersForApplication2(string appUuid, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = appUuid;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerFetchRandomUsersForApplication2",args);
			
		}
	}
	#endif

	public static void FetchRandomUsersForApplication3(string appUuid, float excludeContacts, float limit, RaveUsersCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerFetchRandomUsersForApplication3(appUuid, excludeContacts, limit, RaveSocial.moduleName, "ParseRaveUsersCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerFetchRandomUsersForApplication3(string appUuid, float excludeContacts, float limit, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerFetchRandomUsersForApplication3(string appUuid, float excludeContacts, float limit, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerFetchRandomUsersForApplication3(string appUuid, float excludeContacts, float limit, string callbackModule, string callbackName, string pid) {
		object[] args = new object[6];
		args[0] = appUuid;
		args[1] = excludeContacts;
		args[2] = limit;
		args[3] = callbackModule;
		args[4] = callbackName;
		args[5] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerFetchRandomUsersForApplication3",args);
			
		}
	}
	#endif

	public static void FetchAllIdentities(RaveIdentitiesCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerFetchAllIdentities(RaveSocial.moduleName, "ParseRaveIdentitiesCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerFetchAllIdentities(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerFetchAllIdentities(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerFetchAllIdentities(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerFetchAllIdentities",args);
			
		}
	}
	#endif

	public static void FetchIdentitiesForApplication(RaveIdentitiesCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerFetchIdentitiesForApplication(RaveSocial.moduleName, "ParseRaveIdentitiesCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerFetchIdentitiesForApplication(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerFetchIdentitiesForApplication(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerFetchIdentitiesForApplication(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerFetchIdentitiesForApplication",args);
			
		}
	}
	#endif

	public static void PushProfilePicture(string url, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveUsersManagerPushProfilePicture(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid, url);
	}
	#if UNITY_EDITOR
	private static void RaveUsersManagerPushProfilePicture(string callbackModule, string callbackName, string pid, string url) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveUsersManagerPushProfilePicture(string callbackModule, string callbackName, string pid, string url);	
	#elif UNITY_ANDROID
	private static void RaveUsersManagerPushProfilePicture(string callbackModule, string callbackName, string pid, string url) {
		object[] args = new object[4];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;
		args[3] = url;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveUsersManagerPushProfilePicture",args);
		}
	}
	#endif
}

public interface RaveCurrentUserObserver {
	string CallbackId {
		get;
		set;
	}
	void UserChanged(List<string> changes);
}
