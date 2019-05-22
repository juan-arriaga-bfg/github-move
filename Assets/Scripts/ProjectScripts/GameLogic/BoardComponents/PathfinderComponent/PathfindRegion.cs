using System;
using System.Collections.Generic;
using System.Linq;
public class PathfindRegion
    {
        private HashSet<BoardPosition> regPositions;
        
        private HashSet<Piece> piecesOnRegion;
        public HashSet<Piece> RegionPieces => piecesOnRegion;

        private BoardController board;

        private List<BoardPosition> blockPathPieces = new List<BoardPosition>();
        public List<BoardPosition> BlockPathPieces => blockPathPieces;
        
        public bool Contains(BoardPosition position)
        {
            return regPositions.Contains(position);
        }

        public void AddPosition(BoardPosition position)
        {
            position = new BoardPosition(position.X, position.Y, BoardLayer.Piece.Layer);
            regPositions.Add(position);
            
            var pieceOnPos = board.BoardLogic.GetPieceAt(position);
            if (pieceOnPos != null && piecesOnRegion.Contains(pieceOnPos) == false)
            {
                piecesOnRegion.Add(pieceOnPos);
            }
                
        }

        public void RemovePosition(BoardPosition position)
        {
            position = new BoardPosition(position.X, position.Y, BoardLayer.Piece.Layer);
            regPositions.Remove(position);
            
            var pieceOnPos = board.BoardLogic.GetPieceAt(position);
            if (pieceOnPos != null && piecesOnRegion.Contains(pieceOnPos))
                piecesOnRegion.Remove(pieceOnPos);
        }

        public bool RecalculateState(Action<HashSet<Piece>> onRegionOpen, Piece changedPiece = null)
        {
            if (changedPiece != null &&
                piecesOnRegion.Contains(changedPiece) &&
                board.BoardLogic.GetPieceAt(changedPiece.CachedPosition) == null)
                piecesOnRegion.Remove(changedPiece);
            
            if (piecesOnRegion.Count == 0)
            {
                return false;
            }
                
            var firstPiece = piecesOnRegion.First();
            var canPath = board.Pathfinder.HasPath(firstPiece.CachedPosition, board.AreaAccessController.AvailiablePositions, 
                                                   out blockPathPieces, firstPiece, board.Pathfinder.GetCondition(firstPiece));
            if (canPath)
            {
                onRegionOpen?.Invoke(piecesOnRegion);
            }
                
            return canPath;
        }
        
        public PathfindRegion(BoardController boardController)
        {
            board = boardController;
            regPositions = new HashSet<BoardPosition>();
            new List<BoardPosition>();
            piecesOnRegion = new HashSet<Piece>();
        }
    }