using UnityEngine;

public class UIArrowTutorialStep : BaseTutorialStep
{
    protected Transform anchor;
    
    public override void PauseOn()
    {
        base.PauseOn();
        
        if (IsPerform == false) return;
        
        HideArrow();
    }

    public override void PauseOff()
    {
        base.PauseOff();
        
        if (IsPerform == false) return;
        
        CreateArrow();
    }
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        CreateArrow();
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        HideArrow();
    }

    private void HideArrow()
    {
        if (anchor == null) return;
        
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);
        
        view.CachedHintArrowComponent.HideArrow(anchor);
    }
    
    protected virtual void CreateArrow()
    {
        if (anchor == null) return;
        
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);
        
        view.CachedHintArrowComponent.ShowArrow(anchor, -1);
    }
}
