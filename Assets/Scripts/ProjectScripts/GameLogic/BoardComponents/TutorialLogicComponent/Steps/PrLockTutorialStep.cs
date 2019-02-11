using System.Collections.Generic;
using UnityEngine;

public class PrLockTutorialStep : DelayTutorialStep
{
    protected override void Complete()
    {
        base.Complete();
        
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.ProductionField, PieceTypeFilter.Obstacle);
        foreach (var pos in positions)
        {
            var piece = logic.GetPieceAt(pos);
            
            //disable PR particles
            if(piece.ActorView == null) continue;
            var view = piece.ActorView as ReproductionPieceView;
            if(view == null) continue;
            view.OnTutorialEnd();
        }
    }
}