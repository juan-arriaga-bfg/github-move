public class ActivateDailyRewardStep : BaseTutorialStep
{
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();

        GameDataService.Current.DailyRewardManager.Activate();
        
        UIService.Get.OnCloseWindowEvent += OnCloseWindowEvent;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        
        UIService.Get.OnCloseWindowEvent -= OnCloseWindowEvent;
    }

    private void OnCloseWindowEvent(IWUIWindow window)
    {
        if (window?)
        
        isAutoComplete = true;
        Context.UpdateHard();
    }
}