using System;
using UnityEngine;
using DG.Tweening;

public class UIGenericPopupWindowView : UIGenericWindowView
{
    [IWUIBinding("#Body")] protected RectTransform body;
    
    [IWUIBindingNullable("#Title")] protected NSText title;
    [IWUIBindingNullable("#Message")] protected NSText message;
    
    public override void AnimateShow()
    {
        base.AnimateShow();
        PlayShowSound();
        
        body.anchoredPosition = new Vector2(0f, -Screen.height);
        
        DOTween.Kill(body);
        var sequence = DOTween.Sequence().SetId(body);
        sequence.Append(body.DOAnchorPos(new Vector2(0f, 0f), 0.5f).SetEase(Ease.OutBack));
    }

    protected virtual void PlayShowSound()
    {
        NSAudioService.Current.Play(SoundId.PopupOpen);
    }

    public override void AnimateClose()
    {
        base.AnimateClose();
        PlayCloseSound();
        
        DOTween.Kill(body);
        var sequence = DOTween.Sequence().SetId(body);
        sequence.Append(body.DOAnchorPos(new Vector2(0f, -Screen.height), 0.5f).SetEase(Ease.InBack));
    }
    
    protected virtual void PlayCloseSound()
    {
        NSAudioService.Current.Play(SoundId.PopupClose);
    }

    protected void SetTitle(string text)
    {
        if(title != null) title.Text = text;
    }
    
    protected void SetMessage(string text)
    {
        if(message != null) message.Text = text;
    }
    
    protected void InitButtonBase(UIButtonViewController btn, Action onClick)
    {
        btn.ToState(GenericButtonState.Active)
            .OnClick(onClick);
    }
}