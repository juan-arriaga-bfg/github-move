using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BFGSDK
{
    public class UnityMessageHandler : MonoBehaviour
    {

        // This constant determines how frequently the game loop will ask the BFG SDK Unity wrapper to send pending notifications to the application.
        // The value must be an integer greater than or equal to 1.
        // A value of 1 would mean that the application would request notification submission on every iteration of the game loop.
        // Note that a large value could delay the arrival of SDK notifications, while small value could adversely affect game performance.
        const uint Notification_Flush_Period_In_Frames = 1;

        uint _frames_remaining = 0;
#if UNITY_EDITOR
        // Nothing to see here.
#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void __BfgUtilities__setUnityMessageHandlerObjectName(string name);

	[DllImport("__Internal")]
	private static extern void __BfgUtilities__flushNotificationQueue();


#elif UNITY_ANDROID
	AndroidJavaClass notificationAjc = null;
	AndroidJavaClass unityMessageAjc = null;
#endif

        // Use this for initialization
        void Start()
        {
            _frames_remaining = 0;
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__BfgUtilities__setUnityMessageHandlerObjectName(this.gameObject.name);
#elif UNITY_ANDROID
			notificationAjc = new AndroidJavaClass ("com.bigfishgames.bfgunityandroid.NotificationQueueRunner");
			unityMessageAjc = new AndroidJavaClass ("com.bigfishgames.bfgunityandroid.UnityMessageQueueRunner");

			if (notificationAjc != null)
				notificationAjc.CallStatic("setUnityMessageHandlerObjectName", this.gameObject.name);
#endif
        }


        void Update()
        {
            if (_frames_remaining == 0)
            {
                _frames_remaining = Notification_Flush_Period_In_Frames;
#if UNITY_EDITOR
                // do nothing
#elif UNITY_IOS || UNITY_IPHONE
		__BfgUtilities__flushNotificationQueue();
#elif UNITY_ANDROID
			if (notificationAjc != null)
				notificationAjc.CallStatic<bool>("FlushNotificationQueue");
			if (unityMessageAjc != null)
				unityMessageAjc.CallStatic<bool>("FlushMessageQueue");
#endif
            }
            --_frames_remaining;
        }

        public void HandleNativeMessage(string message)
        {
            NotificationCenter.Instance.RaiseNativeMessageToHandlerSet(message);
        }
    }
}