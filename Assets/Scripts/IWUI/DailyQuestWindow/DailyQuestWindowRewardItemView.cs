using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestWindowRewardItemView : IWUIWindowViewController
{
    [IWUIBinding("#RewardCount")] private NSText lblCount;
    [IWUIBinding("#RewardIco")] private Transform image;
    
    [SerializeField] private Material disabledMaterial;
    
    private Transform icon;
    private List<Image> IconSprites;
    
    public void UpdateUi(CurrencyPair reward, bool enabled)
    {
        lblCount.Text = reward.Amount > 1 && enabled ? "x" + reward.Amount : string.Empty;
        CreateIcon(image, reward.GetIcon());
        
        foreach (var sprite in IconSprites)
        {
            sprite.material = enabled ? null : disabledMaterial;
        }
    }
    
    private void CreateIcon(Transform parent, string id)
    {
        if (icon != null)
        {
            foreach (var sprite in IconSprites)
            {
                sprite.material = null;
            }
            
            UIService.Get.PoolContainer.Return(icon.gameObject);
            icon = null;
        }
        
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(parent);
        
        IconSprites = icon.GetComponentsInChildren<Image>().ToList();
    }
}