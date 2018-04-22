using UnityEngine;
using UnityEngine.UI;

public class UIRobberyWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnSendLabel;
    [SerializeField] private NSText btnClaimLabel;
    
    [SerializeField] private Button btnSend;
    [SerializeField] private Button btnClaim;

    [SerializeField] private Image round;
    [SerializeField] private Image chest;

    [SerializeField] private RectTransform dot;
    
    [SerializeField] private RectTransform line;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIRobberyWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnSendLabel.Text = windowModel.SendText;
        btnClaimLabel.Text = windowModel.ClaimText;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIRobberyWindowModel;
        
    }

    public void OnClickSend()
    {
        
    }
    
    public void OnClickClaim()
    {
        
    }
    
    public void OnClickQuestion()
    {
        
    }
}
