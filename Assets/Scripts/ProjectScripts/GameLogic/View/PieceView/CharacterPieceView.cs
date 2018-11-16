using UnityEngine;

public class CharacterPieceView : PieceBoardElementView
{
    [SerializeField] private Animator charAnimator;

    private CharacterControllerAnimation controller;
    
    public void StartRewardAnimation()
    {
        if(charAnimator == null) return;
        if (controller == null) controller = charAnimator.GetBehaviour<CharacterControllerAnimation>();
        if (controller == null) return;
        
        controller.StartRewardAnimation(charAnimator);
    }
    
    public override void ResetViewOnDestroy()
    {
        if (controller != null) controller.Destroy();
        controller = null;
        
        base.ResetViewOnDestroy();
    }
}