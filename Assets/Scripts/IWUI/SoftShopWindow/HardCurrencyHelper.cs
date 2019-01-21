using System;

public class HardCurrencyHelper
{
    private Action<bool, string> onComplete;
    
    public HardCurrencyHelper()
    {
        IapService.Current.OnPurchaseOK += OnPurchaseOk;
        IapService.Current.OnPurchaseFail += OnPurchaseFail;
    }

    private void OnPurchaseFail(string productId, IapErrorCode error)
    {
        UIWaitWindowView.Hide();
        onComplete?.Invoke(false, productId);
        onComplete = null;

        ShowError(error);
    }

    private void ShowError(IapErrorCode error)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

        model.Title = LocalizationService.Get("iap.failed.title",       "iap.failed.title");
        model.Message = LocalizationService.Get("iap.failed.message",   "iap.failed.message");
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");

        model.OnAccept = () => { };

        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    private void OnPurchaseOk(string productid, string receipt)
    {
        UIWaitWindowView.Hide();
        onComplete?.Invoke(true, productid);
        onComplete = null;

        ProvideReward(productid);
    }

    private void ProvideReward(string productid)
    {
        var reward = 
        
        
    }

    public void Purchase(string productId, Action<bool, string> onComplete)
    {
        this.onComplete = onComplete;
        
        if (!NetworkUtils.CheckInternetConnection(true))
        {
            this.onComplete?.Invoke(false, productId);
            this.onComplete = null;
            return;
        }
        
        UIWaitWindowView.Show();
        IapService.Current.Purchase(productId);
    }
}