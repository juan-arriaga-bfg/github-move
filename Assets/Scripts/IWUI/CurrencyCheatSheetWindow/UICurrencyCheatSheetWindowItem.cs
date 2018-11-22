using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICurrencyCheatSheetWindowItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lblName;
    [SerializeField] private TextMeshProUGUI lblId;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Image ico;

    private CurrencyDef def;
    public CurrencyDef Def => def;
    
    private int oldValue;
    
    public void Init(CurrencyDef def)
    {
        this.def = def;
        
        lblName.text = def.Name;
        lblId.text = $"id {def.Id}";
        
        var spr = GetPieecSprite();
        if (spr != null)
        {
            ico.sprite = spr;
        }
        
        var item = ProfileService.Current.Purchases.GetStorageItem(def.Name);

        oldValue = item.Amount;
        inputField.text = oldValue.ToString();
    }

    public void SetValue(int value)
    {
        var str = value.ToString();
        
        inputField.text = str;
        OnEndEdit(str);
    }
    
    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(def.Icon);
    }

    public void OnEndEdit(string value)
    {
        var newValue = int.Parse(inputField.text);

        if (def.Name == Currency.Level.Name && oldValue > newValue)
        {
            inputField.text = oldValue.ToString();
            return;
        }
        
        inputField.textComponent.color = oldValue != newValue ? Color.yellow : Color.white;
    }

    public void Apply()
    {
        var newValue = int.Parse(inputField.text);
        var deffValue = newValue - oldValue;
        
        if(deffValue == 0) return;
        
        if (deffValue > 0)
        {
            CurrencyHellper.Purchase(def.Name, deffValue);
            
            if(def.Name == Currency.Level.Name) CurrencyHellper.Purchase(Currency.Experience.Name, 1, Currency.Experience.Name, 1);
        }
        else CurrencyHellper.Purchase(def.Name, 0, def.Name, -deffValue);
        
        Debug.Log($"[UICurrencyCheatSheetWindowItem] => OnEndEdit: {def.Name}: {oldValue} -> {newValue}");
    }
}
