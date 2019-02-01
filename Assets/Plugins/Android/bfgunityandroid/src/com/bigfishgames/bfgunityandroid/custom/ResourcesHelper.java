package com.bigfishgames.bfgunityandroid.custom;

import android.app.Activity;
import android.content.Context;
import android.content.res.Resources;
import android.os.Build;
import android.util.Log;

/**
 * Created by keht on 06.05.2016.
 */
public class ResourcesHelper
{
    static final String TAG = "ResourcesHelper";

    private static Context sContext;

    public static void SetContext(Activity activity)
    {
        sContext = activity;
    }

    public static int GetNotificationIconId(Context context) {
        int ret = -1;
        try
        {
            // No LARGE notification icon for 4.x
            if (Build.VERSION.SDK_INT < Build.VERSION_CODES.LOLLIPOP) {
                return 0;
            }

            Resources res = context.getResources();
            ret = res.getIdentifier("ic_notification_large", "drawable", context.getPackageName());
        }
        catch (Exception e)
        {
            Log.e(TAG, e.getMessage());
        }
        return ret;
    }

    public static int GetNotificationIconId()
    {
        return GetNotificationIconId(sContext);
    }

    public static String GetString(String key)
    {
        String ret = "";
        try
        {
            Resources res = sContext.getResources();
            int id = res.getIdentifier(key, "string", sContext.getPackageName());
            ret = res.getString(id, sContext.getPackageName());
        }
        catch (Exception e)
        {
            Log.e(TAG, e.getMessage());
        }
        return ret;
    }
}