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

    private string initialValue;
    
    public bool IsChanged()
    {
        return initialValue != inputField.text; 
    }

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
        inputField.text = item.Amount.ToString();
        
        initialValue = inputField.text;
    }
    
    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(def.Icon);
    }

    public void OnEndEdit(string value)
    {
        StorageItem storageItem = ProfileService.Current.Purchases.GetStorageItem(def.Name);

        int oldValue = storageItem.Amount; 
        int newValue = int.Parse(value);
        
        storageItem.Amount = newValue;

        inputField.textComponent.color = IsChanged() ? Color.yellow : Color.white; 
        
        Debug.Log($"[UICurrencyCheatSheetWindowItem] => OnEndEdit: {def.Name}: {oldValue} -> {newValue}");
    }
}
