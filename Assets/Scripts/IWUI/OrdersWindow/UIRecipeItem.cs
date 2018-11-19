using UnityEngine;
using UnityEngine.UI;

public class UIRecipeItem : UISimpleScrollItem
{
    [SerializeField] private Material lockMaterial;
    [SerializeField] private UIOrderPriceItem prices;
    
    private Material unlockMaterial;
    private Toggle toggle;
    private OrderDef recipe;
    
    public void Init(OrderDef recipe)
    {
        this.recipe = recipe;
        
        if (toggle == null) toggle = gameObject.GetComponent<Toggle>();
        if(unlockMaterial == null) unlockMaterial = icon.material;

        var isLock = recipe.Level > GameDataService.Current.LevelsManager.Level;
        
        toggle.isOn = false;
        toggle.interactable = !isLock;
        
        icon.material = isLock ? lockMaterial : unlockMaterial;
        
        Init(recipe.Uid, string.Format(LocalizationService.Instance.Manager.GetTextByUid("common.message.level", "Level {0}"), recipe.Level));
        label.gameObject.SetActive(isLock);
    }
    
    public override void Select(bool isActive)
    {
        base.Select(isActive);
        
        prices.Init(isActive ? recipe : null, transform);
    }
}