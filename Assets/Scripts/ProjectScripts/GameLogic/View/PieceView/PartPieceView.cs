using System.Collections.Generic;
using UnityEngine;

public struct PartHint
{
    public int Weight;
    public BoardPosition Key;
    public List<BoardPosition> Cells;
}

public class PartPieceView : BuildingPieceView
{
    private List<PartCellView> selectCells;

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        
        var logic = Piece.Context.BoardLogic;
        var id = logic.MatchDefinition.GetNext(Piece.PieceType, false);
        var pattern = logic.MatchDefinition.GetPattern(id);
        var positions = logic.PositionsCache.GetPiecePositionsByType(Piece.PieceType);
        
        var variants = new List<PartHint>();
        
        var hintCells = new List<BoardPosition>();
        var ignoreCells = new List<BoardPosition>{Piece.CachedPosition};
        var bestCells = new List<BoardPosition>();
        
        selectCells = new List<PartCellView>();
        
        // search all variants
        foreach (var position in positions)
        {
            if(position.Equals(Piece.CachedPosition) || ignoreCells.Contains(position)) continue;

            var hints = new List<PartHint>();
            
            for (var i = 0; i < pattern.Count; i++)
            {
                var line = pattern[i];
            
                for (var j = 0; j < line.Count; j++)
                {
                    var weight = 0;
                    var list = GetHintList(pattern, logic, new BoardPosition(position.X - i, position.Y - j, position.Z), out weight);
                    
                    if(list == null) continue;

                    hints.Add(new PartHint {Key = position, Weight = weight, Cells = list});
                }
            }
            
            // search max count pieces variants

            if (hints.Count == 0) continue;
            
            hints.Sort((a, b) => -a.Weight.CompareTo(b.Weight));
            
            var max = hints[0].Weight;
            
            if (max < 2000)
            {
                hintCells = AddCells(hintCells, hints[0].Cells);
                
                continue;
            }
            
            if (max == 4000)
            {
                ignoreCells = AddCells(ignoreCells, hints[0].Cells);
                continue;
            }
            
            variants.AddRange(hints.FindAll(hint => hint.Weight == max));
        }
        
        // ignore ready pieces
        foreach (var ignore in ignoreCells)
        {
            for (var i = variants.Count - 1; i >= 0; i--)
            {
                if (variants[i].Cells.Contains(ignore) == false) continue;
                
                variants.RemoveAt(i);
            }
        }
        
        // search best variants
        foreach (var variant in variants)
        {
            if(variant.Weight < 3000) continue;
            
            bestCells = AddCells(bestCells, variant.Cells);
        }

        foreach (var variant in variants)
        {
            if (variant.Weight < 3000)
            {
                var isIgnore = false;
                
                foreach (var cell in bestCells)
                {
                    if(variant.Cells.Contains(cell) == false) continue;

                    isIgnore = true;
                    break;
                }
                
                if(isIgnore) continue;
            }
            
            hintCells = AddCells(hintCells, variant.Cells);
        }
        
        foreach (var point in hintCells)
        {
            var cell = Context.CreateBoardElementAt<PartCellView>(R.Cell, new BoardPosition(point.X, point.Y, 21));

            cell.ToggleSelection(true);
            selectCells.Add(cell);
        }
    }

    private List<BoardPosition> AddCells(List<BoardPosition> cells, List<BoardPosition> hint)
    {
        foreach (var point in hint)
        {
            if(cells.Contains(point)) continue;
                    
            cells.Add(point);
        }

        return cells;
    }
    
    private List<BoardPosition> GetHintList(List<List<int>> pattern, BoardLogicComponent logic, BoardPosition start, out int weight)
    {
        weight = 0;
        
        if(logic.IsLockedCell(start) || logic.IsPointValid(start) == false) return null;
        
        var positions = new List<BoardPosition>();
            
        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];
            
            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(start.X + i, start.Y + j, start.Z);
                
                if(logic.IsLockedCell(pos) || logic.IsPointValid(pos) == false) return null;
                
                positions.Add(pos);
                
                var piece = logic.GetPieceAt(pos);

                if (piece == null)
                {
                    weight += 100;
                    continue;
                }
                
                if (piece.Draggable == null) continue;

                if (piece.PieceType == Piece.PieceType)
                {
                    weight += 1000;
                    continue;
                }
                
                weight += 10;
            }
        }

        return positions;
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