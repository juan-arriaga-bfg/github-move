using System.Collections.Generic;

public class СompositePieceMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.Zord1.Id,
            PieceType.Zord2.Id,
            PieceType.Zord3.Id,
            PieceType.Zord4.Id
        };
    }
    
    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;

        var pattern = definition.GetPattern(nextType);
        BoardPosition? start = null;

        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];
            
            for (var j = 0; j < line.Count; j++)
            {
                if (line[j] != pieceType) continue;

                start = new BoardPosition(position.X - i, position.Y - j, position.Z);
                break;
            }
            
            if(start != null) break;
        }
        
        if(start == null) return null;

        var positions = new List<BoardPosition>();

        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];

            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(start.Value.X + i, start.Value.Y + j, position.Z);
                var piece = definition.Context.GetPieceAt(pos);

                if (piece == null || piece.PieceType != line[j]) return null;
                
                positions.Add(pos);
            }
        }
        
        var nextAction = new SpawnPieceAtAction
        {
            IsCheckMatch = false,
            At = start.Value,
            PieceTypeId = nextType,
            OnSuccessEvent = pos => SpawnReward(pos, nextType)
        };
        
        return new CollapsePieceToAction
        {
            To = start.Value,
            Positions = positions,
            OnCompleteAction = nextAction
        };
    }
}