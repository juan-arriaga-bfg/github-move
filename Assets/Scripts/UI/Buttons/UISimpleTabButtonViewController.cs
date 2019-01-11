using DG.Tweening;
using UnityEngine;

public class UISimpleTabButtonViewController : UIBaseButtonViewController
{
    [IWUIBindingNullable("#Checkmark")] private CanvasGroup checkmark;
    
    public override void UpdateView()
    {
        base.UpdateView();
        
        if(checkmark == null) return;
        
        var alpha = state == GenericButtonState.Active ? 1f : 0f;
        
        DOTween.Kill(checkmark);
        checkmark.DOFade(alpha, 0.1f).SetId(checkmark);
    }
    
    protected override void AnimateOnPointerDown()
    {
    }
    
    protected override void AnimateOnPointerUp()
    {
    }
    
    protected override void AnimateOnPointerClick()
    {
    }

    public override void PlaySound()
    {
        NSAudioService.Current.Play(SoundId.PopupTabs);
    }
}