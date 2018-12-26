using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;


public delegate void NotificationHandler (string arg);

internal class NotificationHandlerSet
{
	internal List<NotificationHandler> HandlerList { get { return handlerList; } }

	private List<NotificationHandler> handlerList = new List<NotificationHandler> ();

	public void AddHandler (NotificationHandler h)
	{
		handlerList.Add (h);	
	}

	public void RemoveHandler (NotificationHandler h)
	{
		handlerList.Remove (h);
	}

	public void Raise (string arg)
	{		
		foreach (NotificationHandler h in handlerList)
		{
			try
			{
				h (arg);
			}
			catch (Exception e)
			{
				Debug.Log ("HandlerSet:  Raise.  Handler threw: " + e.ToString ());
			}
		}
	}

	public bool IsEmpty {
		get {
			if (handlerList == null || handlerList.Count <= 0)
			{
				return true;
			}
			return false;
		}
	}
}

[System.Serializable]
public class JsonMessage
{
	public string name;
	public string arg;

	public static JsonMessage CreateFromJSON (string jsonString)
	{
		return JsonUtility.FromJson<JsonMessage> (jsonString);
	}
}

public class NotificationCenter
{


	#if UNITY_IOS || UNITY_IPHONE
	[DllImport ("__Internal")] private static extern void __BfgUtilities__addNotificationObserver (string notificationName);

	[DllImport ("__Internal")] private static extern void __BfgUtilities__removeNotificationObserver (string notificationName);
	#endif
	#if UNITY_ANDROID
	public const string BFG_NOTIFICATION_APP_RESUME = "bfg_app_resume";
	public const string BFG_NOTIFICATION_APP_PAUSE	= "bfg_app_pause";
	public const string BFG_NOTIFICATION_APP_STOP	= "bfg_app_stop";
	#endif

	private static NotificationCenter _instance = null;
	private Dictionary<string, NotificationHandlerSet> _notifications_by_name = null;

	private NotificationCenter ()
	{
		_notifications_by_name = new Dictionary<string, NotificationHandlerSet> ();
	}

	public static NotificationCenter Instance {
		get {
			if (_instance == null)
			{
				_instance = new NotificationCenter ();
			}
			return _instance;
		}
	}

	/// <summary>
	/// Checks if there is a handlerset for "notificationName" and if the handlerset has an instance of "handler". 
	/// </summary>
	/// <returns><c>true</c>, if set has observer was handlered, <c>false</c> otherwise.</returns>
	/// <param name="handler">Handler.</param>
	/// <param name="notificationName">Notification name.</param>
	public bool HandlerSetHasObserver (NotificationHandler handler, string notificationName)
	{
		NotificationHandlerSet handlerSet = null;

		if (_notifications_by_name.TryGetValue (notificationName, out handlerSet))
		{
			if (handlerSet != null && handlerSet.HandlerList.Contains (handler))
			{
				return true;
			}
		}
		return false;
	}

	public void AddObserver (NotificationHandler handler, string notificationName)
	{
		NotificationHandlerSet handlerSet = null;
		//Debug.Log ("NotificationCenter AddObserver for: " + notificationName);
		if (!_notifications_by_name.TryGetValue (notificationName, out handlerSet))
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

			_notifications_by_name.Add (notificationName, new NotificationHandlerSet ());
		}
		_notifications_by_name [notificationName].AddHandler (handler);
				
	}

	public void RemoveObserver (NotificationHandler handler, string notificationName)
	{
		// check if "notificationName" has a handlerSet AND if that handlerset has an instance of handler. 
		if (HandlerSetHasObserver (handler, notificationName))
		{
			// we can now remove the handler from notificationName's handlerset since we know there is one to remove. 
			_notifications_by_name [notificationName].RemoveHandler (handler);		

			if (_notifications_by_name [notificationName].IsEmpty)
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

				_notifications_by_name.Remove (notificationName);
			}
		}
	}

	internal void HandleNativeMessage (string message)
	{	
		NotificationHandlerSet handlerSet = null;	
		string name, arg; 

		GetStringAndArg (message, out name, out arg);
		
		if (name != null)
		{
			if (_notifications_by_name.TryGetValue (name, out handlerSet))
			{
				handlerSet.Raise (arg);
			}
			else
			{
				// There are no handlers for this notification name yet. 				
				// This can happen if native (ios or android) code uses UnitySendMessage before this NotificationCenter script has added the handler...or if the developer forgets to add a listener for this notification!			
				Debug.Log ("NotificationCenter. Notification: \"" + name + "\" doesn not have an observer added yet. This notification is going to be cleared.");				
			}
		}
	}

	/// <summary>
	/// Gets the string and argument values from the message json payload of format: {"name": "name_val", "arg": {"arg_json_val"}.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="name">Name.</param>
	/// <param name="arg">Argument.</param>
	// TODO: Make this method internal. Modify Unit tests to still call this method. 
	public void GetStringAndArg (string message, out string name, out string arg)
	{	
		JsonMessage messageObject = JsonMessage.CreateFromJSON (message);
		if (!string.IsNullOrEmpty (messageObject.name))
		{
			name = messageObject.name;
		}
		else
		{
			name = null;
		}
		
		if (string.IsNullOrEmpty (messageObject.arg))
		{
			// JsonUtility can't parse nested json, use JsonHelper to get the entire json object as the value from the "arg" key. 
			messageObject.arg = JsonHelper.GetJsonObject (message, "arg");
			if (!string.IsNullOrEmpty (messageObject.arg))
			{
				arg = messageObject.arg;
			}
			else
			{
				messageObject.arg = "";
				arg = messageObject.arg;				
			}
		}
		else
		{
			// Special handling for this notification since the value needs to be base64 decoded. 
			if (name == bfgPurchaseAndroid.NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT)
			{	
				byte[] argData = Convert.FromBase64String (messageObject.arg);
				messageObject.arg = System.Text.Encoding.UTF8.GetString (argData);
				arg = messageObject.arg; 
			}
			else
			{
				arg = messageObject.arg;
			}
		}
	}
}
