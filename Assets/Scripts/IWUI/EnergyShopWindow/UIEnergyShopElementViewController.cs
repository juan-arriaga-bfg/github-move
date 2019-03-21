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
	    extraLabel.Text = contentEntity.ExtraLabel;
	    
	    freeObj.SetActive(isFree);
	    extraObj.SetActive(string.IsNullOrEmpty(contentEntity.ExtraLabel) == false);

	    if (isFree == false) return;
	    
	    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnStart += UpdateLabel;
	    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnTimeChanged += UpdateLabel;
	    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnComplete += CompleteTimer;
	    UpdateLabel();
    }
    
    public override void OnViewClose(IWUIWindowView context)
    {
	    if (isFree)
	    {
		    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnStart -= UpdateLabel;
		    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnTimeChanged -= UpdateLabel;
		    BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.OnComplete -= CompleteTimer;
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
	    timerLabel.Text = BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.CompleteTime.GetTimeLeftText(true, true, null);
    }

    private void CompleteTimer()
    {
	    isClaimed = false;
	    ChangeView();
    }

    protected override void OnPurchaseComplete()
    {
	    base.OnPurchaseComplete();
	    
	    if (isFree) BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer.Start();
    }
}