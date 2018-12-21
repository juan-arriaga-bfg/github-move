using TMPro;
using UnityEngine;

public class UICurrencyCheatSheetElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#Id")] protected NSText id;
    [IWUIBinding("#Input")] protected TMP_InputField inputField;
    
    private int oldValue;

    public override void Init()
    {
        base.Init();

        var contentEntity = entity as UISimpleScrollElementEntity;
        var item = ProfileService.Current.Purchases.GetStorageItem(contentEntity.LabelText);
        var def = Currency.GetCurrencyDef(contentEntity.LabelText);
        
        id.Text = $"id {def.Id}";
        oldValue = item.Amount;
        SetValue(oldValue);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        if(entity == null) return;
        
        var contentEntity = entity as UISimpleScrollElementEntity;
        var newValue = int.Parse(inputField.text);
        var deffValue = newValue - oldValue;
        
        if(deffValue == 0) return;
        
        if (deffValue > 0)
        {
            CurrencyHellper.Purchase(contentEntity.LabelText, deffValue);
            
            if(contentEntity.LabelText == Currency.Level.Name) CurrencyHellper.Purchase(Currency.Experience.Name, 1, Currency.Experience.Name, 1);
        }
        else CurrencyHellper.Purchase(contentEntity.LabelText, 0, contentEntity.LabelText, -deffValue);
        
        Debug.Log($"[UICurrencyCheatSheetWindowItem] => OnEndEdit: {contentEntity.LabelText}: {oldValue} -> {newValue}");
    }

    public void SetValue(int value)
    {
        var str = value.ToString();
        
        inputField.text = str;
        OnEndEdit(str);
    }
    
    public void OnEndEdit(string value)
    {
        var contentEntity = entity as UISimpleScrollElementEntity;
        var newValue = int.Parse(value);

        if (contentEntity.LabelText == Currency.Level.Name && oldValue > newValue)
        {
            inputField.text = oldValue.ToString();
            return;
        }
        
        inputField.textComponent.color = oldValue != newValue ? Color.yellow : Color.white;
    }
}
