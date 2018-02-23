using System.Collections.Generic;

public interface IMatchActionBuilder
{
    IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position);
}