using System.Collections.Generic;

public interface IMatchActionBuilder
{
    List<int> GetKeys();
    
    bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next);
    IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position);
}