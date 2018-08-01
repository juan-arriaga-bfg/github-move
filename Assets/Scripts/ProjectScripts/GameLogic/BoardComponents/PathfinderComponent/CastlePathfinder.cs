using UnityEngine;

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
            //var castle = board.BoardLogic.GetPieceAt(castlePosition);
            //var mask = castle.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid).Mask;
            //var width = mask[mask.Count-1].X + 1;
            //var height = mask[mask.Count-1].Y + 1;
            //var targetPositions = BoardPosition.GetRect(castlePosition.DownAtDistance(3).LeftAtDistance(3), width + 4, height + 4);
            var target = castlePosition.DownAtDistance(3).Right;
            return pathfinder.HasPath(from, target);
      }
}
