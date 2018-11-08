using System.Collections.Generic;

public static class TutorialBuilder
{
    public static int Amount = 3;
    
    public static BaseTutorialStep BuildTutorial(int index, BoardController context)
    {
        BaseTutorialStep step = null;
        
        switch (index)
        {
            case 0:
            {
                step = new MatchHardTutorialStep
                {
                    FromType = PieceType.PR_C1.Id,
                    ToType = PieceType.PR_C2.Id,
                    FromPositions = new List<BoardPosition> {new BoardPosition(23, 9, context.BoardDef.PieceLayer)},
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(23, 10, context.BoardDef.PieceLayer),
                        new BoardPosition(22, 10, context.BoardDef.PieceLayer)
                    }
                };
                break;
            }
            case 1:
            {
                step = new MatchHardTutorialStep
                {
                    FromType = PieceType.PR_C2.Id,
                    ToType = PieceType.PR_C3.Id,
                    FromPositions = new List<BoardPosition>
                    {
                        new BoardPosition(22, 10, context.BoardDef.PieceLayer),
                        new BoardPosition(23, 10, context.BoardDef.PieceLayer)
                    },
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(24, 11, context.BoardDef.PieceLayer),
                        new BoardPosition(24, 12, context.BoardDef.PieceLayer)
                    }
                };
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 0}, true);
                
                break;
            }
            case 2:
            {
                step = new SwapHardTutorialStep
                {
                    FromType = PieceType.PR_C1.Id,
                    ToType = PieceType.PR_C1.Id,
                    FromPosition = new BoardPosition(20, 11, context.BoardDef.PieceLayer),
                    ToPosition = new BoardPosition(20, 12, context.BoardDef.PieceLayer)
                };
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 1}, true);
                
                break;
            }
        }
        
        if (step != null) step.Id = index;
        
        return step;
    }
}