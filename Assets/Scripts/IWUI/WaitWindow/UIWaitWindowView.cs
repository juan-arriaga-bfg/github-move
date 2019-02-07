using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class UIWaitWindowView : IWUIWindowView
{
    [IWUIBinding("#Text")] private TextMeshProUGUI label;
    [IWUIBinding("#Back")] private CanvasGroup canvasGroup;
    
    private readonly object ANIMATION_ID = new object();
    private readonly object TIMEOUT_ID = new object();

    private float timeout;

    private bool hideOnFocus;
    
    public override void AnimateShow()
    {
        base.AnimateShow();
        
        Animate(true);
    }

    public override void AnimateClose()
    {
        base.AnimateClose();
        NSAudioService.Current.Play(SoundId.PopupClose);

        Animate(false);
    }

    private void Animate(bool visible)
    {
        DOTween.Kill(ANIMATION_ID);

        if (visible)
        {
            canvasGroup.alpha = 0;
        }

        canvasGroup.DOFade(visible ? 1 : 0, 0.2f).SetId(ANIMATION_ID);
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIWaitWindowModel windowModel = Model as UIWaitWindowModel;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIWaitWindowModel windowModel = Model as UIWaitWindowModel;
        
        DOTween.Kill(TIMEOUT_ID);
    }

    public void SetText(string text)
    {
        label.text = text ?? string.Empty;
    }
    
    public UIWaitWindowView SetTimeout(float seconds)
    {
        DOTween.Kill(TIMEOUT_ID);
        
        DOTween.Sequence()
               .SetId(TIMEOUT_ID)
               .InsertCallback(seconds, () =>
                {
                    Controller.CloseCurrentWindow();
                });

        return this;
    }
    
    public UIWaitWindowView HideOnFocus()
    {
        hideOnFocus = true;
        return this;
    }

    public static UIWaitWindowView Show()
    {
        IWUIWindow window = UIService.Get.GetShowedWindowByName(UIWindowType.WaitWindow);
        
        if (window != null)
        {
            return (UIWaitWindowView)window.CurrentView;
        }

        window = UIService.Get.ShowWindow(UIWindowType.WaitWindow);
        
        UIWaitWindowView windowView = (UIWaitWindowView) window.CurrentView;
        windowView.SetText(LocalizationService.Get("window.wait.message", "window.wait.message"));
        windowView.timeout = 0;
        windowView.hideOnFocus = false;
        
        return windowView;
    }

    public static void Hide()
    {
        var window = UIService.Get.GetShowedWindowByName(UIWindowType.WaitWindow);
        
        if (window != null)
        {
            var controller = window.WindowController;
            
            if (!controller.IsClosing)
            {
                controller.ForceStopShowing();
                controller.CloseCurrentWindow();
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hideOnFocus && hasFocus)
        {
            hideOnFocus = false;
            Controller.CloseCurrentWindow();
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (hideOnFocus && !paused)
        {
            hideOnFocus = false;
            Controller.CloseCurrentWindow();
        }
    }
}
