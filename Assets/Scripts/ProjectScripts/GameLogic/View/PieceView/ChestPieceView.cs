using UnityEngine;

public class ChestPieceView : PieceBoardElementView
{
    [SerializeField] private Transform cap;
    
    [SerializeField] private GameObject shine;
    
    private ChestPieceComponent chestComponent;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
        
        if(chestComponent == null || chestComponent.Chest == null) return;

        var isOpen = chestComponent.Chest.State == ChestState.Open;
        
        shine.SetActive(isOpen);
        
        cap.localPosition = new Vector3(cap.localPosition.x, isOpen ? 0.66f : 0.53f);
    }
}