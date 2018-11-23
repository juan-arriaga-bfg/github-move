﻿using System.Collections.Generic;
using Quests;
using UnityEngine;

public static class TutorialBuilder
{
    public const int LockPRStepIndex = 10;
    
    public static BaseTutorialStep BuildTutorial(int index, BoardController context)
    {
        BaseTutorialStep step;
        
        switch (index)
        {
            case 0: // tutorial 1 step 1
            {
                step = new MatchHardTutorialStep
                {
                    FromType = PieceType.PR_C1.Id,
                    ToType = PieceType.PR_C2.Id,
                    FromPositions = new List<BoardPosition> {new BoardPosition(20, 11, context.BoardDef.PieceLayer)},
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(20, 12, context.BoardDef.PieceLayer),
                        new BoardPosition(21, 12, context.BoardDef.PieceLayer),
                        new BoardPosition(20, 11, context.BoardDef.PieceLayer)
                    }
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                break;
            }
            case 1: // tutorial 1 step 2
            {
                step = new MatchHardTutorialStep
                {
                    FromType = PieceType.PR_C2.Id,
                    ToType = PieceType.PR_C3.Id,
                    FromPositions = new List<BoardPosition>
                    {
                        new BoardPosition(20, 12, context.BoardDef.PieceLayer),
                        new BoardPosition(21, 12, context.BoardDef.PieceLayer),
                        new BoardPosition(20, 11, context.BoardDef.PieceLayer)
                    },
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(22, 13, context.BoardDef.PieceLayer),
                        new BoardPosition(22, 14, context.BoardDef.PieceLayer)
                    }
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                
                break;
            }
            /*case 2: // tutorial 1 step 3
            {
                step = new SwapHardTutorialStep
                {
                    FromType = PieceType.PR_C1.Id,
                    ToType = PieceType.PR_C1.Id,
                    FromPosition = new BoardPosition(18, 13, context.BoardDef.PieceLayer),
                    ToPosition = new BoardPosition(18, 14, context.BoardDef.PieceLayer)
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition{Target = 1, ConditionType = TutorialConditionType.Start}, true);
                
                break;
            }*/
            case 2: // tutorial 2 step 1
            {
                step = new HighlightPiecesTutorialStep {Delay = 3, Targets = new List<int>{PieceType.PR_C1.Id}};
                
                step.RegisterComponent(new CheckStepTutorialCondition{Target = 1, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCounterTutorialCondition{Target = 1, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2_ClearFog", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 3: // tutorial 2 step 2
            {
                step = new HighlightPiecesTutorialStep {Delay = 5, Targets = new List<int>{PieceType.PR_C1.Id, PieceType.PR_C2.Id, PieceType.PR_C3.Id}};
                
                step.RegisterComponent(new CheckStepTutorialCondition{Target = 2, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2_ClearFog", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 4: // tutorial 3 - clear fog
            {
                step = new HighlightFogTutorialStep {Delay = 5};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2_ClearFog", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2_ClearFog", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "2_ClearFog", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 5: // tutorial 4 - remove tree
            {
                step = new SelectStorageTutorialStep {Delay = 5, Targets = new List<int>{PieceType.OB1_TT.Id, PieceType.OB2_TT.Id}};
                
                step.RegisterComponent(new CheckStepTutorialCondition{Target = 4, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition{Target = -1, Currensy = Currency.Worker.Name, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "3_KillTree", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 6: // tutorial 5 step 1
            {
                step = new HighlightPiecesTutorialStep {Delay = 5, Targets = new List<int>{PieceType.A1.Id}};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 7: // tutorial 6 - create ingredient
            {
                step = new SelectStorageTutorialStep {Delay = 5, Targets = new List<int>{PieceType.PR_A4.Id, PieceType.PR_B4.Id, PieceType.PR_C4.Id, PieceType.PR_D4.Id, PieceType.PR_E4.Id}};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "7_CreatePiece_PR_A5", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition{Target = -1, Currensy = Currency.Worker.Name, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "7_CreatePiece_PR_A5", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 8: // tutorial 7 - worker
            {
                step = new WorkerTutorialStep();
                break;
            }
            case 9: // remove worker
            {
                step = new BaseTutorialStep{IsIgnoreUi = true};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                
                step.RegisterComponent(new RemoveTutorialAnimation{PieceId = PieceType.NPC_Gnome.Id, AnimationType = TutorialAnimationType.Perform}, true);
                
                break;
            }
            case 10: // unlock PR pieces
            {
                if (LockPRStepIndex != index) Debug.LogError("Tutorial Error: LockPRStepIndex != index");
                
                step = new BaseTutorialStep();
                
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "7_CreatePiece_PR_A5", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "7_CreatePiece_PR_A5", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 11: // lock camera
            {
                step = new CameraLockTutorialStep();
                
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 12: // lock buttons Worker, Energy, Codex
            {
                step = new UiLockTutorialStep{Targets = new List<UiLockTutorialItem>{UiLockTutorialItem.Worker, UiLockTutorialItem.Energy, UiLockTutorialItem.Codex}};
                
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 13: // lock Orders and buttons Orders, Shop
            {
                step = new OrdersLockTutorialStep{Targets = new List<UiLockTutorialItem>{UiLockTutorialItem.Orders, UiLockTutorialItem.Shop}};
                
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "16_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "16_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 14: // add SleepingBeauty
            {
                step = new BaseTutorialStep{IsIgnoreUi = true};
                
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition{Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Hard}, true);
                
                step.RegisterComponent(new RemoveTutorialAnimation{PieceId = PieceType.NPC_SleepingBeautyPlaid.Id, AnimationType = TutorialAnimationType.Perform}, true);
                step.RegisterComponent(new AddTutorialAnimation{PieceId = PieceType.NPC_SleepingBeauty.Id, Target = new BoardPosition(20, 13, context.BoardDef.PieceLayer), AnimationType = TutorialAnimationType.Complete}, true);
                
                break;
            }
            case 15: // unlock Firefly
            {
                step = new FireflyLockTutorialStep();
                
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = GameDataService.Current.ConstantsManager.StartLevelFirefly, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition{Target = GameDataService.Current.ConstantsManager.StartLevelFirefly, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            default:
                return null;
        }
        
        step.Id = index;
        
        return step;
    }
}