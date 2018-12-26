using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class bfgPushNotificationManager
{
#if UNITY_EDITOR
	// Nothing to see here.
#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void __bfgPushNotificationManager__registerForPushNotifications();

	[DllImport("__Internal")]
	private static extern void __bfgPushNotificationManager__setIconBadgeNumber(int badgeNumber);
#endif

	public static void registerForPushNotifications()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgPushNotificationManager__registerForPushNotifications();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

	public static void setIconBadgeNumber(int badgeNumber)
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgPushNotificationManager__setIconBadgeNumber(badgeNumber);
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return;
		#endif
	}
}
