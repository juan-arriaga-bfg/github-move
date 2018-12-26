using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class bfgRating
{
#if UNITY_EDITOR
	// Nothing to see here.
#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern bool __bfgRating__canShowMainMenuRateButton();

	[DllImport("__Internal")]
	private static extern void __bfgRating__mainMenuGiveFeedback();

	[DllImport("__Internal")]
	private static extern void __bfgRating__mainMenuRateApp();

	[DllImport("__Internal")]
	private static extern void __bfgRating__userDidSignificantEvent();
#endif

	public const string BFGRATING_NOTIFICATION_RATING_ALERT_OPENED = "BFGRATING_NOTIFICATION_RATING_ALERT_OPENED";
	public const string BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED = "BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED";

	public static bool canShowMainMenuRateButton()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgRating__canShowMainMenuRateButton();
		#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRatingUnityWrapper")) {result = ajc.CallStatic<bool>("canShowMainMenuRateButton");}
			return result;
		#else
			return false;
		#endif
	}

	public static void mainMenuGiveFeedback()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgRating__mainMenuGiveFeedback();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRatingUnityWrapper")) {ajc.CallStatic("mainMenuGiveFeedback");}
		#else
			return;
		#endif
	}

	public static void mainMenuRateApp()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgRating__mainMenuRateApp();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRatingUnityWrapper")) {ajc.CallStatic("mainMenuRateApp");}
		#else
			return;
		#endif
	}

	public static void userDidSignificantEvent()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgRating__userDidSignificantEvent();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRatingUnityWrapper")) {ajc.CallStatic("userDidSignificantEvent");}
		#else
			return;
		#endif
	}

	public static void enableRatingsPrompt()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			throw new NotImplementedException();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRatingUnityWrapper")) {ajc.CallStatic("enableRatingsPrompt");}
		#else
			return;
		#endif
	}

	public static void disableRatingsPrompt()
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			throw new NotImplementedException();
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgRatingUnityWrapper")) {ajc.CallStatic("disableRatingsPrompt");}
		#else
			return;
		#endif
	}
}
