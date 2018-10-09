using UnityEngine.UI;

public class UIRecipeItem : UISimpleScrollItem
{
    private Toggle toggle;

    public void Init(OrderDef recipe)
    {
        if (toggle == null) toggle = gameObject.GetComponent<Toggle>();

        toggle.isOn = false;
        
        Init(recipe.Uid, "99");
    }
}