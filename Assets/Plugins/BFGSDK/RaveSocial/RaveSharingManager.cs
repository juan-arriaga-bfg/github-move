using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RAVESDK
{
    public class RaveSharingManager : MonoBehaviour
    {
        public RaveSharingManager()
        {
        }

        public static void PostToFacebook(string wallPost, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveSharingManagerPostToFacebook(wallPost, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveSharingManagerPostToFacebook(string wallPost, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSharingManagerPostToFacebook(string wallPost, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveSharingManagerPostToFacebook(string wallPost, string callbackModule, string callbackName, string pid) {
		object[] args = new object[4];
		args[0] = wallPost;
		args[1] = callbackModule;
		args[2] = callbackName;
		args[3] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSharingManagerPostToFacebook",args);
			
		}
	}
#endif

        public static void PostToFacebookWithImage(string wallPost, string image, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveSharingManagerPostToFacebookWithImage(wallPost, image, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveSharingManagerPostToFacebookWithImage(string wallPost, string image, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSharingManagerPostToFacebookWithImage(string wallPost, string image, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveSharingManagerPostToFacebookWithImage(string wallPost, string image, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = wallPost;
		args[1] = image;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSharingManagerPostToFacebookWithImage",args);
			
		}
	}
#endif

        public static void PostToGooglePlus(string postText, string imageURL, RaveCompletionCallback callback)
        {
            //TODO string-ify image here?
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveSharingManagerPostToGooglePlus(postText, imageURL, RaveSocial.moduleName, "ParseRaveCompletionCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveSharingManagerPostToGooglePlus(string postText, string imageURL, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSharingManagerPostToGooglePlus(string postText, string imageURL, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveSharingManagerPostToGooglePlus(string postText, string imageURL, string callbackModule, string callbackName, string pid) {
		object[] args = new object[5];
		args[0] = postText;
		args[1] = imageURL;
		args[2] = callbackModule;
		args[3] = callbackName;
		args[4] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSharingManagerPostToGooglePlus",args);
			
		}
	}
#endif

        public static void ShareWith(List<RaveContact> externalContacts, string subject, string message, RaveShareRequestCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            string contacts = RaveObject.SerializeList<RaveContact>(externalContacts);
            RaveSharingManagerShareWith(contacts, subject, message, RaveSocial.moduleName, "ParseRaveShareRequestCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveSharingManagerShareWith(string externalContacts, string subject, string message, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSharingManagerShareWith(string externalContacts, string subject, string message, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveSharingManagerShareWith(string externalContacts, string subject, string message, string callbackModule, string callbackName, string pid) {
		object[] args = new object[6];
		args[0] = externalContacts;
		args[1] = subject;
		args[2] = message;
		args[3] = callbackModule;
		args[4] = callbackName;
		args[5] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSharingManagerShareWith",args);
			
		}
	}
#endif

        public static void ShareWithViaPlugin(List<RaveContact> externalContacts, string pluginKeyName, string subject, string message, RaveShareRequestCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            string contacts = RaveObject.SerializeList<RaveContact>(externalContacts);
            RaveSharingManagerShareWithViaPlugin(contacts, pluginKeyName, subject, message, RaveSocial.moduleName, "ParseRaveShareRequestCallback", pid);
        }
#if UNITY_EDITOR
        private static void RaveSharingManagerShareWithViaPlugin(string externalContacts, string pluginKeyName, string subject, string message, string callbackModule, string callbackName, string pid) { }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSharingManagerShareWithViaPlugin(string externalContacts, string pluginKeyName, string subject, string message, string callbackModule, string callbackName, string pid);	
#elif UNITY_ANDROID
	private static void RaveSharingManagerShareWithViaPlugin(string externalContacts, string pluginKeyName, string subject, string message, string callbackModule, string callbackName, string pid) {
		object[] args = new object[7];
		args[0] = externalContacts;
		args[1] = pluginKeyName;
		args[2] = subject;
		args[3] = message;
		args[4] = callbackModule;
		args[5] = callbackName;
		args[6] = pid;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			rsClass.CallStatic("RaveSharingManagerShareWithViaPlugin",args);
			
		}
	}
#endif

        public static string GetExternalIdForShareInstall(string appCallUrl, string source)
        {
            return RaveSharingManagerGetExternalIdForShareInstall(appCallUrl, source);
        }
#if UNITY_EDITOR
        private static string RaveSharingManagerGetExternalIdForShareInstall(string appCallUrl, string source) { return ""; }
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveSharingManagerGetExternalIdForShareInstall(string appCallUrl, string source);	
#elif UNITY_ANDROID
	private static string RaveSharingManagerGetExternalIdForShareInstall(string appCallUrl, string source) {
		object[] args = new object[2];
		args[0] = appCallUrl;
		args[1] = source;

		using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
		{
			string s = rsClass.CallStatic<string>("RaveSharingManagerGetExternalIdForShareInstall",args);
			return s;
		}
	}
#endif


    }
}