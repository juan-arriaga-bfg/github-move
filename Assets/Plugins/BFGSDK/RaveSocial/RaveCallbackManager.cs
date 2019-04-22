using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RAVESDK
{
    public class RaveCallbackManager : MonoBehaviour
    {
        // Prefix for parse functions
        public const string PARSE_FUNC_PREFIX = "Parse";
        public const string NO_CALLBACK = "NO_CALLBACK";

        private static System.Object lockThis = new System.Object();
        // Queue for storing callbacks associated by GUID
        private static Dictionary<string, System.Delegate> callbackQueue = new Dictionary<string, System.Delegate>();

        // Store a callback in the queue, generate and return a unique ID for it
        public static string SetCallback(System.Delegate del)
        {
            if (del == null)
                return NO_CALLBACK;

            lock (lockThis)
            {
                string id = System.Guid.NewGuid().ToString();
                callbackQueue.Add(id, del);
                return id;
            }
        }

        // Retrieve a callback delegate by ID and remove it from the queue
        // This function should only be called be the delegate-specific parse functions below
        private static System.Delegate GetCallback(string id)
        {
            if (id == NO_CALLBACK)
                return null;

            lock (lockThis)
            {
                System.Delegate del = callbackQueue[id];

                if (del == null)
                {
                    Debug.Log("RaveCallbackManager.cs: No callback queued for PID [" + id + "]");
                }
                else
                {
                    callbackQueue.Remove(id);
                }
                return del;
            }
        }

        // Retrieve a callback delegate by ID but keep it in the queue
        // This function should only be called in the delegate-specific parse functions below
        private static System.Delegate TriggerCallback(string id)
        {
            if (id == NO_CALLBACK)
                return null;

            lock (lockThis)
            {
                System.Delegate del = callbackQueue[id];

                if (del == null)
                {
                    Debug.Log("RaveCallbackManager.cs: No callback queued for PID [" + id + "]");
                }
                return del;
            }
        }

        // Delete a stored callback
        public static void DeleteCallback(string id)
        {
            lock (lockThis)
            {
                callbackQueue.Remove(id);
            }
        }

        // Special case for login status callback, there is only one and it's global
        public static RaveLoginStatusCallback loginStatusCallback;

        // Utility function to parse out ID from data sent to the parse function
        private string[] ParseIDandData(string inputData)
        {
            //Debug.Log("ParseIDandData: " + inputData);
            if (inputData == null)
            {
                Debug.Log("RaveCallbackManager.cs: inputData is null (errors will follow)");
                // This will result in no callback and prevent crash
                return new string[1] { NO_CALLBACK };
            }
            return inputData.Split('|'); // [ id, actual_data ]
        }
        // Delegate-specific functions - Called by SendMessage from objC/Java

        void ParseRaveCompletionCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveCompletionCallback del = (RaveCompletionCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            string anError = IDandData[1];

            del(anError);
        }

        void ParseRaveAccessTokenCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveAccessTokenCallback del = (RaveAccessTokenCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            string accessToken = IDandData[1];
            string error = IDandData[2];

            del(accessToken, error);
        }

        void ParseRaveAccountExistsCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveAccountExistsCallback del = (RaveAccountExistsCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            bool accountExists = RaveObject.StringToBool(IDandData[1]);
            bool hasPassword = RaveObject.StringToBool(IDandData[2]);
            string error = IDandData[3];

            del(accountExists, hasPassword, error);
        }

        void ParseRaveUserCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveUserCallback del = (RaveUserCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            RaveUser user = null;
            if (IDandData[1] != "NULL")
            {
                user = new RaveUser();
                user.Deserialize(IDandData[1]);
            }
            string error = IDandData[2];

            del(user, error);
        }

        void ParseRaveUsersCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveUsersCallback del = (RaveUsersCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<RaveUser> users = RaveObject.DeserializeList<RaveUser>(IDandData[1]);
            string error = IDandData[2];

            del(users, error);
        }

        void ParseRaveLoginCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveLoginCallback del = (RaveLoginCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            bool loggedIn = RaveObject.StringToBool(IDandData[1]);
            string error = IDandData[2];

            del(loggedIn, error);
        }

        void ParseRaveLoginStatusCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            int status = (int)(Convert.ToSingle(IDandData[1]));
            string error = IDandData[2];

            if (loginStatusCallback != null)
                loginStatusCallback((RaveLoginStatus)status, error);
        }

        void ParseRaveConnectCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveConnectCallback del = (RaveConnectCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            bool connected = RaveObject.StringToBool(IDandData[1]);
            string error = IDandData[2];

            del(connected, error);
        }

        void ParseRaveDisconnectCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveDisconnectCallback del = (RaveDisconnectCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            bool stillLoggedIn = RaveObject.StringToBool(IDandData[1]);
            string error = IDandData[2];

            del(stillLoggedIn, error);
        }

        void ParseRaveReadinessCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveReadinessCallback del = (RaveReadinessCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            bool ready = RaveObject.StringToBool(IDandData[1]);
            string anError = IDandData[2];

            del(ready, anError);
        }

        void ParseRaveContactsCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveContactsCallback del = (RaveContactsCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<RaveContact> contacts = RaveObject.DeserializeList<RaveContact>(IDandData[1]);
            string error = IDandData[2];

            del(contacts, error);
        }

        void ParseRavePluginShareRequestCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RavePluginShareRequestCallback del = (RavePluginShareRequestCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            RaveShareRequest request = new RaveShareRequest();
            request.Deserialize(IDandData[1]);
            string error = IDandData[2];

            del(request, error);
        }

        void ParseRaveShareRequestCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveShareRequestCallback del = (RaveShareRequestCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<RaveShareRequest> requests = RaveObject.DeserializeList<RaveShareRequest>(IDandData[1]);
            string error = IDandData[2];

            del(requests, error);
        }

        void ParseRaveIdentitiesCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveIdentitiesCallback del = (RaveIdentitiesCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<string> identities = RaveObject.DeserializeList(IDandData[1]);
            string error = IDandData[2];

            del(identities, error);
        }

        void ParseRaveGiftKeyCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveGiftKeyCallback del = (RaveGiftKeyCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            string giftKey = IDandData[1];
            string error = IDandData[2];

            del(giftKey, error);
        }

        void ParseRaveGiftResultCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveGiftResultCallback del = (RaveGiftResultCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<string> succeeded = RaveObject.DeserializeList(IDandData[1]);
            List<string> failed = RaveObject.DeserializeList(IDandData[2]);
            string error = IDandData[3];

            del(succeeded, failed, error);
        }

        void ParseRaveContactGiftResultsCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveContactGiftResultsCallback del = (RaveContactGiftResultsCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<RaveGiftRequest> results = RaveObject.DeserializeList<RaveGiftRequest>(IDandData[1]);
            string error = IDandData[2];

            del(results, error);
        }

        void ParseRaveGiftAndShareResultsCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveGiftAndShareResultsCallback del = (RaveGiftAndShareResultsCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<RaveShareRequest> shareRequests = RaveObject.DeserializeList<RaveShareRequest>(IDandData[1]);
            List<RaveContactGiftResult> contactGiftResults = RaveObject.DeserializeList<RaveContactGiftResult>(IDandData[2]);
            string error = IDandData[3];

            del(shareRequests, contactGiftResults, error);
        }

        void ParseRaveThirdPartyCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveThirdPartyCallback del = (RaveThirdPartyCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            bool isConnected = RaveObject.StringToBool(IDandData[1]);
            string thirdPartyId = IDandData[2];
            string error = IDandData[3];

            del(isConnected, thirdPartyId, error);
        }

        void ParseRaveGiftContentCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveGiftContentCallback del = (RaveGiftContentCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            string key = IDandData[1];
            string requestId = IDandData[2];
            string error = IDandData[3];

            del(key, requestId, error);
        }

        void ParseRaveConnectFriendsControllerCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveConnectFriendsControllerCallback del = (RaveConnectFriendsControllerCallback)TriggerCallback(IDandData[0]);
            if (del == null) return;

            RaveConnectFriendsControllerState state = (RaveConnectFriendsControllerState)int.Parse(IDandData[1]);
            del(state);
        }

        void ParseRaveConnectControllerCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveConnectControllerCallback del = (RaveConnectControllerCallback)TriggerCallback(IDandData[0]);
            if (del == null) return;

            RaveConnectControllerState state = (RaveConnectControllerState)int.Parse(IDandData[1]);
            del(state);
        }

        void ParseRaveUserChangedCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveUserChangedCallback del = (RaveUserChangedCallback)TriggerCallback(IDandData[0]);
            if (del == null) return;

            List<string> changedKeys = RaveObject.DeserializeList(IDandData[1]);

            del(changedKeys);
        }

        // Scene Callbacks

        void ParseRaveSceneCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveSceneCallback del = (RaveSceneCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            string error = IDandData[1];

            del(error);
        }

        void ParseAppDataKeyStateChange(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveAppDataKeyStateObserver del = (RaveAppDataKeyStateObserver)TriggerCallback(IDandData[0]);
            if (del == null) return;

            string selectedKey = IDandData[1];
            List<string> unresolvedKeys = RaveObject.DeserializeList(IDandData[2]);

            del(selectedKey, unresolvedKeys);
        }

        void ParseAppDataKeyStateCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveAppDataKeyStateCallback del = (RaveAppDataKeyStateCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            string selectedKey = IDandData[1];
            List<string> rejectedKeys = RaveObject.DeserializeList(IDandData[2]);
            List<string> unresolvedKeys = RaveObject.DeserializeList(IDandData[3]);
            string error = IDandData[4];

            del(selectedKey, rejectedKeys, unresolvedKeys, error);
        }

        void ParseAppDataKeySelectedCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveAppDataKeySelectedCallback del = (RaveAppDataKeySelectedCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            del(IDandData[1], IDandData[2]);
        }

        void ParseAppDataKeyKeysCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveAppDataKeyKeysCallback del = (RaveAppDataKeyKeysCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            List<string> keys = RaveObject.DeserializeList(IDandData[1]);
            string error = IDandData[2];

            del(keys, error);
        }

        void ParseAppDataKeyCallback(string inputData)
        {
            string[] IDAndData = ParseIDandData(inputData);

            RaveAppDataKeyCallback del = (RaveAppDataKeyCallback)GetCallback(IDAndData[0]);
            if (del == null) return;

            del(IDAndData[1], IDAndData[2]);
        }

        void ParseAppDataKeySetCallback(string inputData)
        {
            string[] IDAndData = ParseIDandData(inputData);

            RaveAppDataKeySetCallback del = (RaveAppDataKeySetCallback)GetCallback(IDAndData[0]);
            if (del == null) return;

            List<RaveAppDataKeyUserPair> pairs = RaveObject.DeserializeList<RaveAppDataKeyUserPair>(IDAndData[1]);
            string error = IDAndData[2];
            del(pairs, error);
        }

        void ParseRaveMergePolicyCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);

            RaveMergePolicy callback = (RaveMergePolicy)TriggerCallback(IDandData[0]);
            if (callback == null) return;

            RaveMergeUser targetUser = new RaveMergeUser();
            targetUser.Deserialize(IDandData[1]);

            callback(targetUser, delegate (bool shouldMerge)
            {
                RaveSocialMergeDecision(RaveObject.BoolToString(shouldMerge));
            });
        }

