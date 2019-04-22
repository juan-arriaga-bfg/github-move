package com.bigfishgames.bfgunityandroid;

import com.bigfishgames.bfglib.NSNotification;
import com.bigfishgames.bfglib.NSNotificationCenter;
import android.util.Base64;
import android.util.Log;
import com.google.gson.Gson;

public class NotificationCenterUnityWrapper {

	// Static Data Members

	private static NotificationCenterUnityWrapper _instance = null;
	private NotificationQueueRunner notification_queue_runner = null;

	// Static Methods

	public static NotificationCenterUnityWrapper GetInstance() {
		if (_instance == null)
			_instance = new NotificationCenterUnityWrapper();

		return _instance;
	}

	public static void AddObserver(String notificationName) {
		GetInstance()._AddObserver(notificationName);
	}

	public static void RemoveObserver(String notificationName) {
		GetInstance()._RemoveObserver(notificationName);
	}

	// Non-static Data Members

	// Non-static Methods

	private NotificationCenterUnityWrapper() {
		notification_queue_runner = NotificationQueueRunner.GetInstance();
	}

	private final void _AddObserver(String notificationName) {
		Log.d("NotificationCenterUnityWrapper", "NotificationCenterUnityWrapper._AddObserver called for: " + notificationName);
		NSNotificationCenter.defaultCenter().addObserver(this, "HandleNotification", notificationName, null);
	}

	private final void _RemoveObserver(String notificationName) {
		Log.d("NotificationCenterUnityWrapper", "NotificationCenterUnityWrapper._RemoveObserver called for: " + notificationName);
		NSNotificationCenter.defaultCenter().removeObserver(this, notificationName, null);
	}

	public final void HandleNotification(NSNotification notification) {
		// Convert call, with argument, into a single JSON string representation and invoke UnitySendMessage.
		String notificationName = notification.getName();
		Object notificationObject = notification.getObject();
		Gson gson = new Gson();
		String notificationObjectJSON = gson.toJson(notificationObject);

		// Receipt handling from Bagelcode
		if (notificationName.equalsIgnoreCase("NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT")){
			notificationObjectJSON = "\"" + Base64.encodeToString(notificationObjectJSON.getBytes(), Base64.NO_CLOSE | Base64.NO_WRAP | Base64.URL_SAFE ) + "\"";
			//Log.d("NotificationCenterUnityWrapper", "Receipt encoding to base64");
		}

		String sendMessage = "{\"name\":\"" + notification.getName() + "\", \"arg\":" + notificationObjectJSON + "}";

		notification_queue_runner.addToQueue(sendMessage);
	}

}
