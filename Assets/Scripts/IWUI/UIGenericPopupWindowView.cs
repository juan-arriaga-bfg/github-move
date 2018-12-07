using System;
using UnityEngine;
using DG.Tweening;

public class UIGenericPopupWindowView : UIGenericWindowView
{
    [IWUIBinding("#Title")] private NSText titleLabel;
    [IWUIBinding("#Message")] private NSText messageLabel;
    
    [IWUIBinding("#Body")] private RectTransform body;
    
    [SerializeField] private NSText title;
    [SerializeField] [Obsolete] protected NSText message;
    
    [SerializeField] private RectTransform viewAnchor;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UISampleWindowModel windowModel = Model as UISampleWindowModel;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UISampleWindowModel windowModel = Model as UISampleWindowModel;
    }

    public override void AnimateShow()
    {
        base.AnimateShow();

        var target = body == null ? viewAnchor : body;
        
        target.anchoredPosition = new Vector2(0f, -Screen.height);
        
        DOTween.Kill(target);
        var sequence = DOTween.Sequence().SetId(target);
        sequence.Append(target.DOAnchorPos(new Vector2(0f, 0f), 0.5f).SetEase(Ease.OutBack));
    }

    public override void AnimateClose()
    {
        base.AnimateClose();
        
        var target = body == null ? viewAnchor : body;
        
        DOTween.Kill(target);
        var sequence = DOTween.Sequence().SetId(target);
        sequence.Append(target.DOAnchorPos(new Vector2(0f, -Screen.height), 0.5f).SetEase(Ease.InBack));
    }

    protected void SetTitle(string text)
    {
        if(title != null) title.Text = text;
        if(titleLabel != null) titleLabel.Text = text;
    }
    
    protected void SetMessage(string text)
    {
        if(message != null) message.Text = text;
        if(messageLabel != null) messageLabel.Text = text;
    }
}