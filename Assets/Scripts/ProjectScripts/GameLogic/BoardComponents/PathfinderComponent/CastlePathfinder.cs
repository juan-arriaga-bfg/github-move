using System.Linq;

public static class CastlePathfinder
{
      public static bool CanPathToCastle(this PathfinderComponent pathfinder, Piece piece)
      {
            var board = piece.Context;
            var castlePosition = GameDataService.Current.PiecesManager.CastlePosition;
            var castle = board.BoardLogic.GetPieceAt(castlePosition);
            var mask = castle.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid).Mask;
            var width = mask.Max(elem => elem.X);
            var height = mask.Max(elem => elem.Y);
            var targetPositions = BoardPosition.GetRect(castlePosition.DownAtDistance(3).LeftAtDistance(3), width + 4, height + 4);
            return pathfinder.HasPath(piece.CachedPosition, targetPositions);
      }
}
