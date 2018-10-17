using UnityEngine;
using UnityEngine.UI;

public class UIRecipeItem : UISimpleScrollItem
{
    [SerializeField] private Material lockMaterial;
    
    private Material unlockMaterial;
    private Toggle toggle;

    public void Init(OrderDef recipe)
    {
        if (toggle == null) toggle = gameObject.GetComponent<Toggle>();
        if(unlockMaterial == null) unlockMaterial = icon.material;

        var isLock = recipe.Level > GameDataService.Current.LevelsManager.Level;
        
        toggle.isOn = false;
        toggle.interactable = !isLock;

        icon.material = isLock ? lockMaterial : unlockMaterial;
        
        Init(recipe.Uid, $"Level {recipe.Level}");
        label.gameObject.SetActive(isLock);
    }
}