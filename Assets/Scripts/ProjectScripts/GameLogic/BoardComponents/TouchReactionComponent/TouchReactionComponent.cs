public class TouchReactionComponent : ECSEntity,
    ITouchReactionDefinitionComponent, ITouchReactionConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private Piece context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
    }
    
    protected TouchReactionDefinitionComponent reactionDefinition;
    public TouchReactionDefinitionComponent ReactionDefinition => reactionDefinition ?? (reactionDefinition = GetComponent<TouchReactionDefinitionComponent>(TouchReactionDefinitionComponent.ComponentGuid));

    protected TouchReactionConditionComponent reactionCondition;
    public TouchReactionConditionComponent ReactionCondition => reactionCondition ?? (reactionCondition = GetComponent<TouchReactionConditionComponent>(TouchReactionConditionComponent.ComponentGuid));

    public bool Touch(BoardPosition position)
    {
        if (ReactionCondition == null || ReactionDefinition == null) return false;
        
        if (ReactionCondition.Check(position, context) == false) return false;
        
        ReactionCondition.Recharge();
        
        return ReactionDefinition.Make(position, context);
    }
}