package com.bigfishgames.bfgunityandroid.bfgDelegateWrapper;

import android.support.annotation.Nullable;

import com.bigfishgames.bfglib.NSNotification;
import com.bigfishgames.bfglib.deeplinking.bfgDeepLinkListener;
import com.bigfishgames.bfgunityandroid.NotificationCenterUnityWrapper;

import java.util.HashMap;
import java.util.Map;

/**
 * Created by alex.bowns on 2/26/19.
 */

public class BfgDeeplinkListenerWrapper implements bfgDeepLinkListener
{
    private static BfgDeeplinkListenerWrapper currentInstance = null;

    public static BfgDeeplinkListenerWrapper sharedInstance()
    {
        if (currentInstance == null)
        {
            currentInstance = new BfgDeeplinkListenerWrapper();
        }

        return currentInstance;
    }

    private BfgDeeplinkListenerWrapper()
    {

    }

    @Override
    public void onDeepLinkReceived(@Nullable String deepLink, @Nullable Map<String, String> conversationData, @Nullable String error)
    {
        Map<String, String> deepLinkMap = new HashMap<String, String>();
        deepLinkMap.putAll(conversationData);
        deepLinkMap.put("deepLinkString", (deepLink != null) ? deepLink : "");
        deepLinkMap.put("error", (error != null) ? error : "");
        NSNotification notification = (NSNotification.notificationWithName("NOTIFICATION_DEEPLINK_ONDEEPLINKRECEIVED", deepLinkMap));
        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }
}
