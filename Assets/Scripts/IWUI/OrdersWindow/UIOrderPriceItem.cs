using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderPriceItem : IWUIWindowViewController
{
    [SerializeField] private List<UISimpleScrollItem> priceItems;
    [SerializeField] private Transform parent;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private ToggleGroup toggleGroup;

    private bool isScroll;

    public void Hide(ScrollRect scroll)
    {
        var value = scroll.velocity.y != 0;
        
        if(isScroll == value) return;
        
        isScroll = value; 
        
        if(isScroll == false) return;
        
        toggleGroup.SetAllTogglesOff();
    }
    
    public void HideHard()
    {
        Init(null, parent);
    }
    
    public void Init(OrderDef recipe, Transform target)
    {
        var time = 0.1f * canvasGroup.alpha;

        DOTween.Kill(canvasGroup);

        var sequence = DOTween.Sequence().SetId(canvasGroup);

        sequence.Insert(0, canvasGroup.DOFade(0, 0.03f * canvasGroup.alpha));
        sequence.InsertCallback(0.03f * canvasGroup.alpha, () =>
        {
            transform.SetParent(target, false);
            transform.localPosition = new Vector3(0, -75);
            transform.SetParent(parent, true);
            
            if(recipe == null) return;

            for (var i = 0; i < priceItems.Count; i++)
            {
                var isExcess = i >= recipe.Prices.Count;
                var item = priceItems[i];
            
                item.gameObject.SetActive(!isExcess);
            
                if(isExcess) continue;
            
                var price = recipe.Prices[i];
                var current = ProfileService.Current.GetStorageItem(price.Currency).Amount;
                var color = current == price.Amount ? "FFFFFF" : (current > price.Amount ? "28EC6D" : "EC5928"); 
            
                item.Init(price.Currency, $"<color=#{color}>{current}</color><size=35>/{price.Amount}</size>");
            }
        });

        if (recipe != null) sequence.Insert(0.03f * canvasGroup.alpha, canvasGroup.DOFade(1, 0.07f));
    } 
}