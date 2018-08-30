using System.Collections.Generic;

public class MatchDefinitionComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private readonly Dictionary<int, PieceMatchDef> definition;

    public BoardLogicComponent Context;
    
    public MatchDefinitionComponent(Dictionary<int, PieceMatchDef> def)
    {
        definition = def;
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Context = entity as BoardLogicComponent;
    }

    public int GetPieceCountForMatch(int pieceId)
    {
        PieceMatchDef def;
        
        return definition.TryGetValue(pieceId, out def) ? def.Count : -1;
    }

    public List<List<int>> GetPattern(int pieceId)
    {
        PieceMatchDef def;
        
        return definition.TryGetValue(pieceId, out def) ? def.Pattern : new List<List<int>>();
    }
    
    public int GetNext(int pieceId, bool checkIgnore = true)
    {
        PieceMatchDef def;
        
        if(definition.TryGetValue(pieceId, out def) == false) return PieceType.None.Id;
        
        if(checkIgnore && def.IsIgnore) return PieceType.None.Id;
        
        return def.Next;
    }

    public int GetPrevious(int pieceId)
    {
        PieceMatchDef def;
        
        return definition.TryGetValue(pieceId, out def) ? def.Previous : PieceType.None.Id;
    }

    public int GetFirst(int pieceId)
    {
        var chain = GetChain(pieceId);
        return chain.Count == 0 ? PieceType.None.Id : chain[0];
    }

    public int GetLast(int pieceId, bool checkIgnore = true)
    {
        var chain = GetChain(pieceId, checkIgnore);
        return chain.Count == 0 ? PieceType.None.Id : chain[chain.Count - 1];
    }
    
    public int GetIndexInChain(int pieceId)
    {
        var chain = GetChain(pieceId);
        
        if(chain.Count == 0) return -1;
        
        return chain.IndexOf(pieceId) + 1;
    }

    public List<int> GetChain(int pieceId, bool checkIgnore = true)
    {
        var chain = new List<int>();

        if (pieceId == PieceType.None.Id) return chain;
        
        var unit = pieceId;
        
        do
        {
            chain.Insert(0, unit);
            unit = GetPrevious(unit);
        }
        while (unit != PieceType.None.Id);
        
        unit = GetNext(pieceId, checkIgnore);
        
        while (unit != PieceType.None.Id)
        {
            chain.Add(unit);
            unit = GetNext(unit, checkIgnore);
        }
        
        return chain;
    }
}