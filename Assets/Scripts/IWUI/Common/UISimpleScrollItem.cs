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
    
    public void SetActive(bool value)
    {
        icon.gameObject.SetActive(value);
        label.gameObject.SetActive(value);
    }
}