public class HighlightTaskPointToEnergyPlusButton : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var view = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);

        view.CachedHintArrowComponent.ShowArrow(view.HintAnchorEnergyPlusButton);

        return true;
    }
}