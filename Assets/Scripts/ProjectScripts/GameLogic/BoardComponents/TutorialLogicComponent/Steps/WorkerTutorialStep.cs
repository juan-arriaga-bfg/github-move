public class WorkerTutorialStep : DelayTutorialStep
{
    private TutorialMergeFinger finger;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = PieceType.Boost_WR.Id,
            Amount = 1
        }, true);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = PieceType.Boost_WR.Id,
            Amount = 0
        }, true);
    }
    
    public override void PauseOn()
    {
        if(finger == null) return;
        
        base.PauseOn();
        
        Context.Context.RendererContext.DestroyElement(finger.gameObject);
        finger = null;
    }

    public override void Execute()
    {
        base.Execute();

        var from = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.Boost_WR.Id)[0];
        var target = Context.Context.BoardLogic.PositionsCache.GetNearestByFilter(PieceTypeFilter.WorkPlace, from);
        
        if(target == null) return;
        
        finger = Context.Context.RendererContext.CreateBoardElement<TutorialMergeFinger>((int) ViewType.TutorialMergeFinger);
        finger.Init(Context.Context.RendererContext, from, target.Value);
    }
    
    protected override void Complete()
    {
        if(finger == null) return;
        
        base.Complete();
        
        Context.Context.RendererContext.DestroyElement(finger.gameObject);
    }
    
    public override bool IsExecuteable()
    {
        return finger == null && base.IsExecuteable();
    }
}