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
            var castlePosition = GameDataService.Current.PiecesManager.CastlePosition;
            var castle = board.BoardLogic.GetPieceAt(castlePosition);
            
            if(castle == null) return true;
            
            var mask = castle.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid).Mask;
            var width = mask[mask.Count-1].X + 1;
            var height = mask[mask.Count-1].Y + 1;
            var targetPositions = new HashSet<BoardPosition>(BoardPosition.GetRect(castlePosition.DownAtDistance(2).LeftAtDistance(2), width + 4, height + 4));
            
            return pathfinder.HasPath(from, targetPositions, piece);
      }
}
