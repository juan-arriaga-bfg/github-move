package neskinsoft.core.common;

import android.app.Activity;
import android.content.Context;
import android.content.res.Resources;

/**
 * Created by keht on 06.05.2016.
 */
public class ResourcesHelper
{
    private static Activity sActivity;

    public static void SetAcivity(Activity activity)
    {
        sActivity = activity;
    }

    public static int GetNotificationIcon(Context context) {
        int ret = -1;
        try
        {
            Resources res = context.getResources();
            ret = res.getIdentifier("ic_notification_small", "drawable", context.getPackageName());
        }
        catch (Exception e)
        {
            Logger.e(e);
        }
        return ret;
    }

    public static int GetNotificationIcon()
    {
        return GetNotificationIcon(sActivity);
    }

    public static String GetString(String key)
    {
        String ret = "";
        try
        {
            Resources res = sActivity.getResources();
            int id = res.getIdentifier(key, "string", sActivity.getPackageName());
            ret = res.getString(id, sActivity.getPackageName());
        }
        catch (Exception e)
        {
            Logger.e(e);
        }
        return ret;
    }
}