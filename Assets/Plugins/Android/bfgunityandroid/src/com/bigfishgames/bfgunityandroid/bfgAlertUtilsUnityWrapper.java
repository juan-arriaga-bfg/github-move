//
//  bfgAlertUtilsUnityWrapper.java
//
//  Created by Zira Cook on 10/12/18.
//

package com.bigfishgames.bfgunityandroid;

import com.bigfishgames.bfglib.bfgutils.bfgAlertUtils;
import android.support.v7.app.AlertDialog;

public class bfgAlertUtilsUnityWrapper {

	//@Nullable final String title, @Nullable final DialogInterface.OnClickListener listener
	public static void showGenericErrorAlert (String title)
	{
		final String titleFinal = title;
		//bfgAlertUtils.showGenericErrorAlert(titleFinal);
	}

	//@Nullable final String title,@NonNull final String message,@Nullable final DialogInterface.OnClickListener listener
	public static void showCustomAlert  (String title, String message)
	{
		final String titleFinal = title;
		final String messageFinal = message;
		//bfgAlertUtils.showCustomAlert(titleFinal, messageFinal);
	}

	//@NonNull final Activity activity,@NonNull final AlertDialog dialog
	public static void showWithFixedSizeText ()
	{
		//final Activity mActivity = UnityPlayer.currentActivity;
		//final AlertDialog mDialog = new AlertDialog();
		//bfgAlertUtils.showWithFixedSizeText(mActivity, mDialog);
	}

	//@NonNull final Activity activity,@NonNull final AlertDialog dialog
	public static void fixFontSize ()
	{
		//final Activity mActivity = UnityPlayer.currentActivity;
		//final AlertDialog mDialog = new AlertDialog();
		//bfgAlertUtils.fixFontSize(mActivity, mDialog);
	}
}
