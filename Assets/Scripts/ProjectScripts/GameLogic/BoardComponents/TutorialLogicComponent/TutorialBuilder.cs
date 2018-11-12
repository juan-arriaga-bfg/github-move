using System.Collections.Generic;
using Quests;

public static class TutorialBuilder
{
    public static BaseTutorialStep BuildTutorial(int index, BoardController context)
    {
        BaseTutorialStep step;
        
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
                        new BoardPosition(22, 10, context.BoardDef.PieceLayer),
                        new BoardPosition(23, 9, context.BoardDef.PieceLayer)
                    }
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
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
                        new BoardPosition(23, 10, context.BoardDef.PieceLayer),
                        new BoardPosition(22, 10, context.BoardDef.PieceLayer),
                        new BoardPosition(23, 9, context.BoardDef.PieceLayer)
                    },
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(24, 11, context.BoardDef.PieceLayer),
                        new BoardPosition(24, 12, context.BoardDef.PieceLayer)
                    }
                };
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                
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
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 1, ConditionType = TutorialConditionType.Start}, true);
                
                break;
            }
            case 3:
            {
                step = new HighlightPiecesTutorialStep {Delay = 3, Targets = new List<int>{PieceType.PR_C1.Id}};
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 2, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCounterTutorialCondition{Target = 1, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 4:
            {
                step = new HighlightPiecesTutorialStep {Delay = 5, Targets = new List<int>{PieceType.PR_C1.Id, PieceType.PR_C2.Id, PieceType.PR_C3.Id}};
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 3, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 5:
            {
                step = new HighlightFogTutorialStep {Delay = 5};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "3", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                
                break;
            }
            case 6:
            {
                step = new SelectObstacleTutorialStep {Delay = 5, Targets = new List<int>{PieceType.OB1_A.Id}};
                
                step.RegisterComponent(new ChechStepTutorialCondition{Target = 5, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition{Target = -1, Currensy = Currency.Worker.Name, ConditionType = TutorialConditionType.Complete}, true);
                
                break;
            }
            case 7:
            {
                step = new HighlightPiecesTutorialStep {Delay = 5, Targets = new List<int>{PieceType.A1.Id}};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "6", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            default:
                return null;
        }
        
        step.Id = index;
        
        return step;
    }
}