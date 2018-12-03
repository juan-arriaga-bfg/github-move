public class HighlightTaskPointToShopButton : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        UIMainWindowView view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);

        view.CachedHintArrowComponent.ShowArrow(view.HintAnchorShopButton);

        return true;
    }
}