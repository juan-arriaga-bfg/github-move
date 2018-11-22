using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

public class UICurrencyCheatSheetWindowView : UIGenericPopupWindowView
{
    [SerializeField] private UICurrencyCheatSheetWindowItem itemPrefab;
    [SerializeField] private NSText labelAdd;
    [SerializeField] private NSText labelRemove;

    private List<UICurrencyCheatSheetWindowItem> items;
    
    private List<CurrencyDef> mainCurrency = new List<CurrencyDef>
    {
        Currency.Coins,
        Currency.Crystals,
        Currency.Mana,
    };
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICurrencyCheatSheetWindowModel model = Model as UICurrencyCheatSheetWindowModel;

        SetTitle(model.Title);
        CreateItems(model);

        labelAdd.Text = $"{GetBtnLabel()}99999";
        labelRemove.Text = $"{GetBtnLabel()}0";
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

    public void OnSet(int value)
    {
        foreach (var item in items)
        {
            if (mainCurrency.Contains(item.Def) == false) continue;
            
            item.SetValue(value);
        }
    }

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
    }
}
