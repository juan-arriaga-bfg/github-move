using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver
{
    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);

        for (int i = 0; i < Mask.Count; i++)
        {
            var point = GetPointInMask(position, Mask[i]);
            var index = Random.Range(0, 10);
            
            if(index == 0) continue;
            
            context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = index == 1 ? PieceType.O2.Id : PieceType.O1.Id
            });
        }
    }
}