public class FogObserver : MulticellularPieceBoardObserver
{
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var key = new BoardPosition(position.X, position.Y);
        var def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;

        Mask = BoardPosition.GetRect(BoardPosition.Zero(), def.Size.X, def.Size.Y);
        
        base.OnAddToBoard(position, context);
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        var key = new BoardPosition(position.X, position.Y);
        var def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;
        
        if(def.Pieces != null)
        {
            foreach (var piece in def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
                    {
                        At = GetPointInMask(realPosition, pos),
                        PieceTypeId = PieceType.Parse(piece.Key)
                    });
                }
            }
        }

        var weights = def.PieceWeights == null || def.PieceWeights.Count == 0
            ? GameDataService.Current.FogsManager.DefaultPieceWeights
            : def.PieceWeights;
        
        for (int i = 0; i < Mask.Count; i++)
        {
            var point = GetPointInMask(realPosition, Mask[i]);
            var piece = ItemWeight.GetRandomItem(weights).Piece;
            
            if(piece == PieceType.Empty.Id) continue;
            
            context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = piece
            });
        }
    }
}