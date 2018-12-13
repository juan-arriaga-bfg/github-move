using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabContainerElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#TabLabel")] private NSText tabLabel;

    [IWUIBinding] private UIButtonViewController tabButton;
    
    public override void Init()
    {
        base.Init();
        
        var targetEntity = entity as UITabContainerElementEntity;

        tabLabel.Text = targetEntity.TabLabel;

        tabButton
            .Init()
            .OnClick(Select);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        
        CachedTransform.SetAsLastSibling();

        tabButton.ToState(GenericButtonState.Active);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        
        tabButton.ToState(GenericButtonState.UnActive);
        
        CachedTransform.SetSiblingIndex(Index);
    }
}
