using System.Collections.Generic;
using UnityEngine;

public class СompositePieceMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.A8.Id,
            PieceType.B10.Id,
            PieceType.C8.Id,
            PieceType.D8.Id,
        };
    }

    public bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next)
    {
        next = definition.GetNext(pieceType, false);

        if (next == PieceType.None.Id) return false;

        var pattern = definition.GetPattern(next);
        var starts = new List<BoardPosition>();
        
        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];
            
            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(position.X - i, position.Y - j, position.Z);
                
                if(definition.Context.IsPointValid(pos) == false) continue;
                
                starts.Add(pos);
            }
        }
        
        if(starts.Count == 0) return false;
        
        matchField.Clear();

        foreach (var start in starts)
        {
            var positions = CheckMatch(pattern, definition.Context, start);

            if (positions == null) continue;
            
            matchField.AddRange(positions);
            return true;
        }
        
        return false;
    }

    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;

        var pattern = definition.GetPattern(nextType);
        var starts = new List<BoardPosition>();

        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];
            
            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(position.X - i, position.Y - j, position.Z);
                
                if(definition.Context.IsPointValid(pos) == false) continue;
                
                starts.Add(pos);
            }
        }
        
        if(starts.Count == 0) return null;
        
        var at = BoardPosition.Default();
        List<BoardPosition> positions = null;
        
        foreach (var start in starts)
        {
            positions = CheckMatch(pattern, definition.Context, start);
            at = start;
            if(positions != null) break;
        }

        if(positions == null) return null;
        
        matchField.Clear();
        matchField.AddRange(positions);
        
        return CreateAction(new List<int>{nextType}, positions, at, pieceType);
    }

    private List<BoardPosition> CheckMatch(List<List<int>> pattern, BoardLogicComponent logic, BoardPosition start)
    {
        var positions = new List<BoardPosition>();
            
        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];

            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(start.X + i, start.Y + j, start.Z);
                var piece = logic.GetPieceAt(pos);

                if (piece == null || piece.PieceType != line[j]) return null;
                
                positions.Add(pos);
            }
        }

        return positions;
    }
}