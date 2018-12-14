using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderPriceItem : IWUIWindowViewController
{
    [IWUIBinding("/canvas/#Body/Back/Contents/Tabs/#TabRecipes/Viewport/#ContentRecipes", true)] private UIContainerViewController contentRecipes;
    [IWUIBinding] private UIContainerViewController container;
    [IWUIBinding] private CanvasGroup canvasGroup;
    
    private Transform parent;
    private Transform target;
    private bool isScroll;

    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);

        parent = CachedTransform.parent;
    }

    public void Hide(ScrollRect scroll)
    {
        var value = scroll.velocity.y != 0;
        
        if(isScroll == value) return;
        
        isScroll = value; 
        
        if(isScroll == false) return;
        
        contentRecipes.Deselect(contentRecipes.GetSelectedTab());
        Select(null, parent);
    }
    
    public void HideHard()
    {
        Select(null, parent);
    }
    
    public void Select(List<CurrencyPair> entities, Transform target)
    {
        if(this.target == target) return;
        
        DOTween.Kill(canvasGroup);
        CachedTransform.localScale = Vector3.one;

        var sequence = DOTween.Sequence().SetId(canvasGroup);

        sequence.Insert(0, canvasGroup.DOFade(0, 0.03f * canvasGroup.alpha));
        sequence.InsertCallback(0.03f * canvasGroup.alpha, () =>
        {
            this.target = target;
            CachedTransform.SetParent(target, false);
            CachedTransform.localPosition = new Vector3(0, -75);
            CachedTransform.SetParent(parent, true);
            CachedTransform.localScale = Vector3.one;
            
            if (entities == null || entities.Count <= 0)
            {
                container.Clear();
                return;
            }
            
            var views = new List<IUIContainerElementEntity>(entities.Count);
        
            for (var i = 0; i < entities.Count; i++)
            {
                var def = entities[i];
                var current = ProfileService.Current.GetStorageItem(def.Currency).Amount;
                var color = current == def.Amount ? "FFFFFF" : (current > def.Amount ? "28EC6D" : "EC5928"); 
            
                var entity = new UISimpleScrollElementEntity
                {
                    ContentId = def.Currency,
                    LabelText = $"<color=#{color}>{current}</color><size=35>/{def.Amount}</size>",
                    OnSelectEvent = null,
                    OnDeselectEvent = null
                };
            
                views.Add(entity);
            }
            
            container.Create(views);
        });

        if (entities != null) sequence.Insert(0.03f * canvasGroup.alpha, canvasGroup.DOFade(1, 0.07f));
    } 
}