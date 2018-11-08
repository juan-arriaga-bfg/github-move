using System.Collections.Generic;

public class SwapHardTutorialStep : BaseTutorialStep
{
    public int FromType;
    public int ToType;
    
    public BoardPosition FromPosition;
    public BoardPosition ToPosition;

    private TutorialMergeFinger finger;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceInPositionTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = FromType,
            Positions = new List<BoardPosition>{FromPosition},
            CheckAll = true
        }, true);
        
        RegisterComponent(new CheckPieceInPositionTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = ToType,
            Positions = new List<BoardPosition>{ToPosition},
            CheckAll = true
        }, true);
    }

    public override void PauseOn()
    {
        if(finger != null) finger.Hide();
    }

    public override void PauseOff()
    {
        if(finger != null) finger.Show();
    }
    
    public override void Perform()
    {
        if(isPerform) return;
        
        Context.LockAll();

        isPerform = true;
        
        Context.UnlockCell(FromPosition);
        Context.UnlockCell(ToPosition);
        
        finger = Context.Context.RendererContext.CreateBoardElement<TutorialMergeFinger>((int) ViewType.TutorialMergeFinger);
        finger.Init(Context.Context.RendererContext, FromPosition, ToPosition);
    }

    protected override void Complete()
    {
        if(finger == null) return;
        
        Context.UnlockAll();
        Context.Context.RendererContext.DestroyElement(finger.gameObject);
    }
}