using UnityEngine;

public class UIEnergyShopElementViewController : UIHardShopElementViewController
{
    [IWUIBinding("#FreeLabel")] private NSText freeLabel;
    [IWUIBinding("#TimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ExtraLabel")] private NSText extraLabel;
	
    [IWUIBinding("#Free")] private GameObject freeObj;
    [IWUIBinding("#Extra")] private GameObject extraObj;
    [IWUIBinding("#Free")] private CanvasGroup freeCanvas;
    
    private bool isFree;
    
    public override void Init()
    {
	    var contentEntity = entity as UIShopElementEntity;

	    isFree = contentEntity.Price != null && contentEntity.Price.Amount == 0;
	    isClaimed = isFree && BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.IsExecuteable();
	    
	    base.Init();
	    
	    freeLabel.Text = LocalizationService.Get("common.button.free", "common.button.free");
	    extraLabel.Text = contentEntity.ExtraText;
	    
	    freeObj.SetActive(isFree);
	    extraObj.SetActive(string.IsNullOrEmpty(contentEntity.ExtraText) == false);

        if (isFree)
        {
            BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnStart += StartResetEnergyTimer;
            BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnTimeChanged += UpdateLabel;
            BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnComplete += CompleteResetEnergyTimer;
            
            BoardService.Current.FirstBoard.MarketLogic.ClaimEnergyTimer.OnStart += StartClaimEnergyTimer;
            BoardService.Current.FirstBoard.MarketLogic.ClaimEnergyTimer.OnTimeChanged += UpdateLabel;
            BoardService.Current.FirstBoard.MarketLogic.ClaimEnergyTimer.OnComplete += CompleteClaimEnergyTimer;
            UpdateLabel();
        }
    }

    private void SetModeClaimed()
    {
        IW.Logger.Log($"[UIEnergyShopElementViewController] => SetModeClaimed");
        
        isClaimed = true;
        ChangeView();
        UpdateLabel();
    }

    private void SetModeAvailable()
    {
        IW.Logger.Log($"[UIEnergyShopElementViewController] => SetModeAvailable");
        
        isClaimed = false;
        ChangeView();
        UpdateLabel();
    }
    
    private void StartClaimEnergyTimer()
    {
        IW.Logger.Log($"[UIEnergyShopElementViewController] => StartClaimEnergyTimer");
        
        SetModeAvailable();
    }
    
    private void CompleteClaimEnergyTimer()
    {
        IW.Logger.Log($"[UIEnergyShopElementViewController] => CompleteClaimEnergyTimer");
        
        SetModeClaimed();
    }
    
    private void StartResetEnergyTimer()
    {
        IW.Logger.Log($"[UIEnergyShopElementViewController] => StartResetEnergyTimer");
        
        UpdateLabel();
    }

    private void CompleteResetEnergyTimer()
    {
        IW.Logger.Log($"[UIEnergyShopElementViewController] => CompleteResetEnergyTimer");
        
        UpdateLabel();
    }


    public override void OnViewClose(IWUIWindowView context)
    {
	    if (isFree)
	    {
		    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnStart -= StartResetEnergyTimer;
		    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnTimeChanged -= UpdateLabel;
		    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnComplete -= CompleteResetEnergyTimer;
		    
            BoardService.Current.FirstBoard.MarketLogic.ClaimEnergyTimer.OnStart -= StartClaimEnergyTimer;
            BoardService.Current.FirstBoard.MarketLogic.ClaimEnergyTimer.OnTimeChanged -= UpdateLabel;
            BoardService.Current.FirstBoard.MarketLogic.ClaimEnergyTimer.OnComplete -= CompleteClaimEnergyTimer;
	    }
	    
	    base.OnViewClose(context);
    }

    protected override void ChangeView()
    {
	    base.ChangeView();
	    
	    timerLabel.gameObject.SetActive(isFree && isClaimed);
	    freeCanvas.alpha = isClaimed ? 0.5f : 1;
    }

    private void UpdateLabel()
    {
        var marketLogic = BoardService.Current.FirstBoard.MarketLogic;
        
        if (marketLogic.ResetEnergyTimer.IsStarted)
        {
            timerLabel.Text = marketLogic.ResetEnergyTimer.CompleteTime.GetTimeLeftText(true, true, null, true, false);
        }
        
        if (marketLogic.ClaimEnergyTimer.IsStarted)
        {
            if (!marketLogic.FirstFreeEnergyClaimed)
            {
                nameLabel.Text = LocalizationService.Get("window.shop.energy.item1", "window.shop.energy.item1");
            }
            else
            {
                string mask = LocalizationService.Get("window.shop.energy.disappear", "window.shop.energy.disappear");
                string timeLeft = marketLogic.ClaimEnergyTimer.CompleteTime.GetTimeLeftText(true, true, null, true, false);
                nameLabel.Text = string.Format(mask, timeLeft);
            }
        }
    }

    protected override void OnPurchaseComplete()
    {
	    base.OnPurchaseComplete();

        if (isFree)
        {
            BoardService.Current.FirstBoard.MarketLogic.FreeEnergyClaim();
        }
    }
}