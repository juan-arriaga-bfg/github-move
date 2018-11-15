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

            var positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1);

            if (positions.Count == 0)
            {
                  positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeautyPlaid.Id, 1);
            }
            
            if (positions.Count == 0)
            {
                  positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.A7.Id, 1);
            }
            
            var targetPositions = new HashSet<BoardPosition>{positions[0]};

            List<BoardPosition> tmp;
            return pathfinder.HasPath(from, targetPositions, out tmp, piece);
      }
}