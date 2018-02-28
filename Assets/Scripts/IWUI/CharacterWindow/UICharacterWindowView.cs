using UnityEngine;
using DG.Tweening;

public class UICharacterWindowView : IWUIWindowView 
{
    [SerializeField] private RectTransform viewAnchor;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICharacterWindowModel windowModel = Model as UICharacterWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICharacterWindowModel windowModel = Model as UICharacterWindowModel;
        
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();

        viewAnchor.anchoredPosition = new Vector2(0f, -Screen.height);
        
        DOTween.Kill(viewAnchor);
        var sequence = DOTween.Sequence().SetId(viewAnchor);
        sequence.Append(viewAnchor.DOAnchorPos(new Vector2(0f, 0f), 0.5f).SetEase(Ease.OutBack));
    }

    public override void AnimateClose()
    {
        base.AnimateClose();

        DOTween.Kill(viewAnchor);
        var sequence = DOTween.Sequence().SetId(viewAnchor);
        sequence.Append(viewAnchor.DOAnchorPos(new Vector2(0f, -Screen.height), 0.5f).SetEase(Ease.InBack));
    }
}