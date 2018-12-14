using UnityEngine;
using UnityEngine.UI;

public class UISimpleScrollElementViewController : UIContainerElementViewController
{
    [IWUIBindingNullable("#Icon")] protected Image icon;
    [IWUIBindingNullable("#Anchor")] protected Transform anchor;
    [IWUIBindingNullable("#Label")] protected NSText label;
    
    private Transform content;
    private CanvasGroup group;

    public CanvasGroup Group
    {
        get
        {
            if (group == null) group = GetComponentInChildren<CanvasGroup>(true);

            return group;
        }
    }

    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UISimpleScrollElementEntity;
        
        if (label != null) label.Text = contentEntity.LabelText;
        if (Group != null) Group.alpha = contentEntity.Alpha;
        
        if (string.IsNullOrEmpty(contentEntity.ContentId)) return;
        
        if (icon != null)
        {
            icon.sprite = IconService.Current.GetSpriteById(contentEntity.ContentId);
            return;
        }

        if (anchor != null) CreateIcon(anchor, contentEntity.ContentId);
    }
    
    protected void CreateIcon(Transform parent, string id)
    {
        if (content != null)
        {
            UIService.Get.PoolContainer.Return(content.gameObject);
        }
        
        content = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        content.SetParentAndReset(parent);
    }
}