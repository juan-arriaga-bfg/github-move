using System.Collections.Generic;

public static class ExtendedPathfinder
{
      public static bool CanPathToOpenArea(this PathfinderComponent pathfinder, Piece piece)
      {
            return pathfinder.CanPathToOpenArea(piece, piece.CachedPosition);
      }

      public static bool CanPathToOpenArea(this PathfinderComponent pathfinder, Piece piece, BoardPosition from)
      {
            var board = piece.Context;
            
            var targetPositions = board.AreaAccessController?.AvailiablePositions;

            if (targetPositions == null)
                  return false;
            
            List<BoardPosition> tmp;
            return pathfinder.HasPath(from, targetPositions, out tmp, piece);
      }
}