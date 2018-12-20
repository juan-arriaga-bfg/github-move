using UnityEngine;

public class UIRecipeElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding] private UIOrdersTabButtonViewController button;
    
    [SerializeField] private Material lockMaterial;
    
    private Material unlockMaterial;
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIRecipeElementEntity;
        
        if (unlockMaterial == null) unlockMaterial = icon.material;
        
        icon.material = contentEntity.IsLock ? lockMaterial : unlockMaterial;
        label.gameObject.SetActive(contentEntity.IsLock);
        
        button
            .SetActiveScale(1.1f)
            .SetDragDirection(new Vector2(0f, 1f))
            .SetDragThreshold(100f)
            .ToState(GenericButtonState.UnActive).OnClick(Select);
        
        button.Interactable = !contentEntity.IsLock;
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