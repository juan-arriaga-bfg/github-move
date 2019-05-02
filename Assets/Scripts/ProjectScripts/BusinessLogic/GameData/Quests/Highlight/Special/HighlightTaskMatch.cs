using System.Collections.Generic;

public class HighlightTaskMatch : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId taskPiece = task as IHavePieceId;
        if (taskPiece == null)
        {
            return false;
        }
        
        if (taskPiece.PieceId != PieceType.Empty.Id && taskPiece.PieceId != PieceType.None.Id)
        {
            return false;
        }

        return ShowForAbstractPiece();
    }

    private bool ShowForAbstractPiece()
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var board = BoardService.Current.FirstBoard;
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

        PieceTypeFilter include = PieceTypeFilter.Simple;
        PieceTypeFilter exclude = PieceTypeFilter.Obstacle
                                | PieceTypeFilter.Fake
                                | PieceTypeFilter.Mine
                                | PieceTypeFilter.Multicellular
                                | PieceTypeFilter.Character
                                | PieceTypeFilter.Enemy
                                | PieceTypeFilter.Booster
                                | PieceTypeFilter.Chest
                                | PieceTypeFilter.ProductionField;

        var allPieces = boardLogic.PositionsCache.GetPiecePositionsByFilter(include, exclude);
        
        // count all pieces and remove last in chain
        Dictionary<int, List<BoardPosition>> allPiecesDict = new Dictionary<int, List<BoardPosition>>(); 
        foreach (var position in allPieces)
        {
            Piece piece = boardLogic.GetPieceAt(position);
            int id = piece.PieceType;

            List<BoardPosition> list;
            if (!allPiecesDict.TryGetValue(id, out list))
            {
                int next = matchDef.GetNext(id);
                if (next == PieceType.None.Id)
                {
                    continue;
                }

                var nextDef = PieceType.GetDefById(next);
                if (nextDef.Filter.Has(PieceTypeFilter.Multicellular))
                {
                    continue;
                }
                
                list = new List<BoardPosition>();
                allPiecesDict.Add(id, list);
            }

            list.Add(position);
        }

        // remove count < 3
        Dictionary<int, List<BoardPosition>> x3Dict = new Dictionary<int, List<BoardPosition>>();
        foreach (var pair in allPiecesDict)
        {
            if (pair.Value.Count >= 3)
            {
                x3Dict.Add(pair.Key, pair.Value);
            }
        }
        
        List<BoardPosition> unlocked = new List<BoardPosition>();

        var pathfinder = BoardService.Current.FirstBoard.PathfindLocker;

        foreach (var pair in x3Dict)
        {
            var positions = pair.Value;
            unlocked.Clear();
            foreach (var position in positions)
            {
                if (pathfinder.HasPath(boardLogic.GetPieceAt(position)))
                {
                    unlocked.Add(position);
                }
            }

            if (unlocked.Count >= 3)
            {
                HighlightTaskPointToPieceHelper.ShowArrowForRandomPos(unlocked);
                return true;
            }
        }

        return false;
    }
}