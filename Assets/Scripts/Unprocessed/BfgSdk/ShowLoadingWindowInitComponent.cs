public class ShowLoadingWindowInitComponent : AsyncInitItemBase
{
    public override void Execute()
    {
        // set resource deliverer for UI
        IWUISettings.Instance.SetResourceManager(new DefaultUIResourceManager());

        UIService.Get.ShowWindowInstantly(UIWindowType.LauncherWindow);

        isCompleted = true;
        OnComplete(this);
    }
}