using System.Collections.Generic;
using UnityEngine;

public class PrLockTutorialStep : DelayTutorialStep
{
    List<WorkplaceLifeComponent> lockedProductions = new List<WorkplaceLifeComponent>();

    public override void Execute()
    {
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.ProductionField, PieceTypeFilter.Obstacle);
        foreach (var pos in positions)
        {
            var piece = logic.GetPieceAt(pos);
            var workplace = piece?.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
            if (workplace == null || lockedProductions.Contains(workplace)) continue;
            workplace.Locker.Lock(this);
            lockedProductions.Add(workplace);
            
            //disable PR particles
            if(piece.ActorView == null) continue;
            var view = piece.ActorView as ReproductionPieceView;
            if(view == null) continue;
            view.ToggleEffectsByState(false);
        }
    }
    protected override void Complete()
    {
        foreach (var workplace in lockedProductions)
        {
            workplace.Locker.Unlock(this);
            var piece = workplace.Context;
            
            //enable PR particles
            if(piece?.ActorView == null) continue;
            var view = piece.ActorView as ReproductionPieceView;
            if(view == null) continue;
            view.ToggleEffectsByState(false);
        }
        
        base.Complete();
    }
}