package com.bigfishgames.bfgunityandroid;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.PixelFormat;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;
import android.view.WindowManager;

import com.bigfishgames.bfglib.NSNotification;
import com.bigfishgames.bfglib.NSNotificationCenter;
import com.bigfishgames.bfglib.bfgGdpr.bfgPolicyListener;
import com.bigfishgames.bfglib.bfgreporting.bfgRave;
import com.bigfishgames.bfglib.bfgreporting.bfgReporting;
import com.bigfishgames.bfglib.deeplinking.bfgDeferredDeepLinkListener;
import com.bigfishgames.bfglib.deeplinking.bfgDeferredDeepLinkTracker;
import com.bigfishgames.bfglib.bfgActivity;
import com.bigfishgames.bfglib.bfgManager;
import com.bigfishgames.bfglib.bfgSettings;
import com.bigfishgames.bfglib.bfgpurchase.bfgPurchase;
import com.bigfishgames.bfgunityandroid.bfgDelegateWrapper.BfgRaveADKDelegate;

import com.bigfishgames.bfgunityandroid.NotificationCenterUnityWrapper;

import com.unity3d.player.UnityPlayer;

import co.ravesocial.sdk.RaveException;
import co.ravesocial.sdk.RaveSocial;
import co.ravesocial.sdk.login.RaveLoginStatusListener;

import java.lang.String;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Map;
import java.util.concurrent.Semaphore;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

// Neskinsoft
import com.bigfishgames.bfgunityandroid.custom.SplashDialog;
import com.bigfishgames.bfgunityandroid.custom.ResourcesHelper;
// End

public class BFGUnityPlayerNativeActivity extends bfgActivity implements bfgDeferredDeepLinkListener {

    protected UnityPlayer mUnityPlayer = null;

    private static final String TAG = "BFGUnityPlayerNA";
    public static String DEFAULT_PRODUCTID = "DEFAULT_PRODUCTID";
    protected List<String> mProductIds = null;

    public void addProductId(String productId) {
        if (mProductIds == null) {
            mProductIds = new ArrayList<String>();
        }

        if (productId == DEFAULT_PRODUCTID) {
            productId = bfgSettings.getString("iap_default_product_id", null);
            if (productId != null) {
                productId = productId.toLowerCase(Locale.getDefault());
            }
        }

        if (productId != null && !mProductIds.contains(productId)) {
            mProductIds.add(productId);
        }
    }

    public bfgPolicyListener bfgPolicyListenerInstance = null;

    private static BFGUnityPlayerNativeActivity _current_instance = null;

    public static BFGUnityPlayerNativeActivity currentInstance() {
        return _current_instance;
    }

    public BFGUnityPlayerNativeActivity() {
        _current_instance = this;
    }

    @Override
    protected void onCreate(final Bundle savedInstanceState) {
        final Activity mActivityInstance = this;
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        // Neskinsoft
		SplashDialog.CreateAndShow(this);
        ResourcesHelper.SetContext(this);
		// End
		
        OnCreate_UnityPlayer();

        bfgPolicyListenerInstance = new BfgPolicyDelegate();
        bfgManager.initializeWithActivity(this, savedInstanceState);
        bfgRave.sharedInstance().setDelegate(new RaveDelegate());
        bfgDeferredDeepLinkTracker.getInstance().setDeferredDeepLinkListener(this);


    }

