﻿using UnityEngine;
using UnityEngine.UI;

public class ChangeStorageStateView : UIBoardView
{
    [SerializeField] private Image icon;

    protected override ViewType Id => ViewType.StorageState;

    public override Vector3 Ofset => new Vector3(0, 1.5f);

    private bool isClick;

    public override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 11;
        isClick = false;
        
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;
        
        icon.sprite = IconService.Current.GetSpriteById(storage.Icon);
    }
    
    public void OnClick()
    {
        var definition = Context.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

        definition?.Touch(Context.CachedPosition);
    }
}