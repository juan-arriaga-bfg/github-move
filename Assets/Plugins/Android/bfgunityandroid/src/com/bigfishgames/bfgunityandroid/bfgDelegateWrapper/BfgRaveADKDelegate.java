package com.bigfishgames.bfgunityandroid.bfgDelegateWrapper;

import com.bigfishgames.bfglib.NSNotification;
import com.bigfishgames.bfglib.bfgreporting.bfgRave;
import com.bigfishgames.bfglib.bfgreporting.bfgRaveAppDataKeyDelegate;
import com.bigfishgames.bfgunityandroid.NotificationCenterUnityWrapper;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import co.ravesocial.sdk.RaveException;

/**
 * Created by alex.bowns on 8/6/18.
 */
public class BfgRaveADKDelegate implements bfgRaveAppDataKeyDelegate {
    private static BfgRaveADKDelegate sInstance = null;

    public static BfgRaveADKDelegate sharedInstance() {
        if (sInstance == null)
        {
            sInstance = new BfgRaveADKDelegate();
        }

        return sInstance;
    }

    public void setRaveAppDataKeyDelegate()
    {
        bfgRave.setRaveAppDataKeyDelegate(sInstance);
    }

    private BfgRaveADKDelegate() {
        if (BfgRaveADKDelegate.sInstance == null)
        {
            sInstance = this;
        }
    }

    @Override
    public void bfgRaveAppDataKeyDidChange(String currentAppDataKey, String previousKey) {
        HashMap<String,String> notificationMap = new HashMap<>();
        notificationMap.put("currentAppDataKey", (currentAppDataKey != null) ? currentAppDataKey : "");
        notificationMap.put("previousKey", (previousKey != null) ? previousKey : "");
        NSNotification notification = NSNotification.notificationWithName("RAVE_ADK_DELEGATE_ADK_DID_CHANGE", notificationMap);

        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);

    }

    @Override
    public void bfgRaveAppDataKeyDidReturnUnresolvedKeys(List<String> unresolvedKeys, String currentAppDataKey) {
        HashMap<String,Object> notificationMap = new HashMap<>();
        notificationMap.put("unresolvedKeys", (unresolvedKeys != null) ? unresolvedKeys : new ArrayList<String>());
        notificationMap.put("currentAppDataKey", (currentAppDataKey != null) ? currentAppDataKey : "");
        NSNotification notification = NSNotification.notificationWithName("RAVE_ADK_DELEGATE_ADK_UNRESOLVED_KEYS", notificationMap);

        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }

    @Override
    public void bfgRaveFetchCurrentAppDataKeyDidSucceed(String currentAppDataKey) {
        HashMap<String,String> notificationMap = new HashMap<>();
        notificationMap.put("currentAppDataKey", (currentAppDataKey != null) ? currentAppDataKey : "");
        NSNotification notification = NSNotification.notificationWithName("RAVE_ADK_DELEGATE_FETCH_CURRENT_SUCCEEDED", notificationMap);

        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }

    @Override
    public void bfgRaveFetchCurrentAppDataKeyDidFailWithError(RaveException error) {
        HashMap<String,Object> notificationMap = new HashMap<>();
        notificationMap.put("errorCode", (error != null ) ? (long)error.getErrorCode() : (long)-1);
        notificationMap.put("errorDescription", (error != null ) ? error.getLocalizedMessage() : "");
        NSNotification notification = NSNotification.notificationWithName("RAVE_ADK_DELEGATE_FETCH_CURRENT_FAILED", notificationMap);

        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }

    @Override
    public void bfgRaveSelectAppDataKeyDidSucceed() {
        NSNotification notification = NSNotification.notificationWithName("RAVE_ADK_DELEGATE_ADK_SELECT_SUCCEEDED", null);

        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }

    @Override
    public void bfgRaveSelectAppDataKeyDidFailWithError(RaveException error) {
        HashMap<String,Object> notificationMap = new HashMap<>();
        notificationMap.put("errorCode", (error != null ) ? (long)error.getErrorCode() : (long)-1);
        notificationMap.put("errorDescription", (error != null ) ? error.getLocalizedMessage() : "");
        NSNotification notification = NSNotification.notificationWithName("RAVE_ADK_DELEGATE_ADK_SELECT_FAILED", notificationMap);

        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }
}
