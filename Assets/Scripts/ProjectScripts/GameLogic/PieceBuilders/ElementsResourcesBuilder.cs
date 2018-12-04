using System.Collections.Generic;

public class ElementsResourcesBuilder
{
    public Dictionary<int, string> Build()
    {
        var dict = new Dictionary<int, string>();
        
#region View
        
        dict.Add((int)ViewType.AddResource, R.AddResourceView);
        dict.Add((int)ViewType.HintArrow, R.HintArrow);
        dict.Add((int)ViewType.StorageState, R.ChangeStorageStateView);
        dict.Add((int)ViewType.BoardTimer, R.BoardTimerView);
        dict.Add((int)ViewType.LevelLabel, R.PieceLevelView);
        dict.Add((int)ViewType.Menu, R.MenuView);
        dict.Add((int)ViewType.ObstacleState, R.ChangeObstacleStateView);
        dict.Add((int)ViewType.Bubble, R.BubbleView);
        dict.Add((int)ViewType.MergeParticle, R.MergeParticleSystem);
        dict.Add((int)ViewType.Progress, R.BoardProgressView);
        dict.Add((int)ViewType.Warning, R.Warning);
        dict.Add((int)ViewType.Lock, R.LockView);
        dict.Add((int)ViewType.Cell, R.Cell);
        dict.Add((int)ViewType.OrderBubble, R.OrderBubbleView);
        dict.Add((int)ViewType.Firefly, R.Firefly);
        dict.Add((int)ViewType.DefaultWorker, R.DefaultWorker);
        dict.Add((int)ViewType.ExtraWorker, R.ExtraWorker);
        dict.Add((int)ViewType.TutorialMergeFinger, R.TutorialMergeFinger);
        dict.Add((int)ViewType.PieceRemover, R.PieceRemover);
        dict.Add((int)ViewType.GodRay, R.GodRayView);
        dict.Add((int)ViewType.UIContainer, R.UIBoardViewContainer);
        
#endregion
        
#region Default
        
        dict.Add(PieceType.Generic.Id, R.GenericPiece);
        dict.Add(PieceType.LockedEmpty.Id, R.LockedEmpty);
        
#endregion
        
#region Characters
        
        dict.Add(PieceType.NPC_SleepingBeauty.Id, R.NPC_SleepingBeautyPiece);
        dict.Add(PieceType.NPC_SleepingBeautyPlaid.Id, R.NPC_SleepingBeautyPlaidPiece);
        dict.Add(PieceType.NPC_Rapunzel.Id, R.NPC_RapunzelPiece);
        dict.Add(PieceType.NPC_PussInBoots.Id, R.NPC_PussInBootsPiece);
        dict.Add(PieceType.NPC_Gnome.Id, R.NPC_GnomePiece);
        dict.Add(PieceType.NPC_5.Id, R.NPC_5Piece);
        dict.Add(PieceType.NPC_6.Id, R.NPC_6Piece);
        dict.Add(PieceType.NPC_7.Id, R.NPC_7Piece);
        dict.Add(PieceType.NPC_8.Id, R.NPC_8Piece);
        dict.Add(PieceType.NPC_9.Id, R.NPC_9Piece);
        
#endregion
        
#region Enemies
        
        dict.Add(PieceType.Enemy1.Id, R.Enemy1Piece);
        
#endregion
        
#region Boosters
        
        dict.Add(PieceType.Boost_WR.Id, R.Boost_WRPiece);
        
        dict.Add(PieceType.Boost_CR1.Id, R.Boost_CR1Piece);
        dict.Add(PieceType.Boost_CR2.Id, R.Boost_CR2Piece);
        dict.Add(PieceType.Boost_CR3.Id, R.Boost_CR3Piece);
        dict.Add(PieceType.Boost_CR.Id, R.Boost_CRPiece);
        
#endregion
        
#region Currencies
        
        dict.Add(PieceType.Soft1.Id, R.Soft1Piece);
        dict.Add(PieceType.Soft2.Id, R.Soft2Piece);
        dict.Add(PieceType.Soft3.Id, R.Soft3Piece);
        dict.Add(PieceType.Soft4.Id, R.Soft4Piece);
        dict.Add(PieceType.Soft5.Id, R.Soft5Piece);
        dict.Add(PieceType.Soft6.Id, R.Soft6Piece);
        
        dict.Add(PieceType.Hard1.Id, R.Hard1Piece);
        dict.Add(PieceType.Hard2.Id, R.Hard2Piece);
        dict.Add(PieceType.Hard3.Id, R.Hard3Piece);
        dict.Add(PieceType.Hard4.Id, R.Hard4Piece);
        dict.Add(PieceType.Hard5.Id, R.Hard5Piece);
        dict.Add(PieceType.Hard6.Id, R.Hard6Piece);
        
#endregion
        
#region Mines
        
        dict.Add(PieceType.MN_B.Id, R.MN_BPiece);
        dict.Add(PieceType.MN_C.Id, R.MN_CPiece);
        dict.Add(PieceType.MN_D.Id, R.MN_DPiece);
        
#endregion
        
#region Chests
        
        dict.Add(PieceType.CH_Free.Id, R.CH_FreePiece);
        
        dict.Add(PieceType.SK1_PR.Id, R.SK1_PRPiece);
        dict.Add(PieceType.SK2_PR.Id, R.SK2_PRPiece);
        dict.Add(PieceType.SK3_PR.Id, R.SK3_PRPiece);
        
        dict.Add(PieceType.CH1_A.Id, R.CH1_APiece);
        dict.Add(PieceType.CH2_A.Id, R.CH2_APiece);
        dict.Add(PieceType.CH3_A.Id, R.CH3_APiece);
        
        dict.Add(PieceType.CH1_B.Id, R.CH1_BPiece);
        dict.Add(PieceType.CH2_B.Id, R.CH2_BPiece);
        dict.Add(PieceType.CH3_B.Id, R.CH3_BPiece);
        
        dict.Add(PieceType.CH1_C.Id, R.CH1_CPiece);
        dict.Add(PieceType.CH2_C.Id, R.CH2_CPiece);
        dict.Add(PieceType.CH3_C.Id, R.CH3_CPiece);
        
        dict.Add(PieceType.CH1_D.Id, R.CH1_DPiece);
        dict.Add(PieceType.CH2_D.Id, R.CH2_DPiece);
        dict.Add(PieceType.CH3_D.Id, R.CH3_DPiece);
        
#endregion
        
#region Obstacles
        
        dict.Add(PieceType.Fog.Id, R.FogPiece);
        
        dict.Add(PieceType.OB1_TT.Id, R.OB1_TTPiece);
        dict.Add(PieceType.OB2_TT.Id, R.OB2_TTPiece);
        
        dict.Add(PieceType.OB1_A.Id, R.OB1_APiece);
        dict.Add(PieceType.OB2_A.Id, R.OB2_APiece);
        dict.Add(PieceType.OB3_A.Id, R.OB3_APiece);
        dict.Add(PieceType.OB4_A.Id, R.OB4_APiece);
        dict.Add(PieceType.OB5_A.Id, R.OB5_APiece);
        dict.Add(PieceType.OB6_A.Id, R.OB6_APiece);
        dict.Add(PieceType.OB7_A.Id, R.OB7_APiece);
        dict.Add(PieceType.OB8_A.Id, R.OB8_APiece);
        dict.Add(PieceType.OB9_A.Id, R.OB9_APiece);
        
        dict.Add(PieceType.OB_PR_A.Id, R.OB_PR_APiece);
        dict.Add(PieceType.OB_PR_B.Id, R.OB_PR_BPiece);
        dict.Add(PieceType.OB_PR_C.Id, R.OB_PR_CPiece);
        dict.Add(PieceType.OB_PR_D.Id, R.OB_PR_DPiece);
        dict.Add(PieceType.OB_PR_E.Id, R.OB_PR_EPiece);
        
#endregion
        
#region Simple Pieces
        
        #region A
        
        dict.Add(PieceType.A1.Id, R.A1Piece);
        dict.Add(PieceType.A2.Id, R.A2Piece);
        dict.Add(PieceType.A3Fake.Id, R.A3Piece);
        dict.Add(PieceType.A3.Id, R.A3Piece);
        dict.Add(PieceType.A4Fake.Id, R.A4Piece);
        dict.Add(PieceType.A4.Id, R.A4Piece);
        dict.Add(PieceType.A5Fake.Id, R.A5Piece);
        dict.Add(PieceType.A5.Id, R.A5Piece);
        dict.Add(PieceType.A6Fake.Id, R.A6Piece);
        dict.Add(PieceType.A6.Id, R.A6Piece);
        dict.Add(PieceType.A7Fake.Id, R.A7Piece);
        dict.Add(PieceType.A7.Id, R.A7Piece);
        dict.Add(PieceType.A8Fake.Id, R.A8Piece);
        dict.Add(PieceType.A8.Id, R.A8Piece);
        dict.Add(PieceType.A9Fake.Id, R.A9Piece);
        dict.Add(PieceType.A9.Id, R.A9Piece);
        
        #endregion
        
        #region B
        
        dict.Add(PieceType.B1.Id, R.B1Piece);
        dict.Add(PieceType.B2.Id, R.B2Piece);
        dict.Add(PieceType.B3Fake.Id, R.B3Piece);
        dict.Add(PieceType.B3.Id, R.B3Piece);
        dict.Add(PieceType.B4Fake.Id, R.B4Piece);
        dict.Add(PieceType.B4.Id, R.B4Piece);
        dict.Add(PieceType.B5Fake.Id, R.B5Piece);
        dict.Add(PieceType.B5.Id, R.B5Piece);
        dict.Add(PieceType.B6Fake.Id, R.B6Piece);
        dict.Add(PieceType.B6.Id, R.B6Piece);
        dict.Add(PieceType.B7Fake.Id, R.B7Piece);
        dict.Add(PieceType.B7.Id, R.B7Piece);
        dict.Add(PieceType.B8Fake.Id, R.B8Piece);
        dict.Add(PieceType.B8.Id, R.B8Piece);
        dict.Add(PieceType.B9Fake.Id, R.B9Piece);
        dict.Add(PieceType.B9.Id, R.B9Piece);
        dict.Add(PieceType.B10Fake.Id, R.B10Piece);
        dict.Add(PieceType.B10.Id, R.B10Piece);
        dict.Add(PieceType.B11Fake.Id, R.B11Piece);
        dict.Add(PieceType.B11.Id, R.B11Piece);
        
        #endregion
        
        #region C
        
        dict.Add(PieceType.C1.Id, R.C1Piece);
        dict.Add(PieceType.C2.Id, R.C2Piece);
        dict.Add(PieceType.C3Fake.Id, R.C3Piece);
        dict.Add(PieceType.C3.Id, R.C3Piece);
        dict.Add(PieceType.C4Fake.Id, R.C4Piece);
        dict.Add(PieceType.C4.Id, R.C4Piece);
        dict.Add(PieceType.C5Fake.Id, R.C5Piece);
        dict.Add(PieceType.C5.Id, R.C5Piece);
        dict.Add(PieceType.C6Fake.Id, R.C6Piece);
        dict.Add(PieceType.C6.Id, R.C6Piece);
        dict.Add(PieceType.C7Fake.Id, R.C7Piece);
        dict.Add(PieceType.C7.Id, R.C7Piece);
        dict.Add(PieceType.C8Fake.Id, R.C8Piece);
        dict.Add(PieceType.C8.Id, R.C8Piece);
        dict.Add(PieceType.C9Fake.Id, R.C9Piece);
        dict.Add(PieceType.C9.Id, R.C9Piece);
        
        #endregion
        
        #region D
        
        dict.Add(PieceType.D1.Id, R.D1Piece);
        dict.Add(PieceType.D2.Id, R.D2Piece);
        dict.Add(PieceType.D3Fake.Id, R.D3Piece);
        dict.Add(PieceType.D3.Id, R.D3Piece);
        dict.Add(PieceType.D4Fake.Id, R.D4Piece);
        dict.Add(PieceType.D4.Id, R.D4Piece);
        dict.Add(PieceType.D5Fake.Id, R.D5Piece);
        dict.Add(PieceType.D5.Id, R.D5Piece);
        dict.Add(PieceType.D6Fake.Id, R.D6Piece);
        dict.Add(PieceType.D6.Id, R.D6Piece);
        dict.Add(PieceType.D7Fake.Id, R.D7Piece);
        dict.Add(PieceType.D7.Id, R.D7Piece);
        dict.Add(PieceType.D8Fake.Id, R.D8Piece);
        dict.Add(PieceType.D8.Id, R.D8Piece);
        dict.Add(PieceType.D9Fake.Id, R.D9Piece);
        dict.Add(PieceType.D9.Id, R.D9Piece);
            
        #endregion
        
#endregion
        
#region Reproduction Pieces
        
        #region PR_A
        
        dict.Add(PieceType.PR_A1.Id, R.PR_A1Piece);
        dict.Add(PieceType.PR_A2.Id, R.PR_A2Piece);
        dict.Add(PieceType.PR_A3.Id, R.PR_A3Piece);
        dict.Add(PieceType.PR_A4.Id, R.PR_A4Piece);
        dict.Add(PieceType.PR_A5.Id, R.PR_A5Piece);
        
        #endregion
        
        #region PR_B
        
        dict.Add(PieceType.PR_B1.Id, R.PR_B1Piece);
        dict.Add(PieceType.PR_B2.Id, R.PR_B2Piece);
        dict.Add(PieceType.PR_B3.Id, R.PR_B3Piece);
        dict.Add(PieceType.PR_B4.Id, R.PR_B4Piece);
        dict.Add(PieceType.PR_B5.Id, R.PR_B5Piece);
        
        #endregion
        
        #region PR_C
        
        dict.Add(PieceType.PR_C1.Id, R.PR_C1Piece);
        dict.Add(PieceType.PR_C2.Id, R.PR_C2Piece);
        dict.Add(PieceType.PR_C3.Id, R.PR_C3Piece);
        dict.Add(PieceType.PR_C4.Id, R.PR_C4Piece);
        dict.Add(PieceType.PR_C5.Id, R.PR_C5Piece);
        
        #endregion
        
        #region PR_D
        
        dict.Add(PieceType.PR_D1.Id, R.PR_D1Piece);
        dict.Add(PieceType.PR_D2.Id, R.PR_D2Piece);
        dict.Add(PieceType.PR_D3.Id, R.PR_D3Piece);
        dict.Add(PieceType.PR_D4.Id, R.PR_D4Piece);
        dict.Add(PieceType.PR_D5.Id, R.PR_D5Piece);
        
        #endregion
        
        #region PR_E
        
        dict.Add(PieceType.PR_E1.Id, R.PR_E1Piece);
        dict.Add(PieceType.PR_E2.Id, R.PR_E2Piece);
        dict.Add(PieceType.PR_E3.Id, R.PR_E3Piece);
        dict.Add(PieceType.PR_E4.Id, R.PR_E4Piece);
        dict.Add(PieceType.PR_E5.Id, R.PR_E5Piece);
        
        #endregion
        
#endregion
        
        return dict;
    }
}