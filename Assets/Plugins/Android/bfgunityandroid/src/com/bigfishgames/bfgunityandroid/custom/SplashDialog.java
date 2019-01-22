package com.bigfishgames.bfgunityandroid.custom;

import android.app.Dialog;
import android.content.Context;
import android.view.Window;
import android.widget.FrameLayout;
import android.widget.ImageView;

public class SplashDialog extends Dialog
{
    private static SplashDialog instance;

    private SplashDialog(Context context, int themeResId) {
        super(context, themeResId);
    }

    @Override
    public void onBackPressed()  {
        // Back key is not allowed!
    }

    public static void Kill() {
       instance.cancel();
    }

    public static void CreateAndShow(Context context) {
        final SplashDialog dlg = new SplashDialog(context, android.R.style.Theme_DeviceDefault_NoActionBar_Fullscreen);
        dlg.requestWindowFeature(Window.FEATURE_NO_TITLE);

        FrameLayout imageLayout = new FrameLayout(dlg.getContext());
        imageLayout.setLayoutParams(new FrameLayout.LayoutParams(
                FrameLayout.LayoutParams.MATCH_PARENT,
                FrameLayout.LayoutParams.MATCH_PARENT));

        ImageView imageView = new ImageView(dlg.getContext());
        imageView.setImageResource(com.bigfishgames.bfgunityandroid.R.drawable.bfg_logo);
        imageView.setScaleType(ImageView.ScaleType.CENTER_CROP);

        imageLayout.addView(imageView);

        dlg.setContentView(imageLayout);
        dlg.show();

        instance = dlg;
    }
}
