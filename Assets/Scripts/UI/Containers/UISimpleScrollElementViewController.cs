using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISimpleScrollElementViewController : UIContainerElementViewController
{
    [IWUIBindingNullable("#Icon")] protected Image icon;
    [IWUIBindingNullable("#Anchor")] protected Transform anchor;
    [IWUIBindingNullable("#Label")] protected NSText label;
    
    private Transform content;
    private CanvasGroup group;

    private Material lockMaterial;
    private Material unlockMaterial;
    
    private List<Image> IconSprites;

    private CanvasGroup Group
    {
        get
        {
            if (anchor != null)
            {
                group = anchor.GetComponent<CanvasGroup>();
                if (group == null)
                {
                    group = anchor.gameObject.AddComponent<CanvasGroup>();
                }
            }
            else
            {
                group = GetComponentInChildren<CanvasGroup>();
            }

            return group;
        }
    }

    public bool Sepia
    {
        set
        {
            if (IconSprites == null) return;
            
            if (lockMaterial == null) lockMaterial = (Material) ContentService.Current.GetObjectByName("UiSepia");
            if (unlockMaterial == null) unlockMaterial = IconSprites.Count > 0 ? IconSprites[0].material : lockMaterial;

            foreach (var sprite in IconSprites)
            {
                sprite.material = value ? lockMaterial : unlockMaterial;
            }
        }
    }

    public Transform Anchor
    {
        get { return anchor; }
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
            IconSprites = new List<Image>{icon};
            return;
        }

        if (anchor != null) CreateIcon(anchor, contentEntity.ContentId);
    }
    
    protected void CreateIcon(Transform parent, string id)
    {
        if (content != null)
        {
            Sepia = false;
            UIService.Get.PoolContainer.Return(content.gameObject);
        }

        if (string.IsNullOrEmpty(id)) return;
        
        content = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        content.SetParentAndReset(parent);
        
        IconSprites = content.GetComponentsInChildren<Image>().ToList();
    }

    public override void OnViewCloseCompleted()
    {
        Sepia = false;
        
        if (content != null) UIService.Get.PoolContainer.Return(content.gameObject);

        content = null;
        IconSprites = null;
        
        base.OnViewCloseCompleted();
    }
}