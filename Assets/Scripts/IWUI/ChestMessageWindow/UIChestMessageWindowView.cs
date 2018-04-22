using UnityEngine;

public class UIChestMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnFastLabel;
    [SerializeField] private NSText btnSlowLabel;
    
    [SerializeField] private GameObject btnSlow;

    private bool isFast;
    private bool isSlow;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestMessageWindowModel;

        isFast = isSlow = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        btnFastLabel.Text = windowModel.FastButtonText;
        btnSlowLabel.Text = windowModel.SlowButtonText;
        
        btnSlow.SetActive(windowModel.IsShowSlowButton);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIChestMessageWindowModel;
        windowModel.Chest = null;

        if (isFast && windowModel.OnBoost != null)
        {
            windowModel.OnBoost();
            return;
        }
        
        if (isSlow && windowModel.OnStart != null) windowModel.OnStart();
    }
    
    
    public void FastClick()
    {
        var windowModel = Model as UIChestMessageWindowModel;

        CurrencyHellper.Purchase(windowModel.Chest.Currency, 1, windowModel.Chest.Def.Price, success =>
        {
            if (!success) return;
            isFast = true;
            Controller.CloseCurrentWindow();
        });
    }
    
    public void SlowClick()
    {
        isSlow = true;
        Controller.CloseCurrentWindow();
    }
}
