using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BFGSDK
{
    public class bfgLocalNotificationManager
    {
        public static void cancelNotification(long notificationId)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE

			throw new NotImplementedException();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgLocalNotificationManagerUnityWrapper")) {ajc.CallStatic("cancelNotification", notificationId);}
#else
			return;
#endif
        }

        public static long scheduleNotification(
            string title,
            string content,
            int iconResourceId,
            long replaceNotificationId,
            long epochTimeInMilliseconds,
            bool autoDismiss)
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE

			throw new NotImplementedException();
#elif UNITY_ANDROID
			long result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgLocalNotificationManagerUnityWrapper")) {result = ajc.CallStatic<long>("scheduleNotification",
				title, content, iconResourceId, replaceNotificationId, epochTimeInMilliseconds, autoDismiss);}
			return result;
#else
			return 0;
#endif
        }

        public static long scheduleNotification(
            string title,
            string content,
            int iconResourceId,
            long epochTimeInMilliseconds,
            bool autoDismiss)
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE

			throw new NotImplementedException();
#elif UNITY_ANDROID
			long result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgLocalNotificationManagerUnityWrapper")) {result = ajc.CallStatic<long>("scheduleNotification",
				title, content, iconResourceId, epochTimeInMilliseconds, autoDismiss);}
			return result;
#else
			return 0;
#endif
        }
    }
}