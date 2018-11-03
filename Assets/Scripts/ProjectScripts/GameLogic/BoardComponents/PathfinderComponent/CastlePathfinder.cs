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
            var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
            var targetPositions = new HashSet<BoardPosition>{position};

            List<BoardPosition> tmp;
            return pathfinder.HasPath(from, targetPositions, out tmp, piece);
      }
}