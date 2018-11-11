using System.Collections.Generic;

public class MatchHardTutorialStep : BaseTutorialStep
{
    public int FromType;
    public int ToType;
    
    public List<BoardPosition> FromPositions;
    public List<BoardPosition> ToPositions;

    private TutorialMergeFinger finger;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceInPositionTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = FromType,
            Positions = FromPositions
        }, true);

        RegisterComponent(new CheckPieceInPositionTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = FromType,
            Positions = ToPositions,
            CheckAll = true
        }, true);
        
        RegisterComponent(new CheckPieceInPositionTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = ToType,
            Positions = ToPositions
        }, true);
    }

    public override void PauseOn()
    {
        if(finger != null) finger.Hide();
    }

    public override void PauseOff()
    {
        base.PauseOff();
        
        if(finger != null) finger.Show();
    }
    
    public override void Perform()
    {
        if(IsPerform) return;
        
        base.Perform();
        
        Context.LockAll();
        
        var from = FromPositions.Find(position =>
        {
            var piece = Context.Context.BoardLogic.GetPieceAt(position);
            return piece != null && piece.PieceType == FromType;
        });
        
        Context.UnlockCell(from);
        Context.UnlockCells(ToPositions);
        
        finger = Context.Context.RendererContext.CreateBoardElement<TutorialMergeFinger>((int) ViewType.TutorialMergeFinger);
        finger.Init(Context.Context.RendererContext, from, ToPositions[0]);
    }

    protected override void Complete()
    {
        if(finger == null) return;
        
        Context.UnlockAll();
        Context.Context.RendererContext.DestroyElement(finger.gameObject);
    }
}