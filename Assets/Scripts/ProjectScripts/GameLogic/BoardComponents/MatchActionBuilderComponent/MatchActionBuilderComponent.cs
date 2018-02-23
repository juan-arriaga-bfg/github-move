using System.Collections.Generic;

public class MatchActionBuilderComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    private BoardLogicComponent context;
    private IMatchActionBuilder defaultBuilder;
    
    private Dictionary<int, IMatchActionBuilder> builders = new Dictionary<int, IMatchActionBuilder>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as BoardLogicComponent;
    }
    
    public MatchActionBuilderComponent RegisterDefaultBuilder(IMatchActionBuilder builder)
    {
        defaultBuilder = builder;
        return this;
    }

    public MatchActionBuilderComponent RegisterBuilder(int pieceType, IMatchActionBuilder builder)
    {
        if(builders.ContainsKey(pieceType)) return this;
        
        builders.Add(pieceType, builder);
        return this;
    }
    
    public IBoardAction GetMatchAction(List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        IMatchActionBuilder builder;

        if (builders.TryGetValue(pieceType, out builder) == false)
        {
            builder = defaultBuilder;
        }
        
        return builder.Build(context.MatchDefinition, matchField, pieceType, position);
    }
}