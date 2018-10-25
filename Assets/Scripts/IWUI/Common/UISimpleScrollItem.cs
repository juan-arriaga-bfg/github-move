using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UISimpleScrollItem : IWUIWindowViewController
{
    [SerializeField] protected Image icon;
    [SerializeField] protected Transform anchor;
    [SerializeField] protected NSText label;
    
    [SerializeField] private Transform body;

    private Transform iconObj;
    
    public void Init(string id, string text)
    {
        if (icon != null)
        {
            icon.sprite = IconService.Current.GetSpriteById(id);
        }
        else
        {
            if (iconObj != null)
            {
                UIService.Get.PoolContainer.Return(iconObj.gameObject);
            }
            
            iconObj = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
            iconObj.SetParentAndReset(anchor);
        }
        
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