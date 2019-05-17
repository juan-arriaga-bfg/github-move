using Debug = IW.Logger;
using System.Collections.Generic;
using BfgAnalytics;
using Quests;

public static class TutorialBuilder
{
    public const int FogStepIndex = 4;
    public const int WorkerStepIndex = 10;
    public const int LockPRStepIndex = 12;
    public const int LockMarketStepIndex = 15;
    public const int LockOrderStepIndex = 18;
    public const int FirstOrderStepIndex = 19;
    public const int LockFireflyStepIndex = 21;
    public const int LockEventGameStepIndex = 29;
    
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
                    FromPositions = new List<BoardPosition> {new BoardPosition(20, 11, BoardLayer.Piece.Layer)},
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(20, 12, BoardLayer.Piece.Layer),
                        new BoardPosition(21, 12, BoardLayer.Piece.Layer),
                        new BoardPosition(20, 11, BoardLayer.Piece.Layer)
                    },
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("merge_1"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("merge_1", currentStep.StartTime),
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
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
                        new BoardPosition(20, 12, BoardLayer.Piece.Layer),
                        new BoardPosition(21, 12, BoardLayer.Piece.Layer),
                        new BoardPosition(20, 11, BoardLayer.Piece.Layer)
                    },
                    ToPositions = new List<BoardPosition>
                    {
                        new BoardPosition(22, 13, BoardLayer.Piece.Layer),
                        new BoardPosition(22, 14, BoardLayer.Piece.Layer)
                    },
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("merge_2"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("merge_2", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                
                break;
            }
            case 2: // tutorial 2 step 1
            {
                step = new HighlightPiecesTutorialStep
                {
                    Targets = new List<int>{PieceType.PR_C1.Id},
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("createprc4")
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition {Target = 1, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCounterTutorialCondition {Target = 1, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "2_ClearFog", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 3: // tutorial 2 step 2
            {
                step = new HighlightPiecesTutorialStep
                {
                    Targets = new List<int>{PieceType.PR_C1.Id, PieceType.PR_C2.Id, PieceType.PR_C3.Id},
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("createprc4", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition {Target = 2, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "2_ClearFog", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 4: // tutorial 3 - clear fog
            {
                if (FogStepIndex != index) Debug.LogError("Tutorial Error: FogStepIndex != index");
                
                step = new HighlightFogTutorialStep
                {
                    Delay = 0,
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("openfog"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("openfog", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 2, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "2_ClearFog", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "2_ClearFog", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 5: // tutorial 4 - remove tree
            {
                step = new SelectStorageTutorialStep<ObstacleBubbleView>
                {
                    Delay = 2, 
                    Targets = new List<int>{PieceType.OB1_TT.Id, PieceType.OB2_TT.Id},
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("choptree"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("choptree", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition {Target = 4, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckWorkerTutorialCondition {ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "3_KillTree", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 6: // tutorial 5 step 1
            {
                step = new HighlightPiecesTutorialStep
                {
                    Targets = new List<int>{PieceType.A1.Id},
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("create3a2"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("create3a2", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 7: // tutorial 6 - create ingredient
            {
                step = new BoardArrowTutorialStep
                {
                    Targets = new List<int>{PieceType.PR_A4.Id},
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("harvest"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("harvest", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "7_CreatePiece_PR_A5", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.PR_A5.Id, Amount = 2, MoreThan = true, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "7_CreatePiece_PR_A5", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 8: // tutorial 10 - collecting ingredients
            {
                step = new BoardArrowTutorialStep
                {
                    Targets = new List<int>{PieceType.PR_C5.Id},
                    IsAnyStartCondition = true,
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("ingredients"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("ingredients", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.PR_C5.Id, Amount = 1, MoreThan = true, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition {Target = 3, Currency = new List<string>{Currency.PR_C5.Name}, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.PR_C5.Id, Amount = 3, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 9: // tutorial 8 - crystal
            {
                step = new CrystalTutorialStep
                {
                    Delay = 0,
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("boostcr"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("boostcr", currentStep.StartTime)
                };
                break;
            }
            case 10: // tutorial 9 - worker
            {
                if (WorkerStepIndex != index) Debug.LogError("Tutorial Error: WorkerStepIndex != index");
                
                step = new WorkerTutorialStep2
                {
                    Delay = 0,
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("instantworker"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("instantworker", currentStep.StartTime)
                };
                break;
            }
            case 11: // remove worker
            {
                step = new WorkerTutorialStep1 {IsIgnoreDebug = false, IsIgnoreUi = true};
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                
                step.RegisterComponent(new RemoveTutorialAnimation {PieceId = PieceType.NPC_Gnome.Id, AnimationType = TutorialAnimationType.Perform}, true);
                step.RegisterComponent(new RemoveTutorialAnimation {PieceId = PieceType.NPC_Gnome.Id, AnimationType = TutorialAnimationType.Complete}, true);
                
                break;
            }
            case 12: // unlock PR pieces
            {
                if (LockPRStepIndex != index) Debug.LogError("Tutorial Error: LockPRStepIndex != index");
                
                step = new PrLockTutorialStep();
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 13: // lock camera
            {
                step = new CameraLockTutorialStep();
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 14: // lock buttons Worker, Energy, Codex
            {
                step = new UiLockTutorialStep {Targets = new List<UiLockTutorialItem>{UiLockTutorialItem.Worker, UiLockTutorialItem.Energy, UiLockTutorialItem.Codex}};
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "4_CreatePiece_A2", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                 
                break;
            }
            case 15: // lock buttons Market
            {
                if (LockMarketStepIndex != index) Debug.LogError("Tutorial Error: LockMarketStepIndex != index");
                
                step = new UiLockTutorialStep {Targets = new List<UiLockTutorialItem>{UiLockTutorialItem.Shop}};
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "21_OpenChest", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "21_OpenChest", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 16: // lock buttons Orders
            {
                step = new UiLockTutorialStep {Targets = new List<UiLockTutorialItem>{UiLockTutorialItem.Orders}};
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 17: // lock buttons Remove, Daily
            {
                step = new UiLockTutorialStep {Targets = new List<UiLockTutorialItem> {UiLockTutorialItem.Remove, UiLockTutorialItem.Daily}};
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "92_CreatePiece_NPC_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "92_CreatePiece_NPC_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 18: // lock Orders
            {
                if (LockOrderStepIndex != index) Debug.LogError("Tutorial Error: LockOrderStepIndex != index");
                
                step = new OrdersLockTutorialStep {IsIgnoreDebug = false};
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 19: // Orders window tutorial
            {
                if (FirstOrderStepIndex != index) Debug.LogError("Tutorial Error: FirstOrderStepIndex != index");
                
                step = new BaseTutorialStep
                {
                    IsAnyStartCondition = true,
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("order"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("order", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckStepTutorialCondition {Target = 18, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "65_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition {Target = 2, Currency = new List<string>{Currency.Order.Name}, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "16_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 20: // add SleepingBeauty
            {
                step = new BaseTutorialStep {IsIgnoreDebug = false, IsIgnoreUi = true};
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Completed, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "1_CreatePiece_PR_C4", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Hard}, true);
                 
                step.RegisterComponent(new RemoveTutorialAnimation {PieceId = PieceType.NPC_SleepingBeautyPlaid.Id, AnimationType = TutorialAnimationType.Perform}, true);
                step.RegisterComponent(new AddTutorialAnimation {PieceId = PieceType.NPC_A.Id, Target = new BoardPosition(20, 13, BoardLayer.Piece.Layer), AnimationType = TutorialAnimationType.Complete}, true);
                
                break;
            }
            case 21: // unlock Firefly
            {
                if (LockFireflyStepIndex != index) Debug.LogError("Tutorial Error: LockFireflyStepIndex != index");
                
                step = new FireflyLockTutorialStep
                {
                    IsIgnoreDebug = false,
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("firefly"),
                };
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = GameDataService.Current.ConstantsManager.StartLevelFirefly, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = GameDataService.Current.ConstantsManager.StartLevelFirefly, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 22: // use Mine
            {
                step = new SelectStorageTutorialStep<ObstacleBubbleView>
                {
                    Delay = 2, IsFocusLock = true, IsFastStart = true, Targets = new List<int>{PieceType.MN_B1.Id},
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("mine"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("mine", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "12_UseMine", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckWorkerTutorialCondition {ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "12_UseMine", TargetState = TaskState.InProgress, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            case 23: // free chest tutorial
            {
                step = new MarketTutorialStep
                {
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("market"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("market", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "21_OpenChest", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.CH_Free.Id, Amount = 1, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckPieceInAirShipTutorialCondition {Target = PieceType.CH_Free.Id, ConditionType = TutorialConditionType.Hard}, true);
                break;
            }
            case 24: // daily quest tutorial
            {
                step = new DailyTutorialStep
                {
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("daily"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("daily", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "92_CreatePiece_NPC_C4", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.NPC_C5.Id, ConditionType = TutorialConditionType.Complete}, true);
                break;
            }
            case 25: // offer 1
            {
                step = new OfferStep {Target = 1, IsIgnoreUi = true};
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "16_CompleteOrder", TargetState = TaskState.New, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckTimerCompleteTutorialCondition {Target = BoardService.Current.FirstBoard.MarketLogic.OfferTimer, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition {Target = 1, Currency = new List<string>{Currency.Offer.Name}, ConditionType = TutorialConditionType.Hard}, true);
                break;
            }
            case 26: // offer 2
            {
                step = new OfferStep {Target = 2, IsIgnoreUi = true};
                
                step.RegisterComponent(new CheckStepTutorialCondition {Target = 25, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 10, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckTimerCompleteTutorialCondition {Target = BoardService.Current.FirstBoard.MarketLogic.OfferTimer, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckCurrencyTutorialCondition {Target = 2, Currency = new List<string>{Currency.Offer.Name}, IsPermanentCheck = true, ConditionType = TutorialConditionType.Hard}, true);
                break;
            }
            case 27: // first order piece click
            {
                step = new BoardArrowTutorialStep
                {
                    Targets = new List<int>{PieceType.RC_I.Id},
                    IsAnyStartCondition = true,
                    IsAnyCompleteCondition = true,
                    OnFirstStartCallback = (currentStep) => Analytics.SendTutorialStartStepEvent("orderclick"),
                    OnCompleteCallback = (currentStep) => Analytics.SendTutorialEndStepEvent("orderclick", currentStep.StartTime)
                };
                
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.RC_I.Id, Amount = 1, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.RC_I.Id, Amount = 1, MoreThan = true, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckPieceTutorialCondition {Target = PieceType.RC_I.Id, Amount = 0, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckPieceChangeTutorialCondition {Target = PieceType.RC_I.Id, Amount = -1, ConditionType = TutorialConditionType.Complete}, true);
                break;
            }
            case 28: // vip island
            {
                step = new IslandStep {Target = new BoardPosition(10, 13), IsIgnoreDebug = false};
                
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "87_KillTree", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckQuestTutorialCondition {Target = "87_KillTree", TargetState = TaskState.Claimed, ConditionType = TutorialConditionType.Complete}, true);
                break;
            }
            case 29: // unlock EventGame
            {
                if (LockEventGameStepIndex != index) Debug.LogError("Tutorial Error: LockEventGameStepIndex != index");

                step = new EventGameLockTutorialStep {IsIgnoreDebug = false};
                
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = 0, ConditionType = TutorialConditionType.Start}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = GameDataService.Current.ConstantsManager.StartLevelEventGame, ConditionType = TutorialConditionType.Complete}, true);
                step.RegisterComponent(new CheckLevelTutorialCondition {Target = GameDataService.Current.ConstantsManager.StartLevelEventGame, ConditionType = TutorialConditionType.Hard}, true);
                
                break;
            }
            default:
                return null;
        }
        
        step.Id = index;
        
        return step;
    }
}