﻿using UnityEngine;

public class PieceStorageView : PieceBoardElementView
{
    [SerializeField] private Animator animator;
    
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
        animator.speed = storage.Timer.IsExecuteable() ? 1 : 0;
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
    }
}