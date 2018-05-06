using UnityEngine;

public class PieceStorageView : PieceBoardElementView
{
    [SerializeField] private Animator animator;
    
    private bool clipIsPlay = true;
    private StorageComponent storage;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if(storage == null) return;

        storage.Timer.OnStart += UpdateAnimation;
        storage.Timer.OnComplete += UpdateAnimation;
        
        UpdateAnimation();
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        storage.Timer.OnStart -= UpdateAnimation;
        storage.Timer.OnComplete -= UpdateAnimation;
    }

    private void UpdateAnimation()
    {
        var isPlay = storage.Filling != storage.Capacity;
        
        if(clipIsPlay == isPlay) return;
        
        clipIsPlay = isPlay;

        animator.speed = clipIsPlay ? 1 : 0;
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
    }
}