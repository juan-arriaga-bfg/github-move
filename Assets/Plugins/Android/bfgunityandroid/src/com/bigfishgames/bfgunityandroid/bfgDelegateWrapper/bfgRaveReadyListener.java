package com.bigfishgames.bfgunityandroid.bfgDelegateWrapper;

import com.bigfishgames.bfglib.NSNotification;
import com.bigfishgames.bfglib.bfgreporting.bfgRave;
import com.bigfishgames.bfgunityandroid.NotificationCenterUnityWrapper;
import java.util.HashMap;

public class bfgRaveReadyListener {
    private static bfgRaveReadyListener sInstance = null;

    public static bfgRaveReadyListener sharedInstance() {
        if (sInstance == null)
        {
            sInstance = new bfgRaveReadyListener();
        }

        return sInstance;
    }
    public void setRaveReadyListenerDelegate()
    {
        bfgRave.sharedInstance().listenForRaveReady(new bfgRave.RaveReadyListener() {
            @Override
            public void onComplete(final String raveId) {
                HashMap<String,String> notificationMap = new HashMap<>();
                notificationMap.put("error", "None");
                notificationMap.put("raveId", raveId);
                NSNotification notification = NSNotification.notificationWithName("BFG_RAVE_READY", notificationMap);

                NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
            }
        });
    }

    private bfgRaveReadyListener() {
        if (bfgRaveReadyListener.sInstance == null)
        {
            sInstance = this;
        }
    }
}
