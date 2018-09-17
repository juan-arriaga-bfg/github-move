using System.Collections;
using System.Collections.Generic;
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

    private bool changed;

    public bool IsChanged()
    {
        return changed; 
    }

    public void Init(CurrencyDef def)
    {
        this.def = def;
        
        lblName.text = def.Name;
        lblId.text = $"id: {def.Id}";
        
        var spr = IconService.Current.GetSpriteById(def.Name);
        if (spr != null)
        {
            ico.sprite = spr;
        }
        
        var item = ProfileService.Current.Purchases.GetStorageItem(def.Name);
        inputField.text = item.Amount.ToString();
    }

    public void OnEndEdit(string value)
    {
        StorageItem storageItem = ProfileService.Current.Purchases.GetStorageItem(def.Name);

        int oldValue = storageItem.Amount; 
        int newValue = int.Parse(value);
        
        storageItem.Amount = newValue;

        if (oldValue != newValue)
        {
            changed = true;
        }
        
        Debug.Log($"[UICurrencyCheatSheetWindowItem] => OnEndEdit: {def.Name}: {oldValue} -> {newValue}");
    }
}
