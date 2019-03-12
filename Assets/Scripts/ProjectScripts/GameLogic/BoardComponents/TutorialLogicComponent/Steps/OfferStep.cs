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
        
        base.Perform();
        
        if (window == null) return;
        
        timer.Delay = def.Delay;
        timer.OnComplete += OnTimerComplete;
        
        var save = ProfileService.Current.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);
        
        if (save != null && string.IsNullOrEmpty(save.OfferTimerStartTime) == false) timer.Start(long.Parse(save.OfferTimerStartTime));
        else timer.Start();
        
        window.ChangeVisibility(UiLockTutorialItem.Offer, false, true);
        
        DefaultSafeQueueBuilder.BuildAndRun($"offerStep_{Target}", true, () =>
        {
            var model = UIService.Get.GetCachedModel<UIOfferWindowModel>(UIWindowType.OfferWindow);

            model.Product = def;
            
            UIService.Get.ShowWindow(UIWindowType.OfferWindow);
        });
    }

    protected override void Complete()
    {
        if (window != null) window.ChangeVisibility(UiLockTutorialItem.Offer, true, true);
        
        timer.OnComplete -= OnTimerComplete;
        timer.Stop();
        
        base.Complete();
    }

    private void OnTimerComplete()
    {
        Context.UpdateHard();
    }
}