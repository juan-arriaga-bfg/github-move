using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BFGSDK
{
    public class bfgGameReporting
    {
#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
	private static string notSerializedPattern = @"^([\w\.\-\_\s]+)$";
#endif

#if UNITY_EDITOR
        // Nothing to See here.
#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logPurchaseMainMenuClosed();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logPurchasePayWallClosed(string paywallID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logGameHintRequested();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logCustomPlacement(string placementName);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__preloadCustomPlacement(string placementName);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logStrategyGuideRequested();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logContentGateShown();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logMainMenuShown();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logRateMenuCanceled();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logOptionsShown();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logPurchaseMainMenuShown();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logPurchasePayWallShown(string paywallID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logLevelStart(string levelID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logLevelFinished(string levelID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logMiniGameStart(string miniGameID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logMiniGameSkipped(string miniGameID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logMiniGameFinished(string miniGameID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logAchievementEarned(string achievementID);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logGameCompleted();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logRateMainMenuCanceled();

	[DllImport("__Internal")]
	private static extern bool __bfgGameReporting__logCustomEvent(
		string name,
		int value,
		int level,
		string details1,
		string details2,
		string details3,
		string[] additionalDetailsKeys,
		string[] additionalDetailsValues,
		int additionalDetailsCount);

	[DllImport("__Internal")]
	private static extern bool __bfgGameReporting__logCustomEvent2(
	string name,
	int value,
	int level,
	string details1,
	string details2,
	string details3,
	string additionalDetails);

	// Big Fish iOS SDK 5.7

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logCustomEventImmediately(
		string name,
		int value,
		int level,
		string details1,
		string details2,
		string details3,
		string[] additionalDetailsKeys,
		string[] additionalDetailsValues,
		int additionalDetailsCount);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logMobileAppTrackingCustomEvent(string name);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logSurveyEvent();

    // Big Fish iOS SDK 5.10

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__dismissVisiblePlacement();

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__setSuppressPlacement(bool suppressPlacements);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logRewardedVideoSeenWithProviderVideoLocation(string provider, string videoLocation);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__logRewardedVideoSeenWithProvider(string provider);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__setLastLevelPlayed(string provider);

	[DllImport("__Internal")]
	private static extern void __bfgGameReporting__setPlayerSpend(float playerSpend);

#endif

        //
        // ---------------------------------------
        //

        public static void logMainMenuShown()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logMainMenuShown();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logMainMenuShown");}
#else
			return;
#endif
        }

        public static void logRateMenuCanceled()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logRateMenuCanceled();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logRateMenuCanceled");}
#else
			return;
#endif
        }

        public static void logOptionsShown()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logOptionsShown();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logOptionsShown");}
#else
			return;
#endif
        }



        public static void logPurchaseMainMenuShown()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logPurchaseMainMenuShown();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logPurchaseMainMenuShown");}
#else
			return;
#endif
        }

        public static void logPurchaseMainMenuClosed()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logPurchaseMainMenuClosed();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logPurchaseMainMenuClosed");}
#else
			return;
