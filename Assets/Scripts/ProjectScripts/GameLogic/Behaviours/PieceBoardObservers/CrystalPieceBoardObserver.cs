using System.Collections.Generic;

public class CrystalPieceBoardObserver : PartPieceBoardObserver
{
    private readonly List<int> ids = new List<int>();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        var types = PieceType.GetIdsByFilter(PieceTypeFilter.Workplace | PieceTypeFilter.Multicellular, PieceTypeFilter.Mine);
        
        foreach (var id in types)
        {
            ids.Add(contextPiece.Context.BoardLogic.MatchDefinition.GetPrevious(id));
        }
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
    }

    public override void AddBubble(BoardPosition position, int pieceType)
    {
        foreach (var id in ids)
        {
            if (GameDataService.Current.CodexManager.IsPieceUnlocked(id) == false) continue;
            
            base.AddBubble(position, id);
        }
    }
}