public class ClosePermanentWindowsInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        UIService.Get.CloseWindow(UIWindowType.MainWindow);
        UIService.Get.CloseWindow(UIWindowType.ResourcePanelWindow);

        isCompleted = true;
        OnComplete(this);
    }
}