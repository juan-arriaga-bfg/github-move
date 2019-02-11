package com.bigfishgames.bfgunityandroid.custom;

import android.app.Dialog;
import android.content.Context;
import android.os.Build;
import android.view.View;
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
        final SplashDialog dlg = new SplashDialog(context, android.R.style.Theme_NoTitleBar_Fullscreen);
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

    @Override
    protected void onStart() {
        super.onStart();

        hideSystemUI();
    }

    private void hideSystemUI() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            Window window = getWindow();
            if (window == null) {
                return;
            }

            // Enables regular immersive mode.
            // For "lean back" mode, remove SYSTEM_UI_FLAG_IMMERSIVE.
            // Or for "sticky immersive," replace it with SYSTEM_UI_FLAG_IMMERSIVE_STICKY
            View decorView = getWindow().getDecorView();
            decorView.setSystemUiVisibility(
                    View.SYSTEM_UI_FLAG_IMMERSIVE
                            // Set the content to appear under the system bars so that the
                            // content doesn't resize when the system bars hide and show.
                            | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                            | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                            // Hide the nav bar and status bar
                            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                            | View.SYSTEM_UI_FLAG_FULLSCREEN);
        }
    }
}
