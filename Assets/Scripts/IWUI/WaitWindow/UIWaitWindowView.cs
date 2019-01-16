using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class UIWaitWindowView : IWUIWindowView
{
    [IWUIBinding("#Text")] private TextMeshProUGUI label;
    [IWUIBinding("#Back")] private CanvasGroup canvasGroup;
    
    private readonly object ANIMATION_ID = new object();
    
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
        
    }

    public void SetText(string text)
    {
        label.text = text ?? string.Empty;
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
        windowView.SetText("Please wait....");
        
        return windowView;
    }

    public static void Hide()
    {
        var window = UIService.Get.GetShowedWindowByName(UIWindowType.WaitWindow);
        
        if (window != null)
        {
            if (!window.WindowController.IsClosing)
            {
                window.WindowController.CloseCurrentWindow();
            }
        }
    }
}
