public class UISimpleTabContainerElementViewController : UIContainerElementViewController
{
    [IWUIBinding] protected UISimpleTabButtonViewController button;
    
    [IWUIBindingNullable("#Label")] protected NSText label;
    [IWUIBindingNullable("#CheckmarkLabel")] protected NSText checkmarkLabel;
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UISimpleTabContainerElementEntity;
        
        if (label != null) label.Text = contentEntity.LabelText;
        if (checkmarkLabel != null) checkmarkLabel.Text = contentEntity.CheckmarkText ?? contentEntity.LabelText;
        
        button
            .Init()
            .ToState(GenericButtonState.UnActive)
            .OnClick(Select);
    }
    
    public override void OnSelect()
    {
        base.OnSelect();
        
        CachedTransform.SetAsLastSibling();

        button.ToState(GenericButtonState.Active);
    }
    
    public override void OnDeselect()
    {
        base.OnDeselect();
        
        button.ToState(GenericButtonState.UnActive);
        
        CachedTransform.SetSiblingIndex(Index);
    }
}