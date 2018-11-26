using System.Collections.Generic;

public class СrystalTutorialStep : LoopFingerTutorialStep
{
    private int crystal = PieceType.Boost_CR.Id;
    private int target = PieceType.A4.Id;
    
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
                new BoardPosition(20, 8, Context.Context.BoardDef.PieceLayer),
                new BoardPosition(19, 8, Context.Context.BoardDef.PieceLayer)
            }
        }, true);
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