#if UNITY_IPHONE
        [DllImport("__Internal")]
        static extern void RaveSocialMergeDecision(string shouldMerge);
#else
		static void RaveSocialMergeDecision(string shouldMerge) {
#if UNITY_ANDROID
				using (AndroidJavaClass rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.CustomRaveSocialStatics")) {
					rsClass.CallStatic("RaveSocialMergeDecision", new object[] {"" + shouldMerge});
				}
#endif
		}
#endif


        void ParseBigFishSignUpCallback(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);
            BigFishSignUpSceneCallback del = (BigFishSignUpSceneCallback)GetCallback(IDandData[0]);
            if (del == null) return;

            int status = (int)(Convert.ToSingle(IDandData[1]));
            BigFishSignUpData signUpData = null;
            if (IDandData[2] != "NULL")
            {
                signUpData = new BigFishSignUpData();
                signUpData.Deserialize(IDandData[2]);
            }

            string error = IDandData[3];

            del((RaveCallbackResult)status, signUpData, error);
        }

        void ParseBigFishCALListener(string inputData)
        {
            string[] IDandData = ParseIDandData(inputData);
            BigFishCALListener del = (BigFishCALListener)GetCallback(IDandData[0]);
            if (del == null) return;

            if (IDandData[1] != "NULL")
            {
                del(IDandData[1]);
            }
            else
            {
                del(null);
            }
        }
    }
}