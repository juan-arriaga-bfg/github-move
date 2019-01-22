public class MarketTutorialStep : BaseTutorialStep
{
    public override void Perform()
    {
        if(IsPerform) return;
        
        base.Perform();
        
        var model = UIService.Get.GetCachedModel<UIMarketWindowModel>(UIWindowType.MarketWindow);
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);

        model.IsTutorial = true;

        view.CachedHintArrowComponent.ShowArrow(view.HintAnchorShopButton, -1);
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        var model = UIService.Get.GetCachedModel<UIMarketWindowModel>(UIWindowType.MarketWindow);
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);

        model.IsTutorial = false;

        view.CachedHintArrowComponent.HideArrow(view.HintAnchorShopButton);
    }
}
