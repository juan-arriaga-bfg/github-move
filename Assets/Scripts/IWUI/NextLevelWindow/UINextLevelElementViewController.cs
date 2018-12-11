using DG.Tweening;
using UnityEngine;

public class UINextLevelElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding] private CanvasGroup canvas;
    
    private const float ANIMATION_TIME = 0.5f;

    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UINextLevelElementEntity;
        var delay = contentEntity.Delay;
        
        canvas.alpha = 0;
        CachedTransform.localScale = Vector3.zero;
        
        DOTween.Kill(this);
        
        var sequence = DOTween.Sequence().SetId(this);
        
        sequence.Insert(delay, canvas.DOFade(1, ANIMATION_TIME).SetEase(Ease.OutSine));
        sequence.Insert(delay, CachedTransform.DOScale(Vector3.one, ANIMATION_TIME).SetEase(Ease.OutElastic));
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        
        var contentEntity = entity as UINextLevelElementEntity;
        
        if(contentEntity == null) return;
        
        DOTween.Kill(this);
        
        var speed = ANIMATION_TIME * 0.5f / canvas.alpha;
        canvas.DOFade(0, speed).SetId(this);
    }
}