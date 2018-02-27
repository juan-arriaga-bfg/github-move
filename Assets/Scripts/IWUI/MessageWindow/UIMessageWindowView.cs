using UnityEngine;
using DG.Tweening;

public class UIMessageWindowView : IWUIWindowView
{
    [SerializeField] private NSText title;
    [SerializeField] private NSText message;
    
    [SerializeField] private NSText buttonAcceptLabel;
    [SerializeField] private NSText buttonCancelLabel;
    
    [SerializeField] private GameObject btnAccept;
    [SerializeField] private GameObject btnCancel;
    
    [SerializeField] private RectTransform viewAnchor;
    
    private bool isAccept;
    private bool isCancel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMessageWindowModel;
        
        isAccept = false;
        isCancel = false;

        title.Text = windowModel.Title;
        message.Text = windowModel.Message;
        
        buttonAcceptLabel.Text = windowModel.AcceptLabel;
        buttonCancelLabel.Text = windowModel.CancelLabel;
        
        btnAccept.SetActive(windowModel.OnAccept != null);
        btnCancel.SetActive(windowModel.OnCancel != null);
    }

    public override void OnViewCloseCompleted()
    {   
        base.OnViewCloseCompleted();
        
        var windowModel = Model as UIMessageWindowModel;

        if (isAccept && windowModel.OnAccept != null) windowModel.OnAccept();
        if (isCancel && windowModel.OnCancel != null) windowModel.OnCancel();
    }

    public void OnClickAccept()
    {
        isAccept = true;
    }
    
    public void OnClickCancel()
    {
        isCancel = true;
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();

        viewAnchor.anchoredPosition = new Vector2(0f, -Screen.height);
        
        DOTween.Kill(viewAnchor);
        var sequence = DOTween.Sequence().SetId(viewAnchor);
        sequence.Append(viewAnchor.DOAnchorPos(new Vector2(0f, 0f), 0.5f).SetEase(Ease.OutBack));
    }

    public override void AnimateClose()
    {
        base.AnimateClose();

        DOTween.Kill(viewAnchor);
        var sequence = DOTween.Sequence().SetId(viewAnchor);
        sequence.Append(viewAnchor.DOAnchorPos(new Vector2(0f, -Screen.height), 0.5f).SetEase(Ease.InBack));
    }
}