    protected void OnCreate_UnityPlayer()
    {
        getWindow().takeSurface(null);
        setTheme(android.R.style.Theme_NoTitleBar_Fullscreen);
        getWindow().setFormat(PixelFormat.RGBX_8888);

        mUnityPlayer = new UnityPlayer(this);
        if (mUnityPlayer.getSettings ().getBoolean ("hide_status_bar", true))
            getWindow ().setFlags (WindowManager.LayoutParams.FLAG_FULLSCREEN,
                                   WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(mUnityPlayer);
        mUnityPlayer.requestFocus();
    }
    @Override
    protected void onStart() {
        super.onStart();
        bfgManager.addPolicyListener(bfgPolicyListenerInstance);
    }

    @Override
    protected void onStop() {
        super.onStop();
        bfgManager.removePolicyListener(bfgPolicyListenerInstance);
    }

    @Override
    public void onResume() {
        super.onResume();
        mUnityPlayer.resume();
        mUnityPlayer.windowFocusChanged(true);
        RaveSocial.onStop(this);
        RaveSocial.onStart(this);
    }

    @Override
    public void onPause() {
        super.onPause();
        mUnityPlayer.pause();
    }

    @Override
    public void onDestroy() {
        mUnityPlayer.quit();
        super.onDestroy();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        bfgPurchase.sharedInstance().handleActivityResult(requestCode, resultCode, data);

        if (mProductIds != null) {
            // Call bfgPurchase.acquireProductInformation on product list

            final Semaphore productInfoMutex = new Semaphore(0);

            bfgRunOnUIThread(new Runnable() {
                @Override
                public void run() {
                    bfgPurchase.sharedInstance().acquireProductInformation(mProductIds);

                    bfgUtilsUnityWrapper.releaseMutex(productInfoMutex);
                }
            });

            bfgUtilsUnityWrapper.acquireMutex(productInfoMutex);
        }
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        mUnityPlayer.configurationChanged(newConfig);
        RaveSocial.onStop(this);
        RaveSocial.onStart(this);
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        mUnityPlayer.windowFocusChanged(hasFocus);
    }

    @Override public boolean dispatchKeyEvent(KeyEvent event) {
        if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
            return mUnityPlayer.injectEvent(event);
        return super.dispatchKeyEvent(event);
    }

    @Override public boolean onKeyUp(int keyCode, KeyEvent event)     { return mUnityPlayer.injectEvent(event); }
    @Override public boolean onKeyDown(int keyCode, KeyEvent event)   { return mUnityPlayer.injectEvent(event); }
    @Override public boolean onTouchEvent(MotionEvent event)          { return mUnityPlayer.injectEvent(event); }
    public boolean onGenericMotionEvent(MotionEvent event)  { return mUnityPlayer.injectEvent(event); }

    public static void bfgRunOnUIThread(Runnable runnable) {
        if ((_current_instance != null) && (runnable != null))
            _current_instance.runOnUiThread(runnable);
    }

    private static class BfgRaveExternalCallback implements RaveLoginStatusListener
    {
        @Override
        public void onLoginStatusChanged(RaveLoginStatus raveLoginStatus, RaveException e) {
            NSNotification notification;
            if (raveLoginStatus == RaveLoginStatus.LOGGED_IN) {
                notification = NSNotification.notificationWithName("BFG_RAVE_EXTERNALCALLBACK_LOGGEDIN", null);
            } else if (raveLoginStatus == RaveLoginStatus.LOGGED_OUT) {
                notification = NSNotification.notificationWithName("BFG_RAVE_EXTERNALCALLBACK_LOGGEDOUT", null);
            } else {
                notification = NSNotification.notificationWithName("BFG_RAVE_EXTERNALCALLBACK_LOGINERROR", (e != null) ? e.getMessage() : null);
            }
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }
    }

    private static class BfgPolicyDelegate implements bfgPolicyListener {
        private static BfgPolicyDelegate _current_instance = null;

        private static BfgPolicyDelegate currentInstance() {
            return _current_instance;
        }

        public BfgPolicyDelegate() {
            if (BfgPolicyDelegate._current_instance == null)
            {
                _current_instance = this;
            }
        }

        @Override
        public void willShowPolicies() {
            NSNotification notification = NSNotification.notificationWithName("BFG_POLICY_LISTENER_WILLSHOWPOLICIES", null);
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void onPoliciesCompleted() {
            NSNotification notification = NSNotification.notificationWithName("BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED", null);
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }
    }

    private class RaveDelegate implements bfgRave.bfgRaveDelegate {
        @Override
        public void bfgRaveUserLoginError(Exception e) {
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_USER_LOGIN_ERROR", e));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveSignInCOPPAResult(boolean b) {
            Log.d("bfgRave", "COPPA result (SDK)");
            NSNotification notification;
            if (b == true)
            {
                notification = (NSNotification.notificationWithName("BFG_RAVE_SIGN_IN_COPPA_TRUE", null));
            } else{
                notification = (NSNotification.notificationWithName("BFG_RAVE_SIGN_IN_COPPA_FALSE", null));
            }

            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveSignInCancelled() {
            Log.d("bfgRave", "Sign in cancelled (SDK)");
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_SIGN_IN_CANCELLED", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveSignInSucceeded() {
            Log.d("bfgRave", "Signed in (SDK)");
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_SIGN_IN_SUCCEEDED", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        /**
         * Called when login occurs
         * @param details Details of the login. Contains the previous RaveId, or null if there is
         *                not one available. Also has a flag to indicate whether the login was the
         *                result of a request for cross app login.
         */
        @Override
        public void bfgRaveUserDidLogin(bfgRave.LoginDetails details) {
            Log.d("bfgRave", "Logged in (SDK)");
            Hashtable<String, Object> detailsTable = new Hashtable<String, Object>();
            detailsTable.put("loginViaCAL", details.loginViaCAL);
            detailsTable.put("previousRaveId", (details.previousRaveId != null) ? details.previousRaveId : "");

            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_USER_DID_LOGIN", detailsTable));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveUserDidLogout() {
            Log.d("bfgRave", "Logged out (SDK)");
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_USER_DID_LOGOUT", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveProfileFailedWithError(Exception var1) {
            Log.d("bfgRave", "Profile failed with error (SDK): " + var1.getMessage());
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_PROFILE_FAILED_WITH_ERROR", var1));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveProfileSucceeded() {
            Log.d("bfgRave", "Profile view succeeddd (SDK)");
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_PROFILE_SUCCEEDED", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveProfileCanceled() {
            Log.d("bfgRave", "Profile view canceled (SDK)");
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_PROFILE_CANCELLED", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveChangeDisplayNameDidSucceed() {
            Log.d("bfgRave", "Display name changed successfully (SDK)");
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_CHANGE_DISPLAY_NAME_DID_SUCCEED", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveChangeDisplayNameDidFailWithError(Exception var1) {
            Log.d("bfgRave", "Display name change failed with error (SDK): " +var1.getMessage());
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_CHANGE_DISPLAY_NAME_DID_FAIL_WITH_ERROR", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }

        @Override
        public void bfgRaveSignInFailedWithError(Exception var1) {
            Log.d("bfgRave", "Sign in failed with error (SDK): " + var1.getMessage());
            NSNotification notification = (NSNotification.notificationWithName("BFG_RAVE_SIGN_IN_FAILED_WITH_ERROR", null));
            NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
        }
    }

    @Override
    public void didReceiveDeferredDeepLink(String deepLink, String error, int providerId, long timeSinceLaunchInMillis) {
        String provider = "";
        Map<String, String> deepLinkMap = new HashMap<String, String>();
        switch (providerId) {
            case bfgReporting.BFG_REPORTING_TRACKER_ID_TUNE:
                provider = "TUNE";
                break;
            case bfgReporting.BFG_REPORTING_TRACKER_ID_BIG_FISH:
                provider = "BIGFISH";
                break;
            case bfgReporting.BFG_REPORTING_TRACKER_ID_ALL:
                provider = "ALL - You should never see this text - possible bug!!!";
                break;
            default:
                break;
        }
        String DDLInfo = "Deferred Deep Link Listener:\nProvider: " + provider + "\nDeep Link: " + ((null != deepLink) ? deepLink : "None") + "\nError: " + ((null != error) ? error : "None") + "TimeInMilli: " + Long.toString(timeSinceLaunchInMillis);
        Log.d("DeferredDeepLink", DDLInfo);
        deepLinkMap.put("provider", (provider != null) ? provider : "");
        deepLinkMap.put("deepLinkString", (deepLink != null) ? deepLink : "");
        NSNotification notification = (NSNotification.notificationWithName("NOTIFICATION_DEFERREDDEEPLINK_DIDRECEIVEDEFERREDDEEPLINK", deepLinkMap));
        NotificationCenterUnityWrapper.GetInstance().HandleNotification(notification);
    }

}
