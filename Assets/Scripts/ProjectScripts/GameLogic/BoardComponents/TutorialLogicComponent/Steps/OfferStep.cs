public class OfferStep : BaseTutorialStep
{
    public int Target;
    
    private UIMainWindowView window;
    private TimerComponent timer;
    private ShopDef def;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);

        window = UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow)?.CurrentView as UIMainWindowView;
        timer = BoardService.Current.FirstBoard.MarketLogic.OfferTimer;
        def = GameDataService.Current.ShopManager.Defs[Currency.Offer.Name][Target - 1];
    }

    public override void Perform()
    {
        if (IsPerform) return;

        BoardService.Current.FirstBoard.MarketLogic.Offer = def;
        BoardService.Current.FirstBoard.MarketLogic.OfferIndex = Target;
        
        base.Perform();
        
        if (window == null) return;
        
        timer.Delay = def.Delay;
        timer.OnComplete += OnTimerComplete;
        
        var save = ProfileService.Current.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);
        
        if (save != null && string.IsNullOrEmpty(save.OfferTimerStartTime) == false) timer.Start(long.Parse(save.OfferTimerStartTime));
        else timer.Start();
        
        if (timer.GetProgress() >= 1) return;
        
        window.ChangeVisibility(UiLockTutorialItem.Offer, false, true);
        
        DefaultSafeQueueBuilder.BuildAndRun($"offerStep_{Target}", true, () =>
        {
            UIService.Get.ShowWindow(UIWindowType.OfferWindow);
        });
    }

    protected override void Complete()
    {
        if (window != null) window.ChangeVisibility(UiLockTutorialItem.Offer, true, true);
        
        timer.OnComplete -= OnTimerComplete;
        timer.Stop();
        
        BoardService.Current.FirstBoard.MarketLogic.Offer = null;
        BoardService.Current.FirstBoard.MarketLogic.OfferIndex = -1;
        
        base.Complete();
    }

    private void OnTimerComplete()
    {
        Context.UpdateHard();
        BoardService.Current.FirstBoard.MarketLogic.CompleteOffer();
    }
}