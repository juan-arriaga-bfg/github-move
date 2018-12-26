using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class RaveAchievementsManager : MonoBehaviour {
	public RaveAchievementsManager() {
	}
	
	public static RaveAchievement GetAchievementByKey(string key) {
		RaveAchievement output = new RaveAchievement();
		output.Deserialize(RaveAchievementsManagerGetAchievementByKey(key));
		return output;
	}
	#if UNITY_EDITOR
	private static string RaveAchievementsManagerGetAchievementByKey(string key) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveAchievementsManagerGetAchievementByKey(string key);	
	#elif UNITY_ANDROID
	private static string RaveAchievementsManagerGetAchievementByKey(string key) {
		object[] args = new object[1];
		args[0] = key;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveAchievementsManagerGetAchievementByKey",args);
			return s;
		}
	}
	#endif

	public static List<RaveAchievement> Achievements { get {
			return RaveObject.DeserializeList<RaveAchievement>(RaveAchievementsManagerAchievements());
		} }
	#if UNITY_EDITOR
	private static string RaveAchievementsManagerAchievements() {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveAchievementsManagerAchievements();	
	#elif UNITY_ANDROID
	private static string RaveAchievementsManagerAchievements() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			return rsClass.CallStatic<string>("RaveAchievementsManagerAchievements");
		}
	}
	#endif

	public static void UpdateAchievements(RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveAchievementsManagerUpdateAchievements(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveAchievementsManagerUpdateAchievements(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveAchievementsManagerUpdateAchievements(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveAchievementsManagerUpdateAchievements(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveAchievementsManagerUpdateAchievements",args);
			
		}
	}
	#endif

	public static void UnlockAchievement(string achievementKey, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveAchievementsManagerUnlockAchievement(achievementKey, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveAchievementsManagerUnlockAchievement(string achievementKey, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveAchievementsManagerUnlockAchievement(string achievementKey, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveAchievementsManagerUnlockAchievement(string achievementKey, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = achievementKey;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveAchievementsManagerUnlockAchievement",args);
			
		}
	}
	#endif
}
