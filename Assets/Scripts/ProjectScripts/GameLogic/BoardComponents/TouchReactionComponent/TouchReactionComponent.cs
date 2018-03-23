public class TouchReactionComponent : ECSEntity,
    ITouchReactionDefinitionComponent, ITouchReactionConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }
    
    private Piece context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as Piece;
    }
    
    protected TouchReactionDefinitionComponent reactionDefinition;
    public TouchReactionDefinitionComponent ReactionDefinition
    {
        get
        {
            if (reactionDefinition == null)
            {
                reactionDefinition = GetComponent<TouchReactionDefinitionComponent>(TouchReactionDefinitionComponent.ComponentGuid);
            }
            return reactionDefinition;
        }
    }
    
    protected TouchReactionConditionComponent reactionCondition;
    public TouchReactionConditionComponent ReactionCondition
    {
        get
        {
            if (reactionCondition == null)
            {
                reactionCondition = GetComponent<TouchReactionConditionComponent>(TouchReactionConditionComponent.ComponentGuid);
            }
            return reactionCondition;
        }
    }
    
    public bool Touch(BoardPosition position)
    {
        if (ReactionCondition == null || ReactionDefinition == null) return false;
        
        var worldPos = context.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        context.Context.Manipulator.CameraManipulator.MoveTo(worldPos);
        
        if (ReactionCondition.Check(position, context) == false) return false;
        
        ReactionCondition.Recharge();
        
        return ReactionDefinition.Make(position, context);
    }
}