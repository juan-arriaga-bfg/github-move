public class MarketTutorialStep : UIArrowTutorialStep
 {   
     public override void Perform()
     {
         if(IsPerform) return;
         
         var model = UIService.Get.GetCachedModel<UIMarketWindowModel>(UIWindowType.MarketWindow);
         var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);
         
         model.IsTutorial = true;
         anchor = view.HintAnchorShopButton;
         
         base.Perform();
     }
     
     protected override void Complete()
     {
         var model = UIService.Get.GetCachedModel<UIMarketWindowModel>(UIWindowType.MarketWindow);
         
         model.IsTutorial = false;
         
         base.Complete();
     }
 }