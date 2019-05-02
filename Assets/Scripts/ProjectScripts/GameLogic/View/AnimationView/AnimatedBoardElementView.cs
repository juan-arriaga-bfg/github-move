using DG.Tweening;
using UnityEngine;

public class AnimatedBoardElementView : BoardElementView
{
    private Animator controller;
    
    private readonly int animationIdShow = Animator.StringToHash("Show");
    private readonly int animationIdIdle = Animator.StringToHash("Idle");
    private readonly int animationIdHide = Animator.StringToHash("Hide");
    
    public override void Init(BoardRenderer context)
    {
        base.Init(context);

        controller = GetComponentInChildren<Animator>();
    }

    public virtual void PlayShow()
    {
        if (controller != null) controller.SetTrigger(animationIdShow);
    }

    public virtual void PlayIdle()
    {
        if (controller != null) controller.SetTrigger(animationIdIdle);
    }

    public virtual void PlayHide(float duration = 0)
    {
        if (controller != null) controller.SetTrigger(animationIdHide);
        
        DOTween.Sequence()
            .AppendInterval(duration)
            .AppendCallback(() => Context.DestroyElement(this));
    }
}