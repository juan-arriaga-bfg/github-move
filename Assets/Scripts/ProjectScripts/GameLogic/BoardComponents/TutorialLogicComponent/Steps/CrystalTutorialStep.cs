using System.Collections.Generic;

public class CrystalTutorialStep : LoopFingerTutorialStep
{
    private int crystal = PieceType.Boost_CR.Id;
    private int target = PieceType.A5.Id;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = crystal,
            Amount = 1
        }, true);
        
        RegisterComponent(new CheckPieceInPositionTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = target,
            CheckAbsence = true,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(20, 8, BoardLayer.Piece.Layer),
                new BoardPosition(19, 8, BoardLayer.Piece.Layer)
            }
        }, true);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = crystal,
            Amount = 0
        }, true);
    }
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        startTime = startTime.AddSeconds(-(Delay-0.5f));
    }
    
    public override void Execute()
    {
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
        
        if (positions == null)
        {
            PauseOff();
            return;
        }
        
        from = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(crystal)[0];
        
        var options = new List<List<BoardPosition>>();
        
        foreach (var position in positions)
        {
            int amount;
            var field = new List<BoardPosition>();
            
            if(Context.Context.BoardLogic.FieldFinder.Find(position, field, out amount) == false) continue;
            
            options.Add(field);
        }
        
        if(options.Count == 0)
        {
            PauseOff();
            return;
        }
        
        options.Sort((a, b) => -a.Count.CompareTo(b.Count));

        var max = options[0].Count;
        var immediate = new List<BoardPosition>();

        options = options.FindAll(list => list.Count == max);

        foreach (var option in options)
        {
            immediate.AddRange(from.GetImmediate(option));
        }
        
        to = from.GetImmediate(immediate)[0];
        
        base.Execute();
    }
}