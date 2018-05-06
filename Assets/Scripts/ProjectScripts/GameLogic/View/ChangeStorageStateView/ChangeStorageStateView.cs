using UnityEngine;
using UnityEngine.UI;

public class ChangeStorageStateView : UIBoardView
{
    [SerializeField] private Image icon;
    
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
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;

        var isChest = Context.Context.BoardLogic.MatchDefinition.GetFirst(storage.SpawnPiece) == PieceType.Chest1.Id;
        
        icon.sprite = IconService.Current.GetSpriteById(isChest ? "Chest" : PieceType.Parse(storage.SpawnPiece));
    }
}