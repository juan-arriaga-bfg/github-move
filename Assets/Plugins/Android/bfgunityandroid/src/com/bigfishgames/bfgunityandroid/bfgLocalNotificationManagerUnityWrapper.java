package com.bigfishgames.bfgunityandroid;

import android.content.Intent;
import com.bigfishgames.bfglib.bfgLocalNotificationManager;

public class bfgLocalNotificationManagerUnityWrapper {

	public static void cancelNotification(long notificationId) {
		bfgLocalNotificationManager.sharedInstance().cancelNotification(notificationId);
	}

	public static long scheduleNotification(
			String title,
			String content,
			int iconResourceId,
			long replaceNotificationId,
			long epochTimeInMilliseconds,
			boolean autoDismiss ) {

		// intent - bring up the app when the notification is tapped
		// intent can be customized for specific behavior
		Intent intent = new Intent(BFGUnityPlayerNativeActivity.currentInstance(), BFGUnityPlayerNativeActivity.class);
		intent.setAction(Intent.ACTION_VIEW);

		return bfgLocalNotificationManager.sharedInstance().scheduleNotification(
				title, content, iconResourceId, replaceNotificationId, intent, epochTimeInMilliseconds, autoDismiss );
	}

	public static long scheduleNotification(
			String title,
			String content,
			int iconResourceId,
			long epochTimeInMilliseconds,
			boolean autoDismiss ) {

		// intent - bring up the app when the notification is tapped
		// intent can be customized for specific behavior
		Intent intent = new Intent(BFGUnityPlayerNativeActivity.currentInstance(), BFGUnityPlayerNativeActivity.class);
		intent.setAction(Intent.ACTION_VIEW);

		return bfgLocalNotificationManager.sharedInstance().scheduleNotification(
				title, content, iconResourceId, intent, epochTimeInMilliseconds, autoDismiss );
	}
}
