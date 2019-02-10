public class MarketTutorialStep : BaseTutorialStep
{
    private UIHintArrowViewController arrow;
    
    public override void PauseOn()
    {
        base.PauseOn();
        
        if(arrow == null) return;
        
        arrow.Hide(false);
    }

    public override void PauseOff()
    {
        base.PauseOff();
        
        if(arrow == null) return;
        
        CreateArrow();
    }
    
    public override void Perform()
    {
        if(IsPerform) return;
        
        base.Perform();
        
        CreateArrow();
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        var model = UIService.Get.GetCachedModel<UIMarketWindowModel>(UIWindowType.MarketWindow);
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);

        model.IsTutorial = false;

        view.CachedHintArrowComponent.HideArrow(view.HintAnchorShopButton);

        arrow = null;
    }

    private void CreateArrow()
    {
        var model = UIService.Get.GetCachedModel<UIMarketWindowModel>(UIWindowType.MarketWindow);
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);

        model.IsTutorial = true;

        arrow = view.CachedHintArrowComponent.ShowArrow(view.HintAnchorShopButton, -1);
    }
}
