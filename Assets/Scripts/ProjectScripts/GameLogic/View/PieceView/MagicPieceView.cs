using System.Collections.Generic;
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
            if(piece.PieceType == PieceType.Magic.Id)
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
        if (entities == null)
            return new List<Piece>();

        var matchCheckedPositions = new Dictionary<BoardPosition, bool>();
        
        float currentBestScore = 0;
        
        foreach (var piece in entities.Values)
        {   
            var isMatchable = piece.Matchable?.IsMatchable();
            
            if (isMatchable != true || matchCheckedPositions.ContainsKey(piece.CachedPosition))
                continue;

            bool canCreateMatch = false;
           
            var positions = new List<BoardPosition>();
            canCreateMatch = CheckMatch(board, Piece.CachedPosition, piece.CachedPosition, positions);

            foreach (var position in positions)
            {
                if (matchCheckedPositions.ContainsKey(position))
                    continue;
                matchCheckedPositions.Add(position, canCreateMatch);
            }


            if(canCreateMatch == false)
                continue;

            var score = CalcScore(piece, positions);
            if (score > currentBestScore)
            {
                currentBestScore = score;
                currentBestPieces = new List<Piece>();
                foreach (var pos in positions)
                {
                    currentBestPieces.Add(board.BoardLogic.GetPieceAt(pos));
                }
            }
        }
        
        return currentBestPieces;
    }

    private float CalcScore(Piece piece, List<BoardPosition> matchPositions)
    {
        var chain = GetChain(piece);
        var length = chain.Count;
        var lengthFactor = 0.5f;
        var position = chain.IndexOf(piece.PieceType);
        var positionFactor = 2;
        var matchLength = matchPositions.Count;
        var matchLengthFactor = 0.001f;
        return length * lengthFactor + position * positionFactor + matchLength * matchLengthFactor;
    }
    
    private bool CheckMatch(BoardController board, BoardPosition from, BoardPosition to, List<BoardPosition> positions)
    {
        positions.Add(from);
        var logic = board.BoardLogic;
		
        int currentId;
        if (logic.FieldFinder.Find(to, positions, out currentId) == false) return false;
        var action = logic.MatchActionBuilder.GetMatchAction(positions, currentId, to);
        var isMatch = action != null;
        if (isMatch) board.ReproductionLogic.Restart();

        return isMatch;
    }

    private List<int> GetChain(Piece piece)
    {
        var board = Context.Context;
        var chain = board.BoardLogic.MatchDefinition?.GetChain(piece.PieceType);
        if (chain == null)
            return new List<int>();
        return chain;
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);

        foreach (var piece in bestMatchPieces)
        {
            if(piece.PieceType == PieceType.Magic.Id)
                continue;
            
            var view = piece.ActorView;
            if(view == null)
                continue;
            OffHighlightMatchable(view);
        }
    }
}