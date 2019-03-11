using System;
using System.Collections.Generic;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR

using System.Linq;

#endif

namespace Assets.SimpleAndroidNotifications
{
    public static class NotificationManager
    {
		/// <summary>
		/// Should be the same as declared in the main AndroidManifest.xml!
		/// </summary>
		public const string UnityActivityClassName = "com.bigfishgames.bfgunityandroid.BFGUnityPlayerNativeActivity";

		/// <summary>
		/// This name is declared inside AAR in AndroidManifest.xml (not main).
		/// </summary>
		public const string PluginActivityClassName = "com.hippogames.simpleandroidnotifications.Controller";

		#if UNITY_ANDROID && !UNITY_EDITOR

        private static AndroidJavaClass JavaClass
        {
	        get { return new AndroidJavaClass(PluginActivityClassName); }
        }
		
        #endif

		/// <summary>
		/// Schedule simple notification without app icon.
		/// </summary>
		public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0, bool silent = false)
        {
            return SendCustom(new NotificationParams
            {
                Id = NotificationIdHandler.GetNotificationId(),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = !silent,
                Vibrate = !silent,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = "",
                ExecuteMode = NotificationExecuteMode.Inexact
            });
        }

        /// <summary>
        /// Schedule notification with app icon.
        /// </summary>
        public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0, bool silent = false)
        {
            return SendCustom(new NotificationParams
            {
                Id = NotificationIdHandler.GetNotificationId(),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = !silent,
                Vibrate = !silent,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = "app_icon",
                ExecuteMode = NotificationExecuteMode.Inexact
            });
        }

        /// <summary>
        /// Schedule customizable notification.
        /// </summary>
        public static int SendCustom(NotificationParams notificationParams)
        {
            #if UNITY_EDITOR

            Debug.LogWarning("Simple Android Notifications are not supported for current platform. Build and play this scene on android device!");

            #elif UNITY_ANDROID

            var p = notificationParams;
            var delay = (long) p.Delay.TotalMilliseconds;
            var repeatInterval = p.Repeat ? (long) p.RepeatInterval.TotalMilliseconds : 0;
            var vibration = string.Join(",", p.Vibration.Select(i => i.ToString()).ToArray());

            JavaClass.CallStatic("SetNotification", p.Id, p.GroupName ?? "", p.GroupSummary ?? "", p.ChannelId, p.ChannelName, delay, Convert.ToInt32(p.Repeat), repeatInterval, p.Title, p.Message, p.Ticker, Convert.ToInt32(p.Multiline),
                Convert.ToInt32(p.Sound), p.CustomSound ?? "", Convert.ToInt32(p.Vibrate), vibration, Convert.ToInt32(p.Light), p.LightOnMs, p.LightOffMs, ColotToInt(p.LightColor), p.LargeIcon ?? "", GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), (int) p.ExecuteMode, (int) p.Importance, p.CallbackData, UnityActivityClassName);
            
            NotificationIdHandler.AddScheduledNotificaion(p.Id);

            #elif UNITY_IPHONE

            var notification = new UnityEngine.iOS.LocalNotification
            {
                hasAction = false,
                alertBody = notificationParams.Message,
                fireDate = DateTime.Now.Add(notificationParams.Delay)
            };

            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);

            #endif

            return notificationParams.Id;
        }

        /// <summary>
        /// Cancel notification by id.
        /// </summary>
        public static void Cancel(int id)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

            JavaClass.CallStatic("CancelNotification", id);

            NotificationIdHandler.RemoveScheduledNotificaion(id);

            #endif
        }

        /// <summary>
        /// Cancel all notifications.
        /// </summary>
        public static void CancelAll()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

			JavaClass.CallStatic("CancelAllNotifications");

            NotificationIdHandler.RemoveAllScheduledNotificaions();

            #endif
        }

        /// <summary>
        /// Cancel all displayed notifications only.
        /// </summary>
        public static void CancelAllDisplayed()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

            JavaClass.CallStatic("CancelAllDisplayedNotifications");

            NotificationIdHandler.RemoveAllScheduledNotificaions();

            #endif
        }

        /// <summary>
        /// Return notification callback if app was launched from notification (and null otherwise).
        /// </summary>
        public static NotificationCallback GetNotificationCallback()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

            var currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            var hasExtra = intent.Call<bool>("hasExtra", "Notification.Id");

            if (hasExtra)
            {
                var extras = intent.Call<AndroidJavaObject>("getExtras");

                return new NotificationCallback
                {
                    Id = extras.Call<int>("getInt", "Notification.Id"),
                    Data = extras.Call<string>("getString", "Notification.CallbackData")
                };
            }

            #endif

            return null;
        }

		/// <summary>
		/// Get a list of notification channel Id (API 26+).
		/// </summary>
		public static List<string> GetNotificationChannelIds()
		{
			var ids = new List<string>();

			#if UNITY_ANDROID && !UNITY_EDITOR

            var list = JavaClass.CallStatic<string>("GetNotificationChannelIds");

			if (!string.IsNullOrEmpty(list))
			{
				ids = list.Split(',').ToList();
			}

			#endif

			return ids;
		}

		/// <summary>
		/// Delete notification channel by Id (API 26+).
		/// </summary>
		public static void DeleteNotificationChannel(string channelId)
	    {
			#if UNITY_ANDROID && !UNITY_EDITOR

            JavaClass.CallStatic("DeleteNotificationChannel", channelId);

            #endif
		}

        private static int ColotToInt(Color color)
        {
            var smallIconColor = (Color32) color;
            
            return smallIconColor.r * 65536 + smallIconColor.g * 256 + smallIconColor.b;
        }

        private static string GetSmallIconName(NotificationIcon icon)
        {
            return icon.ToString().ToLower();
        }
    }
}