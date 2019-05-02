package com.bigfishgames.bfgunityandroid;

import com.bigfishgames.bfgunityandroid.NotificationQueueRunner;
import com.unity3d.player.UnityPlayer;

import java.security.MessageDigest;
import java.util.ArrayList;

public class UnityMessageQueueRunner {

    private class Message {
        public String gameObject;
        public String method;
        public String params;

        public Message(String gameObject, String method, String params) {
            this.gameObject = gameObject;
            this.method = method;
            this.params = params;
        }
    }

    private static UnityMessageQueueRunner _instance = null;

    private ArrayList<Message> message_queue = null;

    public static UnityMessageQueueRunner GetInstance() {
        if (_instance == null)
            _instance = new UnityMessageQueueRunner();

        return _instance;
    }

    public static boolean FlushMessageQueue() {
        GetInstance().DoFlushMessageQueue();
        return true;
    }

    private void DoFlushMessageQueue() {
        while (true) {
            if (!(SendMessage())) {
                break;
            }
        }
    }

    private boolean SendMessage() {
        boolean hasMessage = false;
        Message msg = null;

        synchronized(message_queue) {
            if (message_queue.size() > 0) {
                msg = message_queue.remove(0);
                hasMessage = true;
            }
        }

        if (!hasMessage)
            return false;

        UnityPlayer.UnitySendMessage(msg.gameObject, msg.method, msg.params);
        return true;
    }

    public UnityMessageQueueRunner() {
        message_queue = new ArrayList<>();
    }

    public static void AddMessageToQueue(String gameObject, String method, String params) {
        GetInstance().DoAddToQueue(gameObject, method, params);
    }

    public void DoAddToQueue(String gameObject, String method, String params) {
        Message msg = new Message(gameObject, method, params);
        DoAddToQueue(msg);
    }

    public void DoAddToQueue(Message msg) {
        synchronized(message_queue) {
            message_queue.add(msg);
        }
    }
}
