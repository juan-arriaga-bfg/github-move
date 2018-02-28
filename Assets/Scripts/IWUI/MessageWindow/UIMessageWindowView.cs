using UnityEngine;
using DG.Tweening;

public class UIMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText buttonAcceptLabel;
    [SerializeField] private NSText buttonCancelLabel;
    
    [SerializeField] private GameObject btnAccept;
    [SerializeField] private GameObject btnCancel;
    
    private bool isAccept;
    private bool isCancel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMessageWindowModel;
        
        isAccept = false;
        isCancel = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
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
}