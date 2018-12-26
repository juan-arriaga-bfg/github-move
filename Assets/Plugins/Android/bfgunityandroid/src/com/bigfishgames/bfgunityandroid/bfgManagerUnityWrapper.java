package com.bigfishgames.bfgunityandroid;

import java.util.ArrayList;
import java.util.concurrent.Semaphore;

import com.bigfishgames.bfglib.NSNotification;
import com.bigfishgames.bfglib.NSNotificationCenter;
import com.bigfishgames.bfglib.bfgCcs.manager.bfgCcsManager;
import com.bigfishgames.bfglib.bfgGdpr.bfgGdprManager;
import com.bigfishgames.bfglib.bfgGdpr.bfgPolicyListener;
import com.bigfishgames.bfglib.bfgManager;
import com.bigfishgames.bfglib.bfgpurchase.bfgPurchase;
import com.bigfishgames.bfglib.bfgutils.bfgGraphics.bfgAnchorLocation;
import com.bigfishgames.bfglib.bfgutils.bfgGraphics.bfgRect;
import com.unity3d.player.UnityPlayer;

import com.facebook.FacebookSdk;

import android.app.Activity;
import android.util.Log;

public class bfgManagerUnityWrapper {

    private static final String TAG = "bfgManagerUnityWrapper";

    /*
     * TODO Find corresponding Android SDK method or remove altogether.
     public static long sessionCount() {
     // No equivalent method in Android SDK's bfgManager?
     return 0;
     }
     */

    /*
     * TODO Find corresponding Android SDK method or remove altogether.
     public static boolean isInitialLaunch() {
     // No equivalent method in Android SDK's bfgManager?
     return false;
     }
     */

    public static boolean isInitialized() {
        return bfgManager.isInitialized();
    }

    public static long userID() {
        return bfgManager.sharedInstance().userID();
    }

    public static void showMoreGames() {
        Log.d("bfgManagerUnityWrapper", "In bfgManagerUnityWrapper.showMoreGames.");

        BFGUnityPlayerNativeActivity.bfgRunOnUIThread
        (
         new Runnable()
         {
            @Override
            public void run()
            {
                bfgManager.sharedInstance().showMoreGames();
            }
        }
         );
    }

    /*
     * TODO Find corresponding Android SDK method or remove altogether.
     public static void removeMoreGames() {
     // No equivalent method in Android SDK's bfgManager?
     }
     */

    public static void showSupport() {
        bfgManager.sharedInstance().showSupport();
    }

    public static void showPrivacy() {
        bfgManager.sharedInstance().showPrivacy();
    }

    public static void showTerms() {
        bfgManager.sharedInstance().showTerms();
    }

    public static void showWebBrowser(String startPage) {
        bfgManager.sharedInstance().showWebBrowser(startPage);
    }

