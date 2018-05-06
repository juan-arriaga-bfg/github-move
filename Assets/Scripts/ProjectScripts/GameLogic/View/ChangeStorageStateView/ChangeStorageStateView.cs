using UnityEngine;
using UnityEngine.UI;

public class ChangeStorageStateView : UIBoardView
{
    [SerializeField] private Image icon;
    
    protected override ViewType Id
    {
        get { return ViewType.StorageState; }
    }
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, 1.5f); }
    }

    protected override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        priority = 11;
        
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;

        var isChest = Context.Context.BoardLogic.MatchDefinition.GetFirst(storage.SpawnPiece) == PieceType.Chest1.Id;
        
        icon.sprite = IconService.Current.GetSpriteById(isChest ? "Chest" : PieceType.Parse(storage.SpawnPiece));
    }
}