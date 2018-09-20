using System.Collections.Generic;

public struct PartHint
{
    public int Weight;
    public BoardPosition Key;
    public List<BoardPosition> Cells;
}

public class CellHintsComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private BoardLogicComponent context;

    private const int OtherWeight = 10;
    private const int EmptyWeight = 100;
    private const int CurrentWeight = 1000;

    private int findId;
    private List<PartCellView> selectCells;

	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
    
    public void OnDragStart(BoardPosition boardPos, int pieceId)
    {
        findId = pieceId;
        
        var id = context.MatchDefinition.GetNext(findId, false);
        var pattern = context.MatchDefinition.GetPattern(id);
        var positions = context.PositionsCache.GetPiecePositionsByType(findId);
        
        var variants = new List<PartHint>();
        
        var hintCells = new List<BoardPosition>();
        var bestCells = new List<BoardPosition>();
        var ignoreCells = new List<BoardPosition>();
        
        selectCells = new List<PartCellView>();
        
        // search all variants
        foreach (var position in positions)
        {
            if (boardPos.Equals(position) || ignoreCells.Contains(position)) continue;

            var hints = new List<PartHint>();

            for (var i = 0; i < pattern.Count; i++)
            {
                var line = pattern[i];

                for (var j = 0; j < line.Count; j++)
                {
                    var weight = 0;
                    var list = GetHintList(pattern, context, new BoardPosition(position.X - i, position.Y - j, position.Z), out weight);

                    if (list == null) continue;

                    hints.Add(new PartHint {Key = position, Weight = weight, Cells = list});
                }
            }

            // search max count pieces variants
            if (hints.Count == 0) continue;

            hints.Sort((a, b) => -a.Weight.CompareTo(b.Weight));

            var max = hints[0].Weight;

            if (max < CurrentWeight * 2)
            {
                hintCells = AddCells(hintCells, hints[0].Cells);
                continue;
            }

            if (max == CurrentWeight * 4)
            {
                ignoreCells = AddCells(ignoreCells, hints[0].Cells);
                continue;
            }

            variants.AddRange(hints);
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
        
        // ignore copy variants
        foreach (var variant in variants)
        {
            if(variant.Weight < CurrentWeight * 3) continue;
            
            bestCells = AddCells(bestCells, variant.Cells);
        }

        for (var i = variants.Count - 1; i >= 0; i--)
        {
            var variant = variants[i];

            if (variant.Weight >= CurrentWeight * 3) continue;
            
            foreach (var cell in bestCells)
            {
                if(variant.Cells.Contains(cell) == false) continue;

                variants.RemoveAt(i);
                break;
            }
        }

        positions = positions.FindAll(position => !ignoreCells.Contains(position));
        
        // search best variants
        foreach (var position in positions)
        {
            var hints = variants.FindAll(hint => hint.Key.Equals(position));
            
            if(hints.Count == 0) continue;
            
            hints.Sort((a, b) => -a.Weight.CompareTo(b.Weight));
            hintCells = AddCells(hintCells, hints[0].Cells);
        }
        
        foreach (var point in hintCells)
        {
            var cell = context.Context.RendererContext.CreateBoardElementAt<PartCellView>(R.Cell, new BoardPosition(point.X, point.Y, 21));

            cell.ToggleSelection(true);
            selectCells.Add(cell);
        }
    }
    
    public void OnDragEnd()
    {
        foreach (var cell in selectCells)
        {
            cell.ToggleSelection(false);
            context.Context.RendererContext.DestroyElement(cell);
        }
        
        selectCells = null;
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
                    weight += EmptyWeight;
                    continue;
                }
                
                if (piece.Draggable == null) continue;

                if (piece.PieceType == findId)
                {
                    weight += CurrentWeight;
                    continue;
                }
                
                weight += OtherWeight;
            }
        }

        return positions;
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
}