using UnityEngine;

public class UINextLevelWindowView : UIGenericWindowView
{
    [SerializeField] private NSText title;
    [SerializeField] private NSText message;
    [SerializeField] private NSText rewards;
    
    [SerializeField] private Transform tapToContinueAnchor;
    
    [SerializeField] private IWTextMeshAnimation textMeshAnimation;
    
    private TapToContinueTextViewController tapToContinue;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UINextLevelWindowModel windowModel = Model as UINextLevelWindowModel;
        
        title.Text = windowModel.Title;
        message.Text = windowModel.Mesage;
        rewards.Text = windowModel.Rewards;
        
        textMeshAnimation.Animate();
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();
        InitTapToContinue(1f);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UINextLevelWindowModel windowModel = Model as UINextLevelWindowModel;
    }

    public override void OnViewCloseCompleted()
    {
        var manager = GameDataService.Current.LevelsManager;
        
        CurrencyHellper.Purchase(Currency.Level.Name, 1, Currency.Experience.Name, manager.Price);
        CurrencyHellper.Purchase(Currency.EnergyLimit.Name, 1);
        CurrencyHellper.Purchase(manager.Rewards, null, new Vector2(Screen.width/2, Screen.height/2));
       
        var currentValue = ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
        var limitValue = ProfileService.Current.GetStorageItem(Currency.EnergyLimit.Name).Amount;
        var diff = limitValue - currentValue;
        
        if (diff > 0) CurrencyHellper.Purchase(Currency.Energy.Name, diff);
            
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny();
        GameDataService.Current.LevelsManager.UpdateSequence();
        GameDataService.Current.OrdersManager.Unlock();
        
        base.OnViewCloseCompleted();
    }
    
    private void InitTapToContinue(float delay)
    {
        if (tapToContinue == null)
        {
            var pool = UIService.Get.PoolContainer;
            tapToContinue = pool.Create<TapToContinueTextViewController>(R.TapToContinueTextView);
            tapToContinue.transform.SetParent(tapToContinueAnchor, false);
            tapToContinue.transform.localPosition = Vector3.zero;
        }

        tapToContinue.Hide(false);
        tapToContinue.Show(true, delay);
    }
}
