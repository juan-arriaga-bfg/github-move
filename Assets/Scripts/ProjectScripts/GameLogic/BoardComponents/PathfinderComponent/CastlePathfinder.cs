using System.Collections.Generic;

public static class CastlePathfinder
{
      public static bool CanPathToCastle(this PathfinderComponent pathfinder, Piece piece)
      {
            return pathfinder.CanPathToCastle(piece, piece.CachedPosition);
      }

      public static bool CanPathToCastle(this PathfinderComponent pathfinder, Piece piece, BoardPosition from)
      {
            var board = piece.Context;
            var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Hero1.Id, 1)[0];
            var targetPositions = new HashSet<BoardPosition>{position};
            
            return pathfinder.HasPath(from, targetPositions, piece);
      }
}