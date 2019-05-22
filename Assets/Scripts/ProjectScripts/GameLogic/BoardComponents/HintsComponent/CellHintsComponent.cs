using System;
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
    private const int BoosterWeight = 1000;
    private const int CurrentWeight = 10000;

    private List<BoardElementView> selectCells;
    
    private readonly List<int> ids = new List<int>();

	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
		
        var types = PieceType.GetIdsByFilter(PieceTypeFilter.Workplace | PieceTypeFilter.Multicellular, PieceTypeFilter.Mine);
        
        foreach (var id in types)
        {
            ids.Add(context.MatchDefinition.GetPrevious(id));
        }
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

    public bool OnDragStartBoost(BoardPosition boardPos)
    {
        selectCells = new List<BoardElementView>();
        
        foreach (var id in ids)
        {
            if (GameDataService.Current.CodexManager.IsPieceUnlocked(id) == false) continue;
            
            FindVariants(boardPos, id, true);
        }

        return selectCells.Count > 0;
    }
	
    public void OnDragStart(BoardPosition boardPos, int pieceId)
    {
        selectCells = new List<BoardElementView>();
        
        FindVariants(boardPos, pieceId, false);
    }
    
    public void OnDragEnd()
    {
        if (selectCells == null) return;
        
        foreach (var cell in selectCells)
        {
            cell.SetCustomMaterial(BoardElementMaterialType.PiecesLowHighlightMaterial, false);
            context.Context.RendererContext.DestroyElement(cell);
        }
        
        selectCells = null;
    }
    
    public int GetLayerIndexBy(BoardPosition position)
    {
        return BoardLayer.GetDefaultLayerIndexBy(position, context.Context.BoardDef.Width, context.Context.BoardDef.Height);
    }

    private void FindVariants(BoardPosition boardPos, int pieceId, bool isMax)
    {
        var id = context.MatchDefinition.GetNext(pieceId, false);
        var pattern = context.MatchDefinition.GetPattern(id);
        var positions = context.PositionsCache.GetPiecePositionsByType(pieceId);
        
        var variants = new List<PartHint>();
        var hintCells = new List<BoardPosition>();
        var hintBigCells = new List<BoardPosition>();
        
        positions.Sort((a, b) => GetLayerIndexBy(a).CompareTo(GetLayerIndexBy(b)));
        
        // search all variants
        foreach (var position in positions)
        {
            if (boardPos.Equals(position)) continue;

            var hints = new List<PartHint>();

            for (var i = 0; i < pattern.Count; i++)
            {
                var line = pattern[i];

                for (var j = 0; j < line.Count; j++)
                {
                    var list = GetHintList(pieceId, pattern, context, boardPos, new BoardPosition(position.X - i, position.Y - j, position.Z), out var weight);

                    if (list == null || isMax && weight < CurrentWeight * 3) continue;
                    
                    hints.Add(new PartHint {Key = position, Weight = weight, Cells = list});
                }
            }

            // search max count pieces variants
            if (hints.Count == 0) continue;
            
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
                    if (variants[j].Cells.Contains(cell) == false) continue;

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

            if (hints.Count == 0) continue;
            
            hints.Sort((a, b) => -a.Weight.CompareTo(b.Weight));

            if (hints[0].Weight >= (CurrentWeight * 3 + BoosterWeight) && hints[0].Cells.Contains(boardPos) == false) continue;

            var cells = hints[0].Cells;
            
            hintCells = AddCells(hintCells, cells);
            hintBigCells = AddBigCells(hintBigCells, cells);
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

    private List<BoardPosition> AddBigCells(List<BoardPosition> cells, List<BoardPosition> hint)
    {
        hint.Sort((a, b) => a.X.CompareTo(b.X));
        
        var cell = hint[0];
        
        foreach (var item in hint)
        {
            if(item.X != cell.X) break;
            if(item.Y >= cell.Y) continue;
            
            cell = item;
        }
        
        if(cells.Contains(cell) == false) cells.Add(cell);
        
        return cells;
    }
    
    private List<BoardPosition> GetHintList(int id, List<List<int>> pattern, BoardLogicComponent logic, BoardPosition target, BoardPosition start, out int weight)
    {
        var ignore = new List<Type> {typeof(DragAndCheckMatchAction)};
        weight = 0;
        
        if(logic.IsLockedCell(start, ignore) || logic.IsPointValid(start) == false) return null;
        
        var positions = new List<BoardPosition>();
            
        for (var i = 0; i < pattern.Count; i++)
        {
            var line = pattern[i];
            
            for (var j = 0; j < line.Count; j++)
            {
                var pos = new BoardPosition(start.X + i, start.Y + j, start.Z);
                
                if(logic.IsLockedCell(pos, ignore) || logic.IsPointValid(pos) == false) return null;
                
                positions.Add(pos);
                
                var piece = logic.GetPieceAt(pos);
                
                if (piece == null)
                {
                    weight += EmptyWeight;
                    continue;
                }
                
                if (piece.PieceType == PieceType.Fog.Id || PieceType.GetDefById(piece.PieceType).Filter.Has(PieceTypeFilter.Multicellular)) return null;
                
                if (piece.Draggable == null) continue;

                if (piece.PieceType == PieceType.Boost_CR.Id)
                {
                    weight += BoosterWeight;
                    continue;
                }
                
                if (piece.PieceType == id)
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