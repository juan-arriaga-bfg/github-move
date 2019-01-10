public class UIRecipeElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding] private UIOrdersTabButtonViewController button;
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIRecipeElementEntity;

        if(contentEntity.IsLock) Sepia = true;
        
        label.gameObject.SetActive(contentEntity.IsLock);
        button.Interactable = !contentEntity.IsLock;
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        button
            .SetActiveScale(1.1f)
            .ToState(GenericButtonState.UnActive)
            .OnClick(Select);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        
        button.ToState(GenericButtonState.Active);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        
        button.ToState(GenericButtonState.UnActive);
    }
}