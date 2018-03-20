using System.Collections.Generic;

public interface IMatchActionBuilder
{
    List<int> GetKeys();
    IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position);
}