using UnityEngine;

public class UIArrowTutorialStep : BaseTutorialStep
{
    protected Transform anchor;
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
        
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);
        
        view.CachedHintArrowComponent.HideArrow(anchor);
        arrow = null;
    }
    
    protected virtual void CreateArrow()
    {
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);
        
        arrow = view.CachedHintArrowComponent.ShowArrow(anchor, -1);
    }
}
