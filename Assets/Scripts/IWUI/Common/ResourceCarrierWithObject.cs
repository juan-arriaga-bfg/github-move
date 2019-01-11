using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCarrierWithObject : ResourceCarrierBase
{
    [SerializeField] private Transform iconAnchor;
    [SerializeField] private Transform shadowAnchor;
    
    private Material normalMaterial;
    private Material shadowMaterial;

    private Transform icon;
    private Transform shadow;

    private List<Image> shadowSprites;
    
    public IResourceCarrier RefreshIcon(string id)
    {
        icon = CreateIcon(iconAnchor, id);
        shadow = CreateIcon(shadowAnchor, id);
        
        if (normalMaterial == null) normalMaterial = shadow.GetComponentsInChildren<Image>()[0].material;
        if (shadowMaterial == null) shadowMaterial = (Material) ContentService.Current.GetObjectByName("UIOneColor");
        
        shadowSprites = shadow.GetComponentsInChildren<Image>().ToList();

        SetMaterial(shadowMaterial, shadowSprites);
        
        return this;
    }
    
    private Transform CreateIcon(Transform parent, string id)
    {
        var content = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        
        content.SetParentAndReset(parent);
        
        return content;
    }

    private void SetMaterial(Material material, List<Image> sprites)
    {
        foreach (var sprite in sprites)
        {
            sprite.material = material;
        }
    }

    protected override void OnComplete()
    {
        SetMaterial(normalMaterial, shadowSprites);
        
        UIService.Get.PoolContainer.Return(icon.gameObject);
        UIService.Get.PoolContainer.Return(shadow.gameObject);
        
        base.OnComplete();
    }
}
