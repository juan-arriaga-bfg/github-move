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
    private BoardPosition target;
    private List<BoardElementView> selectCells;

	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
    
    public void OnDragStart(BoardPosition boardPos, int pieceId)
    {
        target = boardPos;
        findId = pieceId;
        
        var id = context.MatchDefinition.GetNext(findId, false);
        var pattern = context.MatchDefinition.GetPattern(id);
        var positions = context.PositionsCache.GetPiecePositionsByType(findId);
        
        var variants = new List<PartHint>();
        var hintCells = new List<BoardPosition>();
        var hintBigCells = new List<BoardPosition>();
        
        selectCells = new List<BoardElementView>();
        
        // search all variants
        foreach (var position in positions)
        {
            if (target.Equals(position)) continue;

            var hints = new List<PartHint>();

            for (var i = 0; i < pattern.Count; i++)
            {
                var line = pattern[i];

                for (var j = 0; j < line.Count; j++)
                {
                    var list = GetHintList(pattern, context, new BoardPosition(position.X - i, position.Y - j, position.Z), out var weight);

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
            
            variants.AddRange(hints);
        }
        
        variants.Sort((a, b) => -a.Weight.CompareTo(b.Weight));
        
        // search best variants
        for (var i = 0; i < variants.Count; i++)
        {
            var cells = variants[i].Cells;
            
            for (var j = i + 1; j < variants.Count; j++)
            {
                foreach (var cell in cells)
                {
                    if(variants[j].Cells.Contains(cell) == false) continue;

                    variants.RemoveAt(j);
                    j--;
                    break;
                }
            }
        }
        
        // search all cells
        foreach (var position in positions)
        {
            var hints = variants.FindAll(hint => hint.Key.Equals(position));
            
            if(hints.Count == 0) continue;
            
            hints.Sort((a, b) => -a.Weight.CompareTo(b.Weight));
            
            if(hints[0].Weight == CurrentWeight * 4 && hints[0].Cells.Contains(target) == false) continue;

            var cells = hints[0].Cells;
            
            cells.Sort((a, b) => a.X.CompareTo(b.X));
            
            var cell = cells[0];

            foreach (var item in cells)
            {
                if(item.X != cell.X) break;
                
                if(item.Y >= cell.Y) continue;
                
                cell = item;
            }
            
            hintCells = AddCells(hintCells, cells);
            
            if(hintBigCells.Contains(cell) == false) hintBigCells.Add(cell);
        }
        
        // show
        foreach (var point in hintCells)
        {
            InitCell(R.Cell, new BoardPosition(point.X, point.Y, BoardLayer.Default.Layer));
        }
        
        foreach (var point in hintBigCells)
        {
            InitCell(R.BigCell, new BoardPosition(point.X, point.Y, BoardLayer.MAX.Layer));
        }
    }
    
    public void OnDragEnd()
    {
        if(selectCells == null) return;
        
        foreach (var cell in selectCells)
        {
            cell.SetCustomMaterial(BoardElementMaterialType.PiecesLowHighlightMaterial, false);
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

                if (piece.PieceType == PieceType.Fog.Id || PieceType.GetDefById(piece.PieceType).Filter.Has(PieceTypeFilter.Multicellular)) return null;
                
                if (piece.Draggable == null) continue;

                if (piece.PieceType == findId)
                {
                    weight += CurrentWeight;

                    if (target.Equals(pos)) weight -= OtherWeight;
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

    private void InitCell(string id, BoardPosition point)
    {
        var cell = context.Context.RendererContext.CreateBoardElementAt<BoardElementView>(id, point);
        
        cell.SetCustomMaterial(BoardElementMaterialType.PiecesLowHighlightMaterial, true);
        selectCells.Add(cell);
    }
}