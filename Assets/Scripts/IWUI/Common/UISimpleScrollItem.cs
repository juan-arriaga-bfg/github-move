using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UISimpleScrollItem : IWUIWindowViewController
{
    [SerializeField] protected Image icon;
    [SerializeField] protected NSText label;
    
    [SerializeField] private Transform body;
    
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

    public virtual void Select(bool isActive)
    {
        var scale = isActive ? 1.1f : 1f;
        var time = 0.1f * body.localScale.x / scale;

        DOTween.Kill(body);
        body.DOScale(scale, time).SetId(body);
    }
}