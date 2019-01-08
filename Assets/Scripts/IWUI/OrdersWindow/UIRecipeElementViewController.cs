using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding] private UIOrdersTabButtonViewController button;
    
    private Material lockMaterial;
    private Material unlockMaterial;

    private List<Image> recipeSprites;
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIRecipeElementEntity;

        recipeSprites = anchor.GetComponentsInChildren<Image>().ToList();

        if (lockMaterial == null) lockMaterial = (Material) ContentService.Current.GetObjectByName("UiSepia");
        if (unlockMaterial == null) unlockMaterial = recipeSprites.Count > 0 ? recipeSprites[0].material : lockMaterial;

        foreach (var sprite in recipeSprites)
        {
            sprite.material = contentEntity.IsLock ? lockMaterial : unlockMaterial;
        }
        
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