using System;
using System.Collections.Generic;
using UnityEngine;

public class AirShipEjectionPieceAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;
    
    public Vector3 From;
    public BoardElementView BoardElement;
    public Dictionary<BoardPosition, int> Pieces;
    
    public Func<int, string> AnimationResourceSearch;
    
    public Action OnComplete;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        var logic = gameBoardController.BoardLogic;
        var pieces = new Dictionary<BoardPosition, Piece>();
        
        if (AnimationResourceSearch == null) AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnMultiSpawn);
        
        foreach (var pair in Pieces)
        {
            var position = pair.Key;
            var piece = gameBoardController.CreatePieceFromType(pair.Value);

            if (logic.AddPieceToBoard(position.X, position.Y, piece) == false) continue;
			
            pieces.Add(position, piece);
            logic.LockCell(position, this);
        }
        
        var animation = new ReproductionPieceAnimation
        {
            StartPosition = From,
            BoardElement = BoardElement,
            Pieces = pieces,
            AnimationResourceSearch = AnimationResourceSearch
        };
        
        animation.OnCompleteEvent += (_) =>
        {
            foreach (var pair in Pieces)
            {
                logic.UnlockCell(pair.Key, this);
            }
            
            OnComplete?.Invoke();
        };
		
        gameBoardController.RendererContext.AddAnimationToQueue(animation);
        
        return true;
    }
}