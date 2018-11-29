﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MagicPieceView : PieceBoardElementView
{   
    private readonly ViewAnimationUid AnimationId = new ViewAnimationUid();
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        DOTween.Kill(AnimationId);
       
    }

    private void OnDisable()
    {
        DOTween.Kill(AnimationId);
    }

    private List<Piece> bestMatchPieces = new List<Piece>();

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);

        var board = Context.Context;        
        bestMatchPieces = FindBestMatches(board);
        foreach (var piece in bestMatchPieces)
        {
            if(piece.PieceType == PieceType.Boost_CR.Id)
                continue;
  
            var view = piece.ActorView;
            if(view == null)
                continue;
            HighlightMatchable(piece.ActorView);
        }
    }

    private void HighlightMatchable(PieceBoardElementView view)
    {
        view.ToggleHighlight(true);
        view.ToggleSelection(true);
    }

    private void OffHighlightMatchable(PieceBoardElementView view)
    {
        view.ToggleHighlight(false);
        view.ToggleSelection(false);
    }

    private List<Piece> FindBestMatches(BoardController board)
    {
        var currentBestPieces = new List<Piece>();
        
        // Uncomment to select all the pieces at the gameboard on CR drag. Usefull for highlight testing for example 
        // for (int x = 0; x < board.BoardDef.Width; x++)
        // {
        //     for (int y = 0; y < board.BoardDef.Height; y++)
        //     {
        //         var piece = board.BoardLogic.GetPieceAt(new BoardPosition(x, y, board.BoardDef.PieceLayer));
        //         if (piece != null)
        //         {
        //             currentBestPieces.Add(piece);
        //         }
        //     }
        // }
        //
        // return currentBestPieces;
        
        var entities = board.BoardLogic.BoardEntities;
        
        if (entities == null) return new List<Piece>();

        var matchCheckedPositions = new Dictionary<BoardPosition, bool>();
        var options = new List<KeyValuePair<int, List<BoardPosition>>>();
        var bestScore = 0;
        
        foreach (var piece in entities.Values)
        {   
            var isMatchable = piece.Matchable?.IsMatchable();
            
            if (isMatchable != true || matchCheckedPositions.ContainsKey(piece.CachedPosition)) continue;

            var canCreateMatch = false;
            var positions = new List<BoardPosition>();
            
            canCreateMatch = CheckMatch(board, Piece.CachedPosition, piece.CachedPosition, positions);

            positions.Remove(Piece.CachedPosition);

            foreach (var position in positions)
            {
                if (matchCheckedPositions.ContainsKey(position)) continue;
                
                matchCheckedPositions.Add(position, canCreateMatch);
            }
            
            if(canCreateMatch == false) continue;
            
            var score = CalcScore(piece, positions);
            
            if (bestScore < score) bestScore = score;

            options.Add(new KeyValuePair<int, List<BoardPosition>>(score, positions));
        }

        options = options.FindAll(pair => pair.Key == bestScore);
        
        var immediate = new List<BoardPosition>();
        
        foreach (var option in options)
        {
            immediate.AddRange(Piece.CachedPosition.GetImmediate(option.Value));
        }
        
        var best = Piece.CachedPosition.GetImmediate(immediate)[0];
        var bestOption = options.Find(pair => pair.Value.Contains(best)).Value;
        
        bestOption.Add(Piece.CachedPosition);
        
        foreach (var pos in bestOption)
        {
            currentBestPieces.Add(board.BoardLogic.GetPieceAt(pos));
        }
        
        return currentBestPieces;
    }

    private int CalcScore(Piece piece, List<BoardPosition> matchPositions)
    {
        var chain = Context.Context.BoardLogic.MatchDefinition.GetChain(piece.PieceType);
        var length = chain.Count;
        const float lengthFactor = 0.5f;
        var position = chain.IndexOf(piece.PieceType);
        const int positionFactor = 2;
        var matchLength = matchPositions.Count;
        const float matchLengthFactor = 0.001f;
        
        return (int)((length * lengthFactor + position * positionFactor + matchLength * matchLengthFactor) * 1000);
    }
    
    private bool CheckMatch(BoardController board, BoardPosition from, BoardPosition to, List<BoardPosition> positions)
    {
        positions.Add(from);
        
        var logic = board.BoardLogic;
		
        int currentId;
        int nextId;
        
        return logic.FieldFinder.Find(to, positions, out currentId) && logic.MatchActionBuilder.CheckMatch(positions, currentId, to, out nextId);
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);

        foreach (var piece in bestMatchPieces)
        {
            if(piece.PieceType == PieceType.Boost_CR.Id)
                continue;
            
            var view = piece.ActorView;
            if(view == null)
                continue;
            OffHighlightMatchable(view);
        }
    }
}