using System.Collections.Generic;
using UnityEngine;

public class SimpleMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>();
    }

    public bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next)
    {
        next = definition.GetNext(pieceType, false);
        
        if (next == PieceType.None.Id) return false;
        
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        return countForMatchDefault != -1 && matchField.Count >= countForMatchDefault;
    }
    
    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = matchField.Count;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatchDefault == -1 || countForMatch < countForMatchDefault) return null;
        
        var nextPieces = Add(Mathf.RoundToInt(countForMatch / 3f), nextType, new List<int>());
        
        if(countForMatch % 3 == 1) nextPieces = Add(1, pieceType, nextPieces);

        var nextAction = new SpawnPiecesAction
        {
            IsCheckMatch = false,
            At = position,
            Pieces = nextPieces,
            IsMatch = true,
            OnSuccessEvent = list =>
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (nextPieces[i] == pieceType) continue;
                    
                    SpawnReward(list[i], nextPieces[i]);
                }
            }
        };

        MatchDescription matchDescription = new MatchDescription
        {
            SourcePieceType = pieceType,
            MatchedPiecesCount = matchField.Count,
            CreatedPieceType = nextType,
        };
            
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, matchDescription);
        
        return new CollapsePieceToAction
        {
            To = position,
            Positions = matchField,
            IsMatch = true,
            OnCompleteAction = nextAction
        };
    }
    
    private List<int> Add(int count, int piece, List<int> pieces)
    {
        for (var i = 0; i < count; i++)
        {
            pieces.Add(piece);
        }

        return pieces;
    }
}