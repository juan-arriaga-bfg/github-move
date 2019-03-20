using System.Collections.Generic;

public class MatchActionBuilderComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardLogicComponent context;
    private IMatchActionBuilder defaultBuilder;
    
    private readonly Dictionary<int, IMatchActionBuilder> builders = new Dictionary<int, IMatchActionBuilder>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as BoardLogicComponent;
    }
    
    public MatchActionBuilderComponent RegisterDefaultBuilder(IMatchActionBuilder builder)
    {
        defaultBuilder = builder;
        return this;
    }

    public MatchActionBuilderComponent RegisterBuilder(IMatchActionBuilder builder)
    {
        var keys = builder.GetKeys();

        foreach (var key in keys)
        {
            if(builders.ContainsKey(key)) continue;
        
            builders.Add(key, builder);
        }
        
        return this;
    }

    public bool CheckMatch(List<BoardPosition> matchField, int pieceType, BoardPosition position, out int nextType)
    {
        if (builders.TryGetValue(pieceType, out var builder) == false)
        {
            builder = defaultBuilder;
        }
        
        return builder.Check(context.MatchDefinition, matchField, pieceType, position, out nextType);
    }
    
    public IBoardAction GetMatchAction(List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        if (builders.TryGetValue(pieceType, out var builder) == false)
        {
            builder = defaultBuilder;
        }
        
        return builder.Build(context.MatchDefinition, matchField, pieceType, position);
    }
}