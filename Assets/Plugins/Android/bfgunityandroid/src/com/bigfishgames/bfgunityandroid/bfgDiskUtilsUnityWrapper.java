package com.bigfishgames.bfgunityandroid;

/**
 * Created by Thomas.Schessler on 10/19/18.
 */

import com.bigfishgames.bfglib.bfgutils.bfgDiskUtils;

public class bfgDiskUtilsUnityWrapper {
    public static long availableDiskSpace()
    {
        return bfgDiskUtils.availableDiskSpace();
    }
}
