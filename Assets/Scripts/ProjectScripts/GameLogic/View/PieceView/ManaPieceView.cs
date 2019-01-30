using System.Collections.Generic;
using UnityEngine;

public class ManaPieceView : PieceBoardElementView
{
    private List<FogObserver> fogs = new List<FogObserver>();
    
    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(Piece.PieceType);
        var observers = GameDataService.Current.FogsManager.FogObservers;

        foreach (var observer in observers.Values)
        {
            if (observer.IsRemoved || observer.RequiredLevelReached() == false || observer.CanBeReached() == false) continue;
            
            observer.FillingFake(def.SpawnResources.Amount);
            fogs.Add(observer);
        }
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        foreach (var observer in fogs)
        {
            observer.FillingFake(0);
        }
        
        fogs = new List<FogObserver>();
        
        base.OnDragEnd(boardPos, worldPos);
    }
}
