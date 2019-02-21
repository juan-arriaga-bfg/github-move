using System;
using UnityEngine;

public class TouchReactionComponent : ECSEntity,
    ITouchReactionDefinitionComponent, ITouchReactionConditionComponent, ILockerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private LockerComponent locker;
    public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
    
    private Piece context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        RegisterComponent(new LockerComponent());
    }

    private TouchReactionDefinitionComponent reactionDefinition;
    public TouchReactionDefinitionComponent ReactionDefinition => reactionDefinition ?? (reactionDefinition = GetComponent<TouchReactionDefinitionComponent>(TouchReactionDefinitionComponent.ComponentGuid));

    private TouchReactionConditionComponent reactionCondition;
    public TouchReactionConditionComponent ReactionCondition => reactionCondition ?? (reactionCondition = GetComponent<TouchReactionConditionComponent>(TouchReactionConditionComponent.ComponentGuid));

    public bool Touch(BoardPosition position)
    {
        if (Locker.IsLocked || ReactionCondition == null || ReactionDefinition == null) return false;
        
        if (ReactionCondition.Check(position, context) == false) return false;
        
        ReactionCondition.Recharge();

        if (context.TutorialLocker == null) return ReactionDefinition.Make(position, context);
        
        context.TutorialLocker.Touch();
        return true;
    }
}