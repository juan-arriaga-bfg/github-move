using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class UICurrencyCheatSheetWindowView : UIGenericPopupWindowView
{
    [SerializeField] private UICurrencyCheatSheetWindowItem itemPrefab;
    
    [IWUIBinding("#ButtonAdd")] private UIButtonViewController btnAdd;
    [IWUIBinding("#ButtonRemove")] private UIButtonViewController btnRemove;
    
    [IWUIBinding("#ButtonAddLabel")] protected NSText buttonAddLabel;
    [IWUIBinding("#ButtonRemoveLabel")] protected NSText buttonRemoveLabel;

    private List<UICurrencyCheatSheetWindowItem> items;
    
    private List<CurrencyDef> mainCurrency = new List<CurrencyDef>
    {
        Currency.Coins,
        Currency.Crystals,
        Currency.Mana,
        Currency.Energy,
    };
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICurrencyCheatSheetWindowModel model = Model as UICurrencyCheatSheetWindowModel;

        SetTitle(model.Title);
        CreateItems(model);
        
        InitBtnBase(btnAdd, () => OnSet(999999));
        InitBtnBase(btnRemove, () => OnSet(0));

        buttonAddLabel.Text = $"{GetBtnLabel()}99999";
        buttonRemoveLabel.Text = $"{GetBtnLabel()}0";
    }

    private string GetBtnLabel()
    {
        var str = new StringBuilder();

        for (var i = 0; i < mainCurrency.Count; i++)
        {
            var def = mainCurrency[i];
            str.Append($"<sprite name=\"{def.Name}\">{(i == mainCurrency.Count - 1 ? " = " : ", ")}");
        }

        return str.ToString();
    }

    private void CreateItems(UICurrencyCheatSheetWindowModel model)
    {
        itemPrefab.gameObject.SetActive(true);
        
        items = new List<UICurrencyCheatSheetWindowItem>();
        var currencies = model.CurrenciesList();

        foreach (var currencyDef in currencies)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(itemPrefab.transform.parent, false);
            item.Init(currencyDef); 
            
            items.Add(item);
        }

        itemPrefab.gameObject.SetActive(false);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        UICurrencyCheatSheetWindowModel windowModel = Model as UICurrencyCheatSheetWindowModel;

        foreach (var item in items)
        {
            item.Apply();
            Destroy(item.gameObject);
        }
        
        ProfileService.Instance.Manager.SaveLocalProfile();
    }

    private void OnSet(int value)
    {
        foreach (var item in items)
        {
            if (mainCurrency.Contains(item.Def) == false) continue;
            
            item.SetValue(value);
        }
    }
}
