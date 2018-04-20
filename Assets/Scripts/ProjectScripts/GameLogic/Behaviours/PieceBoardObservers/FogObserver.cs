using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver
{
    private int Free = 10;
    private int Uniq = 10;
    
    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        var key = new BoardPosition(position.X, position.Y);
        FogDef def;
        
        if(GameDataService.Current.FogsManager.Fogs.TryGetValue(key, out def) && def.Pieces != null)
        {
            foreach (var piece in def.Pieces)
            {
                context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
                {
                    At = GetPointInMask(realPosition, piece.Value),
                    PieceTypeId = PieceType.Parse(piece.Key)
                });
            }
        }
        
        for (int i = 0; i < Mask.Count; i++)
        {
            var point = GetPointInMask(position, Mask[i]);
            var index = Random.Range(0, 101);
            
            if(index < Free) continue;
            
            context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = index < (Free + Uniq) ? PieceType.OX1.Id : PieceType.O1.Id
            });
        }
    }
}