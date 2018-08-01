using System.Collections.Generic;
using UnityEngine;

public class DraggablePieceComponent : IECSComponent, IDraggable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }

    private Piece piece;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        piece = entity as Piece;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual bool IsDraggable(BoardPosition at)
    {
        return true;
    }

    public virtual bool IsValidDrag(BoardPosition to)
    {
        
        var board = piece.Context;
        to.Z = board.BoardDef.PieceLayer;
        if (piece.CachedPosition.Equals(to))
            return true;
        
        if (to.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) == false)
        {
            return false;
        }

        var targetPositions = GetAllPiecePoints(piece, to);
        foreach (var position in targetPositions)
        {
            var pieceTo = board.BoardLogic.GetPieceAt(position);
            if (piece.Equals(pieceTo))
                continue;
			
            if (board.BoardLogic.IsLockedCell(position))
                return false;
            
            if (pieceTo != null)
            {
                var draggable = pieceTo.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);
                if (draggable == null || !draggable.IsDraggable(position) || IsLargeObject(pieceTo))
                    return false;
            }
        }
        return true;
    }
    
    private bool IsLargeObject(Piece piece)
    {
        var isMulticellular =
            piece.IsHasComponent(MulticellularPieceBoardObserver.ComponentGuid);
        return isMulticellular;
    }
    
    private List<BoardPosition> GetAllPiecePoints(Piece piece, BoardPosition shift)
    {
        shift.Z = piece.Context.BoardDef.PieceLayer;
        var multicellular =
            piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        if (multicellular != null)
        {
            var result = new List<BoardPosition>();
            for (int i = 0; i < multicellular.Mask.Count; i++)
            {
                var targetPos = multicellular.Mask[i] + shift;
                result.Add(targetPos);
            }

            return result;
        }

        return new List<BoardPosition> {shift};
    }
}