using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class RaveLeaderboardManager : MonoBehaviour {
	public RaveLeaderboardManager() {
	}

	public static List<RaveLeaderboard> Leaderboards {
		get {
			String s = RaveLeaderboardManagerLeaderboards();
			return RaveObject.DeserializeList<RaveLeaderboard>(s);
		}
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerLeaderboards() {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerLeaderboards();	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerLeaderboards() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerLeaderboards");
			return s;
		}
	}
	#endif

	public static void UpdateLeaderboards(RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateLeaderboards(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateLeaderboards(string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateLeaderboards(string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateLeaderboards(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateLeaderboards",args);
			
		}
	}
	#endif

	public static RaveLeaderboard GetLeaderboardByKey(string leaderboardkey) {
		RaveLeaderboard output = new RaveLeaderboard();
		output.Deserialize(RaveLeaderboardManagerGetLeaderboardByKey(leaderboardkey));
		return output;
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetLeaderboardByKey(string leaderboardkey) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetLeaderboardByKey(string leaderboardkey);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetLeaderboardByKey(string leaderboardkey) {
		object[] args = new object[1];
		args[0] = leaderboardkey;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetLeaderboardByKey",args);
			return s;
		}
	}
	#endif

	public static void UpdateLeaderboardByKey(string leaderboardKey, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateLeaderboardByKey(leaderboardKey, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateLeaderboardByKey(string leaderboardKey, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateLeaderboardByKey(string leaderboardKey, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateLeaderboardByKey(string leaderboardKey, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = leaderboardKey;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateLeaderboardByKey",args);
			
		}
	}
	#endif

	public static void SubmitScoreByKey(string leaderboardKey, float score, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerSubmitScoreByKey(leaderboardKey, score, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerSubmitScoreByKey(string leaderboardKey, float score, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerSubmitScoreByKey(string leaderboardKey, float score, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerSubmitScoreByKey(string leaderboardKey, float score, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = leaderboardKey;
		args[1] = score;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerSubmitScoreByKey",args);
			
		}
	}
	#endif

	public static void UpdateGlobalScoresByKey(string leaderboardKey, float page, float pageSize, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateGlobalScoresByKey(leaderboardKey, page, pageSize, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateGlobalScoresByKey(string leaderboardKey, float page, float pageSize, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateGlobalScoresByKey(string leaderboardKey, float page, float pageSize, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateGlobalScoresByKey(string leaderboardKey, float page, float pageSize, string callbackModule, string callbackName, string pid) {
		object[] args = new object[6];
		args[0] = leaderboardKey;
		args[1] = page;
		args[2] = pageSize;
		args[3] = callbackModule;
		args[4] = callbackName;
		args[5] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateGlobalScoresByKey",args);
			
		}
	}
	#endif

	public static List<RaveScore> GetGlobalScoresByKey(string leaderboardKey, float page, float pageSize) {
		return RaveObject.DeserializeList<RaveScore>(RaveLeaderboardManagerGetGlobalScoresByKey(leaderboardKey, page, pageSize));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetGlobalScoresByKey(string leaderboardKey, float page, float pageSize) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetGlobalScoresByKey(string leaderboardKey, float page, float pageSize);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetGlobalScoresByKey(string leaderboardKey, float page, float pageSize) {
		object[] args = new object[3];
		args[0] = leaderboardKey;
		args[1] = page;
		args[2] = pageSize;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetGlobalScoresByKey",args);
			return s;
		}
	}
	#endif

	public static List<RaveScore> GetFriendsScoresByKey(string leaderboardKey, float page, float pageSize) {
		return RaveObject.DeserializeList<RaveScore>(RaveLeaderboardManagerGetFriendsScoresByKey(leaderboardKey, page, pageSize));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetFriendsScoresByKey(string leaderboardKey, float page, float pageSize) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetFriendsScoresByKey(string leaderboardKey, float page, float pageSize);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetFriendsScoresByKey(string leaderboardKey, float page, float pageSize) {
		object[] args = new object[3];
		args[0] = leaderboardKey;
		args[1] = page;
		args[2] = pageSize;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetFriendsScoresByKey",args);
			return s;
		}
	}
	#endif

	public static void UpdateFriendsScoresByKey(string leaderboardKey, float page, float pageSize, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateFriendsScoresByKey(leaderboardKey, page, pageSize, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateFriendsScoresByKey(string leaderboardKey, float page, float pageSize, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateFriendsScoresByKey(string leaderboardKey, float page, float pageSize, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateFriendsScoresByKey(string leaderboardKey, float page, float pageSize, string callbackModule, string callbackName, string pid) {
		object[] args = new object[6];
		args[0] = leaderboardKey;
		args[1] = page;
		args[2] = pageSize;
		args[3] = callbackModule;
		args[4] = callbackName;
		args[5] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateFriendsScoresByKey",args);
			
		}
	}
	#endif

	public static void UpdateMyGlobalScoresByKey(string leaderboardKey, float pageSize, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateMyGlobalScoresByKey(leaderboardKey, pageSize, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateMyGlobalScoresByKey(string leaderboardKey, float pageSize, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateMyGlobalScoresByKey(string leaderboardKey, float pageSize, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateMyGlobalScoresByKey(string leaderboardKey, float pageSize, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = leaderboardKey;
		args[1] = pageSize;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateMyGlobalScoresByKey",args);
			
		}
	}
	#endif

	public static List<RaveScore> GetMyGlobalScoresByKey(string leaderboardKey, float pageSize) {
		return RaveObject.DeserializeList<RaveScore>(RaveLeaderboardManagerGetMyGlobalScoresByKey(leaderboardKey, pageSize));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetMyGlobalScoresByKey(string leaderboardKey, float pageSize) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetMyGlobalScoresByKey(string leaderboardKey, float pageSize);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetMyGlobalScoresByKey(string leaderboardKey, float pageSize) {
		object[] args = new object[2];
		args[0] = leaderboardKey;
		args[1] = pageSize;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetMyGlobalScoresByKey",args);
			return s;
		}
	}
	#endif

	public static List<RaveScore> GetMyFriendsScoresByKey(string leaderboardKey, float pageSize) {
		return RaveObject.DeserializeList<RaveScore>(RaveLeaderboardManagerGetMyFriendsScoresByKey(leaderboardKey, pageSize));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetMyFriendsScoresByKey(string leaderboardKey, float pageSize) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetMyFriendsScoresByKey(string leaderboardKey, float pageSize);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetMyFriendsScoresByKey(string leaderboardKey, float pageSize) {
		object[] args = new object[2];
		args[0] = leaderboardKey;
		args[1] = pageSize;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetMyFriendsScoresByKey",args);
			return s;
		}
	}
	#endif

	public static void UpdateMyFriendsScoresByKey(string leaderboardKey, float pageSize, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateMyFriendsScoresByKey(leaderboardKey, pageSize, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateMyFriendsScoresByKey(string leaderboardKey, float pageSize, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateMyFriendsScoresByKey(string leaderboardKey, float pageSize, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateMyFriendsScoresByKey(string leaderboardKey, float pageSize, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = leaderboardKey;
		args[1] = pageSize;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateMyFriendsScoresByKey",args);
			
		}
	}
	#endif

	public static List<RaveScore> GetMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent) {
		return RaveObject.DeserializeList<RaveScore>(RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(leaderboardKey, adjacent));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent) {
		object[] args = new object[2];
		args[0] = leaderboardKey;
		args[1] = adjacent;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey",args);
			return s;
		}
	}
	#endif

	public static void UpdateMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(leaderboardKey, adjacent, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(string leaderboardKey, float adjacent, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = leaderboardKey;
		args[1] = adjacent;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey",args);
			
		}
	}
	#endif

	public static List<RaveScore> GetMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent) {
		return RaveObject.DeserializeList<RaveScore>(RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(leaderboardKey, adjacent));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent) {
		object[] args = new object[2];
		args[0] = leaderboardKey;
		args[1] = adjacent;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey",args);
			return s;
		}
	}
	#endif

	public static void UpdateMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent, RaveCompletionCallback callback) {
		string pid = RaveCallbackManager.SetCallback(callback);
		RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(leaderboardKey, adjacent, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
	}
	#if UNITY_EDITOR
	private static void RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent, string callbackModule, string callbackName, string pid) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent, string callbackModule, string callbackName, string pid);	
	#elif UNITY_ANDROID
	private static void RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(string leaderboardKey, float adjacent, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = leaderboardKey;
		args[1] = adjacent;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			rsClass.CallStatic("RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey",args);
			
		}
	}
	#endif

	public static float GetHighScoreByKey(string leaderboardKey) {
		return Convert.ToSingle(RaveLeaderboardManagerGetHighScoreByKey(leaderboardKey));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetHighScoreByKey(string leaderboardKey) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetHighScoreByKey(string leaderboardKey);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetHighScoreByKey(string leaderboardKey) {
		object[] args = new object[1];
		args[0] = leaderboardKey;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetHighScoreByKey",args);
			return s;
		}
	}
	#endif

	public static float GetFriendsPositionByKey(string leaderboardKey) {
		return Convert.ToSingle(RaveLeaderboardManagerGetFriendsPositionByKey(leaderboardKey));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetFriendsPositionByKey(string leaderboardKey) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetFriendsPositionByKey(string leaderboardKey);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetFriendsPositionByKey(string leaderboardKey) {
		object[] args = new object[1];
		args[0] = leaderboardKey;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetFriendsPositionByKey",args);
			return s;
		}
	}
	#endif

	public static float GetGlobalPositionByKey(string leaderboardKey) {
		return Convert.ToSingle(RaveLeaderboardManagerGetGlobalPositionByKey(leaderboardKey));
	}
	#if UNITY_EDITOR
	private static string RaveLeaderboardManagerGetGlobalPositionByKey(string leaderboardKey) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveLeaderboardManagerGetGlobalPositionByKey(string leaderboardKey);	
	#elif UNITY_ANDROID
	private static string RaveLeaderboardManagerGetGlobalPositionByKey(string leaderboardKey) {
		object[] args = new object[1];
		args[0] = leaderboardKey;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveLeaderboardManagerGetGlobalPositionByKey",args);
			return s;
		}
	}
	#endif
}
