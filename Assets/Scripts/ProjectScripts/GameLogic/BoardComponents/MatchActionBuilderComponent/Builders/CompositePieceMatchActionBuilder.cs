using System.Collections.Generic;
using UnityEngine;

public class CompositePieceMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.A8.Id,
            PieceType.B9.Id,
            PieceType.C9.Id,
            PieceType.D9.Id,
            PieceType.E9.Id,
            PieceType.F9.Id,
            PieceType.G9.Id,
            PieceType.H9.Id,
            PieceType.I9.Id,
            PieceType.J9.Id,
            PieceType.EXT_A8.Id,
            PieceType.EXT_B8.Id,
            PieceType.EXT_C8.Id,
            PieceType.EXT_D8.Id,
            PieceType.EXT_E8.Id,
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

                if (definition.Context.IsPointValid(pos) == false) continue;
                
                starts.Add(pos);
            }
        }

        if (starts.Count == 0) return false;
        
        matchField.Clear();

        foreach (var start in starts)
        {
            var positions = CheckMatch(pattern, definition.Context, start);

            if (positions == null) continue;
            
            matchField.AddRange(positions);
        }
        
        return starts.Count > 0;
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

                if (definition.Context.IsPointValid(pos) == false) continue;
                
                starts.Add(pos);
            }
        }

        if (starts.Count == 0) return null;
        
        var options = new List<List<BoardPosition>>();
        
        foreach (var start in starts)
        {
            var positions = CheckMatch(pattern, definition.Context, start);
            
            if (positions == null) continue;

            if (positions[2].Equals(position))
            {
                matchField.Clear();
                matchField.AddRange(positions);
                return CreateAction(new List<int>{nextType}, positions, start, pieceType);
            }
            
            options.Add(positions);
        }

        if (options.Count == 0) return null;
        
        matchField.Clear();
        matchField.AddRange(options[0]);
        
        return CreateAction(new List<int>{nextType}, matchField, matchField[0], pieceType);
    }

    private List<BoardPosition> CheckMatch(List<List<int>> pattern, BoardLogicComponent logic, BoardPosition start)
    {
        var isBooster = false;
        var positions = new List<BoardPosition>();
            
        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];

            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(start.X + i, start.Y + j, start.Z);
                var piece = logic.GetPieceAt(pos);

                if (piece == null
                    || piece.PieceType != line[j] && piece.PieceType != PieceType.Boost_CR.Id
                    || piece.PieceType == PieceType.Boost_CR.Id && isBooster)
                {
                    return null;
                }

                if (piece.PieceType == PieceType.Boost_CR.Id) isBooster = true;
                
                positions.Add(pos);
            }
        }

        return positions;
    }
}