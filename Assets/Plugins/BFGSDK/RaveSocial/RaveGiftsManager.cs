using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RAVESDK
{
    public class RaveGiftsManager : MonoBehaviour
    {
        public RaveGiftsManager()
        {
        }

        public static RaveGiftType GetGiftTypeByKey(string typeKey)
        {
            RaveGiftType output = new RaveGiftType();
            output.Deserialize(RaveGiftsManagerGetGiftTypeByKey(typeKey));
            return output;
        }
#if UNITY_EDITOR
        private static string RaveGiftsManagerGetGiftTypeByKey(string typeKey) { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveGiftsManagerGetGiftTypeByKey(string typeKey);	
#elif UNITY_ANDROID
	private static string RaveGiftsManagerGetGiftTypeByKey(string typeKey) {
		object[] args = new object[1];
		args[0] = typeKey;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveGiftsManagerGetGiftTypeByKey",args);
			return s;
		}
	}
#endif

        public static List<RaveGiftType> GiftTypes
        {
            get
            {
                return RaveObject.DeserializeList<RaveGiftType>(RaveGiftsManagerGiftTypes());
            }
        }
#if UNITY_EDITOR
        private static string RaveGiftsManagerGiftTypes() { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveGiftsManagerGiftTypes();	
#elif UNITY_ANDROID
	private static string RaveGiftsManagerGiftTypes() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveGiftsManagerGiftTypes");
			return s;
		}
	}
#endif

        public static void UpdateGiftTypes(RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerUpdateGiftTypes(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerUpdateGiftTypes(string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerUpdateGiftTypes(string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerUpdateGiftTypes(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerUpdateGiftTypes",args);
		}
	}
#endif

        public static RaveGift GetGiftById(string giftId)
        {
            RaveGift output = new RaveGift();
            output.Deserialize(RaveGiftsManagerGetGiftById(giftId));
            return output;
        }
#if UNITY_EDITOR
        private static string RaveGiftsManagerGetGiftById(string giftId) { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveGiftsManagerGetGiftById(string giftId);	
#elif UNITY_ANDROID
	private static string RaveGiftsManagerGetGiftById(string giftId) {
		object[] args = new object[1];
		args[0] = giftId;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveGiftsManagerGetGiftById",args);
			return s;
		}
	}
#endif

        public static RaveGiftRequest GetGiftRequestById(string requestId)
        {
            RaveGiftRequest output = new RaveGiftRequest();
            output.Deserialize(RaveGiftsManagerGetGiftRequestById(requestId));
            return output;
        }
#if UNITY_EDITOR
        private static string RaveGiftsManagerGetGiftRequestById(string requestId) { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveGiftsManagerGetGiftRequestById(string requestId);	
#elif UNITY_ANDROID
	private static string RaveGiftsManagerGetGiftRequestById(string requestId) {
		object[] args = new object[1];
		args[0] = requestId;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveGiftsManagerGetGiftRequestById",args);
			return s;
		}
	}
#endif

        public static List<RaveGift> Gifts
        {
            get
            {
                return RaveObject.DeserializeList<RaveGift>(RaveGiftsManagerGifts());
            }
        }
#if UNITY_EDITOR
        private static string RaveGiftsManagerGifts() { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveGiftsManagerGifts();	
#elif UNITY_ANDROID
	private static string RaveGiftsManagerGifts() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveGiftsManagerGifts");
			return s;
		}
	}
#endif

        public static void UpdateGifts(RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerUpdateGifts(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerUpdateGifts(string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerUpdateGifts(string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerUpdateGifts(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerUpdateGifts",args);
			
		}
	}
#endif

        public static List<RaveGiftRequest> GiftRequests
        {
            get
            {
                return RaveObject.DeserializeList<RaveGiftRequest>(RaveGiftsManagerGiftRequests());
            }
        }
#if UNITY_EDITOR
        private static string RaveGiftsManagerGiftRequests() { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveGiftsManagerGiftRequests();	
#elif UNITY_ANDROID
	private static string RaveGiftsManagerGiftRequests() {
		
		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveGiftsManagerGiftRequests");
			return s;
		}
	}
#endif

        public static void UpdateGiftRequests(RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerUpdateGiftRequests(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerUpdateGiftRequests(string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerUpdateGiftRequests(string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerUpdateGiftRequests(string callbackModule, string callbackName, string pid) {
		object[] args = new object[3];
		args[0] = callbackModule;
		args[1] = callbackName;
		args[2] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerUpdateGiftRequests",args);
			
		}
	}
#endif

        public static void SendGiftsToUsersWithKey(string giftTypeKey, List<String> userIds, RaveGiftResultCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerSendGiftsToUsersWithKey(giftTypeKey, RaveObject.SerializeList(userIds), RaveSocial.moduleName, "ParseRaveGiftResultCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerSendGiftsToUsersWithKey(string giftTypeKey, string userIds, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerSendGiftsToUsersWithKey(string giftTypeKey, string userIds, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerSendGiftsToUsersWithKey(string giftTypeKey, string userIds, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = giftTypeKey;
		args[1] = userIds;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerSendGiftsToUsersWithKey",args);
			
		}
	}
#endif

        public static void AcceptGiftId(string giftId, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerAcceptGiftId(giftId, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerAcceptGiftId(string giftId, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerAcceptGiftId(string giftId, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerAcceptGiftId(string giftId, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = giftId;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerAcceptGiftId",args);
			
		}
	}
#endif

        public static void RejectGiftById(string giftId, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerRejectGiftById(giftId, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerRejectGiftById(string giftId, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerRejectGiftById(string giftId, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerRejectGiftById(string giftId, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = giftId;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerRejectGiftById",args);
			
		}
	}
#endif

        public static void RequestGiftWithKey(string giftTypeKey, List<String> userIds, RaveGiftResultCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerRequestGiftWithKey(giftTypeKey, RaveObject.SerializeList(userIds), RaveSocial.moduleName, "ParseRaveGiftResultCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerRequestGiftWithKey(string giftTypeKey, string userIds, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerRequestGiftWithKey(string giftTypeKey, string userIds, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerRequestGiftWithKey(string giftTypeKey, string userIds, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = giftTypeKey;
		args[1] = userIds;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerRequestGiftWithKey",args);
			
		}
	}
#endif

        public static void GrantGiftRequestById(string requestId, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerGrantGiftRequestById(requestId, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerGrantGiftRequestById(string requestId, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerGrantGiftRequestById(string requestId, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerGrantGiftRequestById(string requestId, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = requestId;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerGrantGiftRequestById",args);
			
		}
	}
#endif

        public static void IgnoreGiftRequestById(string requestId, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerIgnoreGiftRequestById(requestId, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerIgnoreGiftRequestById(string requestId, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerIgnoreGiftRequestById(string requestId, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerIgnoreGiftRequestById(string requestId, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = requestId;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerIgnoreGiftRequestById",args);
			
		}
	}
#endif

        public static void SendGifts(string giftTypeId, List<String> userIds, RaveGiftResultCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveGiftsManagerSendGifts(giftTypeId, RaveObject.SerializeList(userIds), RaveSocial.moduleName, "ParseRaveGiftResultCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveGiftsManagerSendGifts(string giftTypeId, string userIds, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveGiftsManagerSendGifts(string giftTypeId, string userIds, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveGiftsManagerSendGifts(string giftTypeId, string userIds, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = giftTypeId;
		args[1] = userIds;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveGiftsManagerSendGifts",args);
			
		}
	}
#endif
    }
}