#endif
        }

        public static void logPurchasePayWallShown(string paywallID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logPurchasePayWallShown(paywallID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logPurchasePayWallShown", paywallID);}
#else
			return;
#endif
        }

        public static void logPurchasePayWallClosed(string paywallID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logPurchasePayWallClosed(paywallID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logPurchasePayWallClosed", paywallID);}
#else
			return;
#endif
        }

        public static void logIAPbuttontapped(int purchaseButton)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			throw new NotImplementedException();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logIAPbuttontapped", purchaseButton);}
#else
			return;
#endif
        }

        public static void logLevelStart(string levelID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logLevelStart(levelID);
#elif UNITY_ANDROID
				using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logLevelStart", levelID);}
#else
			return;
#endif
        }

        public static void logLevelFinished(string levelID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logLevelFinished(levelID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logLevelFinished", levelID);}
#else
			return;
#endif
        }

        public static void logMiniGameStart(string miniGameID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logMiniGameStart(miniGameID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logMiniGameStart", miniGameID);}
#else
			return;
#endif
        }

        public static void logMiniGameSkipped(string miniGameID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logMiniGameSkipped(miniGameID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logMiniGameSkipped", miniGameID);}
#else
			return;
#endif
        }

        public static void logMiniGameFinished(string miniGameID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logMiniGameFinished(miniGameID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logMiniGameFinished", miniGameID);}
#else
			return;
#endif
        }

        public static void logAchievementEarned(string achievementID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logAchievementEarned(achievementID);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logAchievementEarned", achievementID);}
#else
			return;
#endif
        }

        public static void logGameHintRequested()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logGameHintRequested();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logGameHintRequested");}
#else
			return;
#endif
        }

        public static void logGameCompleted()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logGameCompleted();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logGameCompleted");}
#else
			return;
#endif
        }

        public static void logRateMainMenuCanceled()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logRateMainMenuCanceled();
#elif UNITY_ANDROID
		//warning deprecated method as of Android SDK 6.0
            throw new NotImplementedException();
#else
			return;
#endif
        }

        /// <summary>
        /// Log the Additional details that have already been serialized to JSON.
        /// WARNING: the additionalDetails must already be serialized to valid JSON.
        /// BfgUtilities.ConvertObjectToJsonString() can do this.
        /// </summary>
        /// <returns><c>true</c>, if custom event was logged, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="_value">Value.</param>
        /// <param name="level">Level.</param>
        /// <param name="details1">Details1.</param>
        /// <param name="details2">Details2.</param>
        /// <param name="details3">Details3.</param>
        /// <param name="additionalDetails">Additional details.</param>
        public static bool logCustomEventSerialized(string name, long _value, long level, string details1, string details2, string details3, string additionalDetails)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			// The .mm code confirms if the additionalDetails are a safe JSON object,
			// it will log an exception and not logcustomevent if additionalDetails is not safeserialized JSON
		 	return __bfgGameReporting__logCustomEvent2(name, (int)_value, (int)level, details1, details2, details3, additionalDetails);
#elif UNITY_ANDROID
		bool result = false;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {result = ajc.CallStatic<bool>("logCustomEvent", name, _value, level, details1, details2, details3, additionalDetails);}
		return result;
#else
			return false;
#endif
        }

        /// <summary>
        /// Logs the custom event.
        /// WARNING: the Dictionary<string,string> must not take serialized JSON. Please use the other logCustomEvent() method for that.
        /// </summary>
        /// <returns><c>true</c>, if custom event was loged, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="_value">Value.</param>
        /// <param name="level">Level.</param>
        /// <param name="details1">Details1.</param>
        /// <param name="details2">Details2.</param>
        /// <param name="details3">Details3.</param>
        /// <param name="additionalDetails">Additional details.</param>
        public static bool logCustomEvent(string name, long _value, long level, string details1, string details2, string details3, Dictionary<string, string> additionalDetails)
        {

#if UNITY_EDITOR
            return false;
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
			if (additionalDetails == null) {
				return logCustomEvent(name, _value, level, details1, details2, details3, new string[0], new string[0]);
			}

			int additionalDetailsCount = additionalDetails.Count;
			int i = 0;
			string[] additionalDetailsKeys = new string[additionalDetailsCount];
			string[] additionalDetailsValues = new string[additionalDetailsCount];

			foreach (KeyValuePair<string, string> pair in additionalDetails)
			{
				if (pair.Key == null){
					string exceptionWithName = String.Format("Null Key Detected in: bfgGameReporting.cs - logCustomEvent()");
					//string reason = String.Format("\nThis key/value has a null key.\nKey: \"{0}\" \nValue: {1}\nNull keys are invalid JSON.", pair.Key, pair.Value);
		Debug.Log("Serialization error: "+exceptionWithName);
		Debug.Log("Serialization error reason: "+exceptionWithName);
				}
				else if (pair.Value == null) {
					additionalDetailsKeys[i] = pair.Key;
					additionalDetailsValues[i] = "";
					++i;
					continue;
				}
				// Regex Check pair key doesn't have any characters other than alpha numeric symbols, " ", "_", "-", and "."
				else if(!Regex.Match(pair.Value, notSerializedPattern, RegexOptions.IgnoreCase).Success ||
					!Regex.Match(pair.Key, notSerializedPattern, RegexOptions.IgnoreCase).Success){
					string exceptionWithName = String.Format("SERIALIZED JSON DETECTED IN: bfgGameReporting.cs - logCustomEvent()");
					string reason = String.Format("\nThis key/value appears to have serialized JSON.\nKey: \"{0}\" \nValue: {1}\nPlease use logCustomEventSerialized() method for already serialized data.", pair.Key, pair.Value);
					Debug.Log(exceptionWithName+reason);
				}

				additionalDetailsKeys[i] = pair.Key;
				additionalDetailsValues[i] = pair.Value;
				++i;
			}

			return logCustomEvent(name, _value, level, details1, details2, details3, additionalDetailsKeys, additionalDetailsValues);
#elif UNITY_ANDROID
			string additionalDetailsJsonString = BfgUtilities.ConvertDictionaryOfStringsToJsonString(additionalDetails);
				bool result = false;
			 using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {result = ajc.CallStatic<bool>("logCustomEvent", name, _value, level, details1, details2, details3, additionalDetailsJsonString);}
			 return result;
#else
			return false;
#endif
        }

        public static bool logCustomEvent(
            string name,
            long value,
            long level,
            string details1,
            string details2,
            string details3,
            string[] additionalDetailsKeys,   // Keys for additionalDetails { key, value } pair, length needs to match additionalDetailsValues
            string[] additionalDetailsValues) // Values for additionalDetails { key, value } pair, length needs to match additionalDetailsKeys
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
				return __bfgGameReporting__logCustomEvent(
				name, (int) value, (int) level, details1, details2, details3,
				additionalDetailsKeys, additionalDetailsValues, additionalDetailsKeys.Length);
#elif UNITY_ANDROID
			int additionalDetailsCount = additionalDetailsKeys.Length;
			Dictionary<string,string> additionalDetails = new Dictionary<string,string>();

			for (int i = 0; i < additionalDetailsCount; ++i)
			{
				additionalDetails.Add(additionalDetailsKeys[i], additionalDetailsValues[i]);
			}

			return logCustomEvent(name, value, level, details1, details2, details3, additionalDetails);

#else
			return false;
#endif
        }

        public static void logCustomPlacement(string placementName)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logCustomPlacement(placementName);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logCustomPlacement", placementName);}
