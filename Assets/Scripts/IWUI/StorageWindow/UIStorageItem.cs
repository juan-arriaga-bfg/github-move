using UnityEngine;
using UnityEngine.UI;

public class UIStorageItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText label;
    
    public void Init(StorageItem item)
    {
        icon.sprite = IconService.Current.GetSpriteById(item.Currency);
        label.Text = item.Amount.ToString();
    }
}