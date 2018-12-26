package com.bigfishgames.bfgunityandroid;

import java.util.ArrayList;

import com.unity3d.player.UnityPlayer;

public class NotificationQueueRunner {
	
	private static NotificationQueueRunner _instance = null;

	private ArrayList<String> message_queue = null;

	private String unityMessageHandlerObjectName = "";

	public static NotificationQueueRunner GetInstance() {
		if (_instance == null)
			_instance = new NotificationQueueRunner();

		return _instance;
	}

	public static void setUnityMessageHandlerObjectName(String name) {
		GetInstance().DoSetUnityMessageHandlerObjectName(name);
	}
	
	public void DoSetUnityMessageHandlerObjectName(String name) {
		synchronized(unityMessageHandlerObjectName) {
			unityMessageHandlerObjectName = name;
		}
	}

	public static boolean FlushNotificationQueue() {
		GetInstance().DoFlushNotificationQueue();
		return true;
	}

	private final void DoFlushNotificationQueue() {
		
		for (;;) {
			boolean hasMessage = false;
			String msg = null;
	
			synchronized(message_queue) {
				if (message_queue.size() > 0) {
					msg = message_queue.remove(0);
					hasMessage = true;
				}
			}
			
			if (!hasMessage)
				break;
	
			//Log.d("NotificationCenterUnityWrapper", "Sending JSON: " + msg);
			UnityPlayer.UnitySendMessage(unityMessageHandlerObjectName, "HandleNativeMessage", msg);
		}
	}
	
	public NotificationQueueRunner() {
		message_queue = new ArrayList<String>();
	}
	
	public void addToQueue(String msg) {
		synchronized(message_queue) {
			message_queue.add(msg);
		}
	}
}
