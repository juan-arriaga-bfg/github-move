using UnityEngine;
using UnityEngine.UI;

public class UISimpleScrollItem : IWUIWindowViewController
{
    [SerializeField] protected Image icon;
    [SerializeField] protected NSText label;
    
    public void Init(string id, string text)
    {
        icon.sprite = IconService.Current.GetSpriteById(id);
        label.Text = text;
    }
}