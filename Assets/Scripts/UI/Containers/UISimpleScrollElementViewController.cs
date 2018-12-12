using UnityEngine;
using UnityEngine.UI;

public class UISimpleScrollElementViewController : UIContainerElementViewController
{
    [IWUIBindingNullable("#Icon")] protected Image icon;
    [IWUIBindingNullable("#Anchor")] protected Transform anchor;
    [IWUIBindingNullable("#Label")] protected NSText label;
    
    private Transform content;

    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UISimpleScrollElementEntity;
        
        if (label != null) label.Text = contentEntity.LabelText;
        
        if (icon != null)
        {
            icon.sprite = IconService.Current.GetSpriteById(contentEntity.ContentId);
            return;
        }
        
        if (content != null)
        {
            UIService.Get.PoolContainer.Return(content.gameObject);
        }
            
        content = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(contentEntity.ContentId));
        content.SetParentAndReset(anchor);
    }
}