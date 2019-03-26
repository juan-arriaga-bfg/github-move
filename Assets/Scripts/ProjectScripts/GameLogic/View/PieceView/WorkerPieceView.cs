using System.Collections.Generic;
using UnityEngine;

public class WorkerPieceView : PieceBoardElementView
{
    private List<Piece> targets;
    
    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);

        targets = FindTargets();
        
        foreach (var target in targets)
        {
            Highlight(target.ActorView, true);
        }
    }
    
    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);

        if (targets == null) return;

        foreach (var target in targets)
        {
            Highlight(target.ActorView, false);
        }

        targets = null;
    }

    private List<Piece> FindTargets()
    {
        var logic = Context.Context.BoardLogic;
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace);
        var result = new List<Piece>();
        
        positions.AddRange(Context.Context.PartPiecesLogic.GetAllPositions());

        foreach (var position in positions)
        {
            var piece = logic.GetPieceAt(position);

            if (Context.Context.PathfindLocker.HasPath(piece) == false
                || piece.PieceState != null && piece.PieceState.State != BuildingState.Waiting && piece.PieceState.State != BuildingState.Warning) continue;
            
            result.Add(piece);
        }
        
        return result;
    }
    
    private void Highlight(PieceBoardElementView view, bool isOn)
    {
        view.ToggleHighlight(isOn);
        view.ToggleSelection(isOn);
    }
}