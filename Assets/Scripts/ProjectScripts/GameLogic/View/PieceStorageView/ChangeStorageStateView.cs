﻿using UnityEngine;

public class ChangeStorageStateView : IWBaseMonoBehaviour
{
    [SerializeField] private GameObject iconGo;
    [SerializeField] private SpriteRenderer icon;
    
    private PieceBoardElementView context;
    private StorageComponent storage;
    
    public void Init(PieceBoardElementView context)
    {
        this.context = context;
        iconGo.SetActive(false);
    }
    
    private void Update()
    {
        if (context == null || context.Piece == null) return;

        if (storage == null)
        {
            storage = context.Piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
            
            if (storage == null) return;

            icon.sprite = IconService.Current.GetSpriteById(PieceType.Parse(storage.SpawnPiece));
        }
        
        iconGo.SetActive(storage.Filling > 0);
    }
}