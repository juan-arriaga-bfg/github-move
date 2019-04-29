using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIEventElementViewController : UIContainerElementViewController
{
    [IWUIBindingNullable("#Check")] private GameObject check;
    
    [IWUIBindingNullable("#Normal")] private GameObject normal;
    [IWUIBindingNullable("#Premium")] private GameObject premium;
    
    [IWUIBindingNullable("#NormalShine")] private GameObject normalShine;
    [IWUIBindingNullable("#PremiumShine")] private GameObject premiumShine;
    
    [IWUIBindingNullable("#NormalAnchor")] private Transform normalAnchor;
    [IWUIBindingNullable("#PremiumAnchor")] private Transform premiumAnchor;
    
    [IWUIBindingNullable("#NormalLabel")] private NSText normalLabel;
    [IWUIBindingNullable("#PremiumLabel")] private NSText premiumLabel;

    private Transform normalIcon;
    private Transform premiumIcon;
    
    private List<Image> normalSprites;
    private List<Image> premiumSprites;
    
    private Material lockMaterial;
    private Material unlockMaterial;
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIEventElementEntity;

        var isNormalActive = contentEntity.Step.NormalRewards != null && contentEntity.Step.NormalRewards.Count > 0;
        var isPremiumActive = contentEntity.Step.PaidRewards != null && contentEntity.Step.PaidRewards.Count > 0;
        
        normal.SetActive(isNormalActive);
        premium.SetActive(isPremiumActive);

        if (isNormalActive)
        {
            normalLabel.Text = $"x{contentEntity.Step.NormalRewards[0].Amount}";
            CreateIcon(ref normalIcon, normalAnchor, ref normalSprites, contentEntity.Step.NormalRewards[0].Currency);
        }
        
        if (isPremiumActive)
        {
            premiumLabel.Text = $"x{contentEntity.Step.PaidRewards[0].Amount}";
            CreateIcon(ref premiumIcon, premiumAnchor, ref premiumSprites, contentEntity.Step.PaidRewards[0].Currency);
        }
        
        SetProgress(0);
    }

    public void SetProgress(int value)
    {
        var contentEntity = entity as UIEventElementEntity;
        
        check.SetActive(false);
        normalShine.SetActive(false);
        premiumShine.SetActive(false);
    }
    
    public override void OnViewCloseCompleted()
    {
        Return(ref normalIcon, ref normalSprites);
        Return(ref premiumIcon, ref premiumSprites);
        
        base.OnViewCloseCompleted();
    }
    
    protected void CreateIcon(ref Transform current, Transform parent, ref List<Image> sprites, string id)
    {
        if (current != null) Return(ref current, ref sprites);
        if (string.IsNullOrEmpty(id)) return;
        
        current = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        current.SetParentAndReset(parent);
        
        sprites = current.GetComponentsInChildren<Image>().ToList();
    }
    
    private void Sepia(List<Image> sprites, bool value)
    {
        if (sprites == null) return;
            
        if (lockMaterial == null) lockMaterial = (Material) ContentService.Current.GetObjectByName("UiSepia");
        if (unlockMaterial == null) unlockMaterial = sprites.Count > 0 ? sprites[0].material : lockMaterial;

        foreach (var sprite in sprites)
        {
            sprite.material = value ? lockMaterial : unlockMaterial;
        }
    }

    private void Return(ref Transform current, ref List<Image> sprites)
    {
        Sepia(sprites, false);
        
        if(current != null) UIService.Get.PoolContainer.Return(current.gameObject);
        
        current = null;
        sprites = null;
    }
}