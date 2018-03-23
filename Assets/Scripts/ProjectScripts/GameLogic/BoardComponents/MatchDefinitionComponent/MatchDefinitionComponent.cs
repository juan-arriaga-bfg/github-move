using System.Collections.Generic;

public class MatchDefinitionComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    private Dictionary<int, PieceMatchDef> definition;

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
    
    public int GetNext(int pieceId)
    {
        PieceMatchDef def;
        
        return definition.TryGetValue(pieceId, out def) ? def.Next : PieceType.None.Id;
    }

    public int GetPrevious(int pieceId)
    {
        PieceMatchDef def;
        
        return definition.TryGetValue(pieceId, out def) ? def.Previous : PieceType.None.Id;
    }

    public List<int> GetChain(int pieceId)
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
        
        unit = GetNext(pieceId);
        
        while (unit != PieceType.None.Id)
        {
            chain.Add(unit);
            unit = GetNext(unit);
        }
        
        return chain;
    }
}