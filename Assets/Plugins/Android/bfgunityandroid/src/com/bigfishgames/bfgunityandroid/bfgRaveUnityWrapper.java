package com.bigfishgames.bfgunityandroid;

import android.app.Activity;

import com.bigfishgames.bfglib.bfgreporting.bfgRave;
import com.unity3d.player.UnityPlayer;
/**
 * Created by alex.bowns on 4/3/17.
 */

public class bfgRaveUnityWrapper {

    public static void fetchCurrentAppDataKey()
    {
      bfgRave.sharedInstance().fetchCurrentAppDataKey();
    }

    public static void selectRaveAppDataKey(String key)
    {
      bfgRave.sharedInstance().selectRaveAppDataKey(key);
    }

    public static boolean isRaveInitialized() {
        return bfgRave.sharedInstance().isRaveInitialized();
    }

    public static boolean isCurrentGuest() {
        return bfgRave.sharedInstance().isCurrentGuest();
    }

    public static String currentRaveId() {
        return bfgRave.sharedInstance().currentRaveId();
    }

    public static String lastRaveId() {
        return bfgRave.sharedInstance().lastRaveId();
    }

    public static boolean isCurrentAuthenticated()
    {
        return bfgRave.sharedInstance().isCurrentAuthenticated();
    }

    public static boolean isCurrentPersonalized()
    {
        return bfgRave.sharedInstance().isCurrentPersonalized();
    }

    public static boolean isLastGuest(){
        return bfgRave.sharedInstance().isLastGuest();
    }

    public static void presentSignIn() {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().presentSignIn(mActivity);
            }
        });
    }

    public static void presentSignInWithOrigin(final String origin) {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().presentSignIn(mActivity, origin);
            }
        });
    }

    public static void presentProfile() {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().presentProfile(mActivity);
            }
        });
    }

    public static void presentProfileWithOrigin(final String origin) {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().presentProfile(mActivity, origin);
            }
        });
    }

    public static void presentNewsletterSignup() {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().presentNewsletterSignUp(mActivity);
            }
        });
    }

    public static void presentNewsletterSignupWithOrigin(final String origin) {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().presentNewsletterSignUp(mActivity, origin);
            }
        });
    }

    public static void performCrossAppLogin(Activity activity) {
        final Activity mActivity = UnityPlayer.currentActivity;
        mActivity.runOnUiThread(new Runnable() {
            public void run() {
                bfgRave.sharedInstance().performCrossAppLogin(mActivity);
            }
        });
    }

    public static void logoutCurrentUser() {
        bfgRave.sharedInstance().logoutCurrentUser();
    }

    public static String currentRaveDisplayName() {
        return bfgRave.sharedInstance().currentRaveDisplayName();
    }

    public static void changeRaveDisplayName(String raveDisplayName) {
        bfgRave.sharedInstance().changeRaveDisplayName(raveDisplayName);
    }
}
