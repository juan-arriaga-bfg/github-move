using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BFGSDK
{
    #region Helper Classes
    internal class NotificationHandlerSet
    {
        internal bool IsEmpty { get { return (handlerList == null || handlerList.Count <= 0); } }

        internal List<NotificationHandler> HandlerList { get { return handlerList; } }

        private List<NotificationHandler> handlerList = new List<NotificationHandler>();

        internal void AddHandler(NotificationHandler h)
        {
            handlerList.Add(h);
        }

        internal void RemoveHandler(NotificationHandler h)
        {
            handlerList.Remove(h);
        }

        internal void RaiseAllHandlers(string arg)
        {
            foreach (NotificationHandler h in handlerList)
            {
                try
                {
                    h(arg);
                }
                catch (Exception e)
                {
                    Debug.Log("HandlerSet:  Raise.  Handler threw: " + e);
                }
            }
        }

    }

    [System.Serializable]
    public class JsonMessage
    {
        public string name = null;
        public string arg = null;

        public static JsonMessage CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<JsonMessage>(jsonString);
        }
    }

    #endregion


    public delegate void NotificationHandler(string arg);

    public class NotificationCenter
    {


#if UNITY_IOS || UNITY_IPHONE
        [DllImport("__Internal")] private static extern void __BfgUtilities__addNotificationObserver(string notificationName);

        [DllImport("__Internal")] private static extern void __BfgUtilities__removeNotificationObserver(string notificationName);
#endif
#if UNITY_ANDROID
    public const string BFG_NOTIFICATION_APP_RESUME = "BFG_APP_RESUME";
    public const string BFG_NOTIFICATION_APP_PAUSE = "BFG_APP_PAUSE";
    public const string BFG_NOTIFICATION_APP_STOP = "BFG_APP_STOP";
#endif

        private static NotificationCenter _instance = null;

        private Dictionary<string, NotificationHandlerSet> handlerSetPerNotification = null;
        internal Dictionary<string, NotificationHandlerSet> HandlerSetPerNotification { get { return handlerSetPerNotification; } }

        private NotificationCenter()
        {
            handlerSetPerNotification = new Dictionary<string, NotificationHandlerSet>();
        }

        public static NotificationCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationCenter();
                }
                return _instance;
            }
        }

        public bool HandlerSetHasObserver(NotificationHandler handler, string notificationName)
        {
            NotificationHandlerSet handlerSet = null;
            if (TryGetNotificationHandlerSet(notificationName, out handlerSet))
            {
                return IsHandlerInSet(handler, handlerSet);
            }
            return false;
        }

        public void AddObserver(NotificationHandler handler, string notificationName)
        {
            if (!HandlerSetPerNotificationHasKey(notificationName) && !HandlerSetHasObserver(handler, notificationName))
            {
#if UNITY_EDITOR
                // nothing to do here
#elif UNITY_IOS || UNITY_IPHONE
				__BfgUtilities__addNotificationObserver(notificationName);
#elif UNITY_ANDROID
				// (Invoke Java to add observer for notificationName)
				using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.NotificationCenterUnityWrapper")) {
					ajc.CallStatic("AddObserver", notificationName);
				}
#endif
                handlerSetPerNotification.Add(notificationName, new NotificationHandlerSet());
            }
            if (!HandlerSetHasObserver(handler, notificationName))
            {
                handlerSetPerNotification[notificationName].AddHandler(handler);
            }

        }

        public void RemoveObserver(NotificationHandler handler, string notificationName)
        {
            if (HandlerSetHasObserver(handler, notificationName))
            {
                handlerSetPerNotification[notificationName].RemoveHandler(handler);

                if (handlerSetPerNotification[notificationName].IsEmpty)
                {
#if UNITY_EDITOR
                    // do nothing
#elif UNITY_IOS || UNITY_IPHONE

					__BfgUtilities__removeNotificationObserver(notificationName);
#elif UNITY_ANDROID
					// (Invoke Java to remove observer for notificationName)
					using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.NotificationCenterUnityWrapper")) {
						ajc.CallStatic("RemoveObserver", notificationName);
					}
#endif

                    handlerSetPerNotification.Remove(notificationName);
                }
            }
        }

        internal void RaiseNativeMessageToHandlerSet(string message)
        {
            NotificationHandlerSet handlerSet = null;
            string name, arg;

            GetStringAndArgFromJson(message, out name, out arg);

            if (TryGetNotificationHandlerSet(name, out handlerSet))
            {
                handlerSet.RaiseAllHandlers(arg);
            }
            else
            {
                // A handler hasn't been added for this notification in C# yet				
                // This can happen if native (ios or android) code uses UnitySendMessage before this NotificationCenter script has added the handler...or if the developer forgets to add a listener for this notification!			
                Debug.Log("NotificationCenter. Notification: \"" + name + "\" does not have an observer added yet. This notification is going to be cleared.");
            }
        }

        /// <summary>
        /// Gets the string and argument values from the message json payload of format: {"name": "name_val", "arg": {"arg_json_val"}.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="name">Name.</param>
        /// <param name="arg">Argument.</param> 
        public void GetStringAndArgFromJson(string message, out string name, out string arg)
        {
            JsonMessage messageObject = JsonMessage.CreateFromJSON(message);
            name = GetMessageName(messageObject);

            if (string.IsNullOrEmpty(messageObject.arg))
            {
                // JsonUtility can't parse nested json, use JsonHelper to get the entire json object as the value from the "arg" key.
                arg = ParseNestedJsonArg(messageObject, message);
            }
            else
            {
                // Special handling for this notification since the value needs to be base64 decoded. 
                if (name == bfgPurchaseAndroid.NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT)
                {
                    arg = DecryptBase64Json(messageObject.arg);
                }
                else
                {
                    arg = messageObject.arg;
                }
            }
        }

        #region Helper Methods

        private bool TryGetNotificationHandlerSet(string notificationName, out NotificationHandlerSet handlerSet)
        {
            handlerSet = null;
            if (notificationName != null)
            {
                return handlerSetPerNotification.TryGetValue(notificationName, out handlerSet);
            }
            return false;
        }

        private bool IsHandlerInSet(NotificationHandler handler, NotificationHandlerSet handlerSet)
        {
            return handlerSet.HandlerList.Contains(handler);
        }

        private void ResetInstance()
        {
            _instance = null;
        }

        private string GetMessageName(JsonMessage jsonMessage)
        {
            if (!string.IsNullOrEmpty(jsonMessage.name))
            {
                return jsonMessage.name;
            }
            else
            {
                return null;
            }
        }

        private string GetMessageArg(JsonMessage jsonMessage)
        {
            if (!string.IsNullOrEmpty(jsonMessage.name))
            {
                return jsonMessage.name;
            }
            else
            {
                return null;
            }
        }

        private string ParseNestedJsonArg(JsonMessage messageObject, string message)
        {
            string arg;
            messageObject.arg = JsonHelper.GetJsonObject(message, "arg");

            if (!string.IsNullOrEmpty(messageObject.arg))
            {
                arg = messageObject.arg;
            }
            else
            {
                messageObject.arg = "";
                arg = messageObject.arg;
            }

            return arg;
        }

        private string DecryptBase64Json(string message)
        {
            byte[] argData = Convert.FromBase64String(message);
            message = System.Text.Encoding.UTF8.GetString(argData);
            return message;
        }

        private bool HandlerSetPerNotificationHasKey(string key)
        {
            return handlerSetPerNotification.ContainsKey(key);
        }

        #endregion

    }
}