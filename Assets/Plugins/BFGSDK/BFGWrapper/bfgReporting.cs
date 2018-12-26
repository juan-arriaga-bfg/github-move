using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class bfgReporting
{
#if UNITY_EDITOR
	// Nothing to see here.
#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void __bfgReporting__logSingleFireEvent(string name);

	[DllImport("__Internal")]
	private static extern void __bfgReporting__logEvent(string name);

	[DllImport("__Internal")]
	private static extern void __bfgReporting__logEventWithParameters(
		string name,
		string[] additionalDetailsKeys,
		string[] additionalDetailsValues,
		int additionalDetailsCount);
#endif


	/// <summary>
	/// Logs the event.
	/// </summary>
	/// <param name="eventName">Event name.</param>
	public static void logEvent(string eventName)
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgReporting__logEvent(eventName);
		#elif UNITY_ANDROID
			//warning deprecated method as of Android SDK 5.6
			throw new NotImplementedException();			
		#else
			return;
		#endif
	}

	/// <summary>
	/// Logs the event with parameters.
	/// </summary>
	/// <param name="eventName">Event name.</param>
	/// <param name="parameters">Parameters.</param>
	public static void logEventWithParameters(string eventName, Dictionary<string,string> parameters)
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			int additionalDetailsCount = parameters.Count;
			int i = 0;
			string[] additionalDetailsKeys = new string[additionalDetailsCount];
			string[] additionalDetailsValues = new string[additionalDetailsCount];

			foreach (KeyValuePair<string, string> pair in parameters)
			{
				additionalDetailsKeys[i] = pair.Key;
				additionalDetailsValues[i] = pair.Value;
				++i;
			}

			logEventWithParameters(eventName, additionalDetailsKeys, additionalDetailsValues);
		#elif UNITY_ANDROID
			//warning deprecated method as of Android SDK 5.6
			throw new NotImplementedException();
			
		#else
			return;
		#endif
	}

	/// <summary>
	/// Logs the event with parameters.
	/// </summary>
	/// <param name="eventName">Event name.</param>
	/// <param name="additionalDetailsKeys">Keys for additionalDetails { key, value } pair, length needs to match additionalDetailsValues.</param>
	/// <param name="additionalDetailsValues">Values for additionalDetails { key, value } pair, length needs to match additionalDetailsKeys</param>
	public static void logEventWithParameters(string eventName, string[] additionalDetailsKeys, string[] additionalDetailsValues)  
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgReporting__logEventWithParameters(eventName, additionalDetailsKeys, additionalDetailsValues, additionalDetailsKeys.Length);
		#elif UNITY_ANDROID
			//warning deprecated method as of Android SDK 5.6
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

	/// <summary>
	/// Logs the single fire event.
	/// </summary>
	/// <param name="eventName">Event name.</param>
	public static void logSingleFireEvent(string eventName)
	{
		#if UNITY_EDITOR
			return;
		#elif UNITY_IOS || UNITY_IPHONE
			__bfgReporting__logSingleFireEvent(eventName);
		#elif UNITY_ANDROID
			//warning Removed in Big Fish Android SDK 5.6			
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

	
    /// <summary>
    /// Gets the rave identifier.
    /// </summary>
    /// <returns>The rave identifier.</returns>
	public static string getRaveId()
	{
		#if UNITY_EDITOR
			return "";
		#elif UNITY_IOS || UNITY_IPHONE
			throw new NotImplementedException();
		#elif UNITY_ANDROID
			string result = "";
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgReportingUnityWrapper")) {result = ajc.CallStatic<string>("getRaveId");}
			return result;
		#else
			return "";
		#endif
	}
}
