using UnityEngine;
using UnityEngine.UI;

public class UIMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText buttonAcceptLabel;
    [SerializeField] private NSText buttonCancelLabel;
    
    [SerializeField] private Image imageMessage;
    
    [SerializeField] private GameObject btnAccept;
    [SerializeField] private GameObject btnCancel;
    
    private bool isAccept;
    private bool isCancel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMessageWindowModel;
        
        imageMessage.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Image));
        message.gameObject.SetActive(string.IsNullOrEmpty(windowModel.Image));

        if (!string.IsNullOrEmpty(windowModel.Image))
        {
            imageMessage.sprite = IconService.Current.GetSpriteById(windowModel.Image);
        }
        
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

        windowModel.Image = null;

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