﻿using UnityEngine;

public class PieceStorageView : UIBoardView
{
    [SerializeField] private SpriteRenderer icon;

    private StorageComponent storage;
    private Animator clip;
    private bool clipIsPlay = true;
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, (multiSize == 0 ?  -0.3f : 0.7f)); }
    }

    protected override void SetOfset()
    {
        if (multiSize == 0)
        {
            base.SetOfset();
            return;
        }
        
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;
        
        icon.sprite = IconService.Current.GetSpriteById(PieceType.Parse(storage.SpawnPiece));
        storage.Timer.OnStart += UpdateView;
        storage.Timer.OnExecute += FindStartState;
        storage.Timer.OnComplete += UpdateView;
        UpdateView();
    }
    
    private void UpdateView()
    {
        if (storage == null) return;
        Change(storage.Filling / (float) storage.Capacity > 0.2);
        UpdateAnimation(storage.Filling != storage.Capacity);
    }
    
    private void UpdateAnimation(bool isPlay)
    {
        if(clip == null || clipIsPlay == isPlay) return;
        
        clipIsPlay = isPlay;

        clip.speed = clipIsPlay ? 1 : 0;
        clip.Play(clip.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
    }

    private void FindStartState()
    {
        if (clip == null)
        {
            var boardElement = Context.Context.RendererContext.GetElementAt(Context.CachedPosition);
            
            if (boardElement != null)
            {
                clip = boardElement.GetComponentInChildren<Animator>();
            }
        }
        
        storage.Timer.OnExecute -= FindStartState;
        
        if (clip == null || storage == null) return;
        
        UpdateAnimation(storage.Filling != storage.Capacity);
    }
}