#else
			return;
#endif
        }

        public static void preloadCustomPlacement(string placementName)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__preloadCustomPlacement(placementName);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("preloadCustomPlacement", placementName);}
#else
			return;
#endif
        }

        public static void logStrategyGuideRequested()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logStrategyGuideRequested();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void logContentGateShown()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logContentGateShown();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        // Big Fish iOS SDK 5.7

        public static void logCustomEventImmediately(string name, long _value, long level, string details1, string details2, string details3, Dictionary<string, string> additionalDetails)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			int additionalDetailsCount = additionalDetails.Count;
			int i = 0;
			string[] additionalDetailsKeys = new string[additionalDetailsCount];
			string[] additionalDetailsValues = new string[additionalDetailsCount];

			foreach (KeyValuePair<string, string> pair in additionalDetails)
			{
				additionalDetailsKeys[i] = pair.Key;
				additionalDetailsValues[i] = pair.Value;
				++i;
			}

			logCustomEventImmediately(name, _value, level, details1, details2, details3, additionalDetailsKeys, additionalDetailsValues);
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void logCustomEventImmediately(
            string name,
            long value,
            long level,
            string details1,
            string details2,
            string details3,
            string[] additionalDetailsKeys,   // Keys for additionalDetails { key, value } pair, length needs to match additionalDetailsValues
            string[] additionalDetailsValues) // Values for additionalDetails { key, value } pair, length needs to match additionalDetailsKeys
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logCustomEventImmediately(
				name, (int) value, (int) level, details1, details2, details3,
				additionalDetailsKeys, additionalDetailsValues, additionalDetailsKeys.Length);
#elif UNITY_ANDROID
			int additionalDetailsCount = additionalDetailsKeys.Length;
			Dictionary<string,string> additionalDetails = new Dictionary<string,string>();

			for (int i = 0; i < additionalDetailsCount; ++i)
			{
				additionalDetails.Add(additionalDetailsKeys[i], additionalDetailsValues[i]);
			}

			logCustomEventImmediately(name, value, level, details1, details2, details3, additionalDetails);
#else
			return;
#endif
        }

        public static void logMobileAppTrackingCustomEvent(string name)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logMobileAppTrackingCustomEvent(name);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logMobileAppTrackingCustomEvent", name);}
#else
			return;
#endif
        }

        public static void logSurveyEvent()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logSurveyEvent();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        // Big Fish iOS SDK 5.10

        public static void dismissVisiblePlacement()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__dismissVisiblePlacement();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void setSuppressPlacement(bool suppressPlacements)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__setSuppressPlacement(suppressPlacements);
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void setPlayerSpend(float playerSpend)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		__bfgGameReporting__setPlayerSpend(playerSpend);
#elif UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("setPlayerSpend", playerSpend);}
#else
		return;
#endif
        }

        public static void setLastLevelPlayed(string lastLevel)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		__bfgGameReporting__setLastLevelPlayed(lastLevel);
#elif UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("setLastLevelPlayed", lastLevel);}
#else
		return;
#endif
        }

        public static void logRewardedVideoSeenWithProviderVideoLocation(string provider, string videoLocation)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logRewardedVideoSeenWithProviderVideoLocation(provider, videoLocation);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logRewardedVideoSeenWithProvider", provider, videoLocation);}
#else
			return;
#endif
        }

        public static void logRewardedVideoSeenWithProvider(string provider)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgGameReporting__logRewardedVideoSeenWithProvider(provider);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgGameReportingUnityWrapper")) {ajc.CallStatic("logRewardedVideoSeenWithProvider", provider);}
#else
			return;
#endif
        }
    }
}