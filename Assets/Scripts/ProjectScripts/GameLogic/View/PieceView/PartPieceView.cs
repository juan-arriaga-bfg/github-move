using System.Collections.Generic;
using UnityEngine;

public class PartPieceView : BuildingPieceView
{
    private List<PartCellView> selectCells;
    
    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        
        var positions = Piece.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(Piece.PieceType);
        var fields = new List<List<BoardPosition>>();
        var max = int.MinValue;
        var id = PieceType.None.Id;
        
        selectCells = new List<PartCellView>();
        
        foreach (var position in positions)
        {
            if(position.Equals(Piece.CachedPosition) || fields.Find(list => list.Contains(position)) != null) continue;
            
            var field = new List<BoardPosition>();
            
            if(Piece.Context.BoardLogic.FieldFinder.Find(position, field, out id) == false || field.Count < max) continue;

            max = Mathf.Max(max, field.Count);
            
            fields.Add(field);
            fields = fields.FindAll(list => list.Count == max);
        }
        
        id = Piece.Context.BoardLogic.MatchDefinition.GetNext(id, false);
        
        var pattern = Piece.Context.BoardLogic.MatchDefinition.GetPattern(id);
        
        foreach (var field in fields)
        {
            foreach (var point in field)
            {
                var cell = Context.CreateBoardElementAt<PartCellView>(R.Cell, new BoardPosition(point.X, point.Y, 21));
            
                cell.ToggleSelection(true);
                selectCells.Add(cell);
            }
        }
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);

        foreach (var cell in selectCells)
        {
            cell.ToggleSelection(false);
            Context.DestroyElement(cell);
        }
        
        selectCells = null;
    }
}