    public static boolean checkForInternetConnection() {

        final BoolReturn boolreturn = new BoolReturn();
        final Semaphore mutex = new Semaphore(0);

        BFGUnityPlayerNativeActivity.bfgRunOnUIThread
        (
         new Runnable()
         {
            @Override
            public void run()
            {
                boolean retval = bfgManager.sharedInstance().checkForInternetConnection(false);
                boolreturn._val = retval;
                mutex.release();
            }
        }
         );

        try {
            mutex.acquire();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        return boolreturn._val;
    }

    public static boolean checkForInternetConnectionAndAlert(boolean displayAlert) {
        return bfgManager.sharedInstance().checkForInternetConnection(displayAlert);
    }

    public static boolean startBranding() {
        Log.d("bfgManagerUnityWrapper", "Entering bfgManagerUnityWrapper::startBranding.");
        //boolean retval = true;
        if (bfgManager.sharedInstance() == null)
            Log.d("bfgManagerUnityWrapper", "bfgManager.sharedInstance() RETURNED NULL!");
        boolean retval = bfgManager.sharedInstance().startBranding();
        Log.d("bfgManagerUnityWrapper", "Returned from bfgManager::startBranding.  Attempting return to CSharp layer.");
        return retval;
    }

    public static void showCcsButton(float[] buttonBoundsArray)
    {
        final Activity mActivity = UnityPlayer.currentActivity;
        final bfgRect buttonBounds = new bfgRect(buttonBoundsArray[0], buttonBoundsArray[1], buttonBoundsArray[2], buttonBoundsArray[3] );
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                bfgManager.sharedInstance().showCcsButton(mActivity, buttonBounds);
            }
        });
    }

    public static void showCcsButton(float[] buttonBoundsArray, String gameLocation) {
        final Activity mActivity = UnityPlayer.currentActivity;
        final String finalGameLocation = gameLocation;
        final bfgRect buttonBounds = new bfgRect(buttonBoundsArray[0], buttonBoundsArray[1], buttonBoundsArray[2], buttonBoundsArray[3] );
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                bfgManager.sharedInstance().showCcsButton(mActivity, buttonBounds, finalGameLocation);
            }
        });

    }

    public static void setUserID(long userID)
    {
        bfgManager.sharedInstance().setUserID(userID);
    }

    public static void hideCcsButton() {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                bfgManager.sharedInstance().hideCcsButton(mActivity);
            }
        });
    }

    // We do not do anything for this native plugin method since BFGUnityPlayerNativeActivity.java creates a bfgPolicyListener instance in the OnCreate() method. This is done because the instance of policyListener will be added as early in the applications lifecycle as possible.
    //    public static void addPolicyListener()
    //    {
    //        if (BFGUnityPlayerNativeActivity.currentInstance().bfgPolicyListenerInstance != null) {
    //            bfgManager.sharedInstance().addPolicyListener(BFGUnityPlayerNativeActivity.currentInstance().bfgPolicyListenerInstance);
    //        }
    //    }

    // Since addPolicyListener only adds a single bfgPolicyListener Instance (see addPolicyListener above), we know to remove the specific instance of bfgPolicyListerInstance in BFGUnityPlayerNativeActivity
    public static void removePolicyListener()
    {
        if (BFGUnityPlayerNativeActivity.currentInstance().bfgPolicyListenerInstance != null) {
            bfgManager.sharedInstance().removePolicyListener(BFGUnityPlayerNativeActivity.currentInstance().bfgPolicyListenerInstance);
        }
    }

    public static boolean didAcceptPolicyControl(String policyControlString)
    {
        return bfgManager.sharedInstance().didAcceptPolicyControl(policyControlString);
    }

    public static void setLimitEventAndDataUsage(boolean limitData)
    {
            final Activity mActivity = UnityPlayer.currentActivity;
            FacebookSdk.setLimitEventAndDataUsage(mActivity, limitData);
    }

    public static boolean isShowingCcsButton()
    {
        return bfgManager.sharedInstance().isShowingCcsButton();
    }

    public static float[] createCcsButtonBounds(float widthPercent, String  horizontalAnchor, String verticalAnchor)
    {
        final Activity mActivity = UnityPlayer.currentActivity;
        bfgAnchorLocation horizontal = null;
        bfgAnchorLocation vertical = null;

        try {
            horizontal = bfgAnchorLocation.valueOf(horizontalAnchor);
            vertical = bfgAnchorLocation.valueOf(verticalAnchor);
        } catch (IllegalArgumentException e) {
            Log.w(TAG, "Cannot find the enum: " + horizontalAnchor);
            e.printStackTrace();
            return null;
        }

        if (vertical != null && horizontal != null) {
            bfgRect ccsRect = bfgManager.sharedInstance().createCcsButtonBounds(mActivity, widthPercent, horizontal, vertical);
            float[] ccsRectBoundsArray = {ccsRect.x, ccsRect.y, ccsRect.w, ccsRect.h};
            return ccsRectBoundsArray;
        } else {
            return null;
        }
    }

    public static boolean launchSDKByURLScheme(String urlScheme) {
        return bfgManager.sharedInstance().navigateToURL(urlScheme);
    }

}
