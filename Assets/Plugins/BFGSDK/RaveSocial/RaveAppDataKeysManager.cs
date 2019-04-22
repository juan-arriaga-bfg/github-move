using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RAVESDK
{
    public delegate void RaveAppDataKeyStateObserver(string selectedkey, List<string> unresolvedKeys);
    public delegate void RaveAppDataKeyStateCallback(string selectedkey, List<string> rejectedKeys, List<string> unresolvedKeys, string error);
    public delegate void RaveAppDataKeyKeysCallback(List<string> keys, string error);
    public delegate void RaveAppDataKeySelectedCallback(string selectedkey, string error);
    public delegate void RaveAppDataKeyCallback(string key, string error);
    public delegate void RaveAppDataKeySetCallback(List<RaveAppDataKeyUserPair> key, string error);

    /**
     * Manages app data keys
     *
     * App data keys can be used to map cloud or other back-end storage data to a user account per-application.  Keys are based off of RaveIDs.
     * When a user is merged and both the source and target user of the merge have an active key for a game, the keys will enter
     * an unresolved state and no key will be selected.  This API provides the unresolved keys so that an application can provide
     * a selection, either automatically or via user input.  Once a key is selected, the remaining unresolved keys will become rejected.
     * Rejected keys can be selected at any time.
     *
     * Any key in the list of all available can be selected at any time.
     *
     */
    public class RaveAppDataKeysManager : MonoBehaviour
    {
        public RaveAppDataKeysManager()
        {
        }

        /**
         * Sets an instance of the RaveAppDataKeysStateObserver
         *
         * @param observer Observer for changes in status
         */
        public static void SetStateObserver(RaveAppDataKeyStateObserver observer)
        {
            string pid = RaveCallbackManager.SetCallback(observer);
            RaveAppDataKeysManagerSetStateObserver(RaveSocial.moduleName, "ParseAppDataKeyStateChange", pid);
        }

        /**
         * Cached last selected key, if any. When possible prefer key from state observer
         * @return Last selected app key or null
         */
        public static string LastSelectedKey()
        {
            return RaveAppDataKeysManagerLastSelectedKey();
        }

        /**
         * Retrieves selected, rejected and unresolved keys from the server
         *
         * Will return last known cached results if the server is unreachable
         *
         * @param callback The result callback
         */
        public static void FetchCurrentState(RaveAppDataKeyStateCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerFetchCurrentState(RaveSocial.moduleName, "ParseAppDataKeyStateCallback", pid);
        }

        /**
         * Retrieves a single combined list of all available keys from the server
         *
         * Will return last known cached results if the server is unreachable
         *
         * @param listener The result listener
         */
        public static void FetchAvailable(RaveAppDataKeyKeysCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerFetchAvailable(RaveSocial.moduleName, "ParseAppDataKeyKeysCallback", pid);
        }

        /**
         * Retrieves selected key from the server
         *
         * Will return last known cached results if the server is unreachable
         *
         * @param callback The result callback
         */
        public static void FetchSelected(RaveAppDataKeySelectedCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerFetchSelected(RaveSocial.moduleName, "ParseAppDataKeySelectedCallback", pid);
        }

        /**
         * Retrieves unresolved keys from the server
         *
         * Will return last known cached results if the server is unreachable
         *
         * @param callback The result callback
         */
        public static void FetchUnresolved(RaveAppDataKeyKeysCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerFetchUnresolved(RaveSocial.moduleName, "ParseAppDataKeyKeysCallback", pid);
        }

        /**
         * Sets a key as selected on the server
         *
         * Requires active network
         *
         * @param key The key to select, must be 32 character hex format
         * @param callback The result callback
         */
        public static void SelectKey(string key, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerSelectKey(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid, key);
        }

        /**
         * Deactivates a key
         *
         * Requires active network and only works for keys which are selected, rejected or unresolved
         *
         * Deactivated keys are removed from selected, rejected or unresolved lists but will remain in the list of available keys
         *
         * @param key The key to deactivate, must be 32 character hex format
         * @param callback The result callback
         */
        public static void DeactivateKey(string key, RaveCompletionCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerDeactivateKey(RaveSocial.moduleName, "ParseRaveCompletionCallback", pid, key);
        }

        /**
         *  Fetch the currently selected key for a specified user.
         *
         *  Requires active network and only works for keys which are selected
         *
         *  @param raveId   The user raveId to access their key
         *  @param callback Callback supplying the selected key or an error
         */
        public static void FetchUserKey(string raveId, RaveAppDataKeyCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerFetchUserKey(RaveSocial.moduleName, "ParseAppDataKeyCallback", pid, raveId);
        }

        /**
         *  Fetch a set of keys given an array of RaveIDs.
         *
         *  @param raveIds  The list of desired RaveIDs.
         *  @param listener Callback supplying a list of app data keys or an error
         */
        public static void FetchUserKeySet(List<string> raveIds, RaveAppDataKeySetCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            string ids = RaveObject.SerializeList(raveIds);
            RaveAppDataKeysManagerFetchUserKeySet(RaveSocial.moduleName, "ParseAppDataKeySetCallback", pid, ids);
        }

        /**
         *  Fetch the current user's contacts app data keys.
         *
         *  @param listener Callback supplying a list of app data keys or an error.
         */
        public static void FetchUserKeySetForContacts(RaveAppDataKeySetCallback callback)
        {
            string pid = RaveCallbackManager.SetCallback(callback);
            RaveAppDataKeysManagerFetchUserKeySetForContacts(RaveSocial.moduleName, "ParseAppDataKeySetCallback", pid);
        }

#if UNITY_EDITOR
        private static void RaveAppDataKeysManagerSetStateObserver(string moduleName, string callbackName, string pid) { }
        private static string RaveAppDataKeysManagerLastSelectedKey() { return null; }
        private static void RaveAppDataKeysManagerFetchCurrentState(string moduleName, string callbackName, string pid) { }
        private static void RaveAppDataKeysManagerFetchAvailable(string moduleName, string callbackName, string pid) { }
        private static void RaveAppDataKeysManagerFetchSelected(string moduleName, string callbackName, string pid) { }
        private static void RaveAppDataKeysManagerFetchUnresolved(string moduleName, string callbackName, string pid) { }
        private static void RaveAppDataKeysManagerSelectKey(string moduleName, string callbackName, string pid, string key) { }
        private static void RaveAppDataKeysManagerDeactivateKey(string moduleName, string callbackName, string pid, string key) { }
        private static void RaveAppDataKeysManagerFetchUserKey(string moduleName, string callbackName, string pid, string raveId) { }
        private static void RaveAppDataKeysManagerFetchUserKeySet(string moduleName, string callbackName, string pid, string raveIds) { }
        private static void RaveAppDataKeysManagerFetchUserKeySetForContacts(string moduleName, string callbackName, string pid) { }
#elif UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerSetStateObserver(string moduleName, string callbackName, string pid);

		[DllImport ("__Internal")]
		private static extern string RaveAppDataKeysManagerLastSelectedKey();

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchCurrentState(string moduleName, string callbackName, string pid);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchAvailable(string moduleName, string callbackName, string pid);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchSelected(string moduleName, string callbackName, string pid);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchUnresolved(string moduleName, string callbackName, string pid);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerSelectKey(string moduleName, string callbackName, string pid, string key);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerDeactivateKey(string moduleName, string callbackName, string pid, string key);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchUserKey(string moduleName, string callbackName, string pid, string raveId);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchUserKeySet(string moduleName, string callbackName, string pid, string raveIds);

		[DllImport ("__Internal")]
		private static extern void RaveAppDataKeysManagerFetchUserKeySetForContacts(string callbackModule, string callbackName, string pid);
#elif UNITY_ANDROID
		private static void RaveAppDataKeysManagerSetStateObserver(string moduleName, string callbackName, string pid) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerSetStateObserver", new object[] {moduleName, callbackName, pid});
			}
		}
		private static string RaveAppDataKeysManagerLastSelectedKey() {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				return rsClass.CallStatic<string>("RaveAppDataKeysManagerLastSelectedKey");
			}
    //removing the return because it's causing a unity error on building.
			//return null;
		}
		private static void RaveAppDataKeysManagerFetchCurrentState(string moduleName, string callbackName, string pid) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchCurrentState", new object[] {moduleName, callbackName, pid});
			}
		}
		private static void RaveAppDataKeysManagerFetchAvailable(string moduleName, string callbackName, string pid) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchAvailable", new object[] {moduleName, callbackName, pid});
			}
		}
		private static void RaveAppDataKeysManagerFetchSelected(string moduleName, string callbackName, string pid) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchSelected", new object[] {moduleName, callbackName, pid});
			}
		}
		private static void RaveAppDataKeysManagerFetchUnresolved(string moduleName, string callbackName, string pid) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchUnresolved", new object[] {moduleName, callbackName, pid});
			}
		}
		private static void RaveAppDataKeysManagerSelectKey(string moduleName, string callbackName, string pid, string key) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerSelectKey", new object[] {moduleName, callbackName, pid, key});
			}
		
		}
		private static void RaveAppDataKeysManagerDeactivateKey(string moduleName, string callbackName, string pid, string key) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerDeactivateKey", new object[] {moduleName, callbackName, pid, key});
			}
		}
		private static void RaveAppDataKeysManagerFetchUserKey(string moduleName, string callbackName, string pid, string raveId) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") ) 
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchUserKey", new object[] {moduleName, callbackName, pid, raveId});
			}
		}

		private static void RaveAppDataKeysManagerFetchUserKeySet(string moduleName, string callbackName, string pid, string raveIds) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchUserKeySet", new object[] {moduleName, callbackName, pid, raveIds});
			}
		}

		private static void RaveAppDataKeysManagerFetchUserKeySetForContacts(string moduleName, string callbackName, string pid) {
			using( AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics") )
			{
				rsClass.CallStatic("RaveAppDataKeysManagerFetchUserKeySetForContacts", new object[] {moduleName, callbackName, pid});
			}
		}

#endif

    }
}