using UnityEngine;
using DG.Tweening;

public class UISampleWindowView : UIGenericWindowView
{
    [SerializeField] private RectTransform viewAnchor;
    
    [SerializeField] private NSText label;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UISampleWindowModel windowModel = Model as UISampleWindowModel;

        label.Text = string.Format("RandomNumber:{0}", windowModel.RandomNumber.ToString());

    }

    public override void OnViewClose()
    {
        base.OnViewClose();

        
        UISampleWindowModel windowModel = Model as UISampleWindowModel;
        
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
