using System.Collections.Generic;

public class ElementsResourcesBuilder
{
    public Dictionary<int, string> Build()
    {
        var dict = new Dictionary<int, string>();
        
#region View

        dict.Add((int) ViewType.AddResource, R.AddResourceView);
        dict.Add((int) ViewType.HintArrow, R.HintArrow);
        dict.Add((int) ViewType.BoardTimer, R.BoardTimerView);
        dict.Add((int) ViewType.ObstacleBubble, R.ObstacleBubbleView);
        dict.Add((int) ViewType.Bubble, R.BubbleView);
        dict.Add((int) ViewType.MergeParticle, R.MergeParticleSystem);
        dict.Add((int) ViewType.Progress, R.BoardProgressView);
        dict.Add((int) ViewType.Warning, R.Warning);
        dict.Add((int) ViewType.Lock, R.LockView);
        dict.Add((int) ViewType.OrderBubble, R.OrderBubbleView);
        dict.Add((int) ViewType.Firefly, R.Firefly);
        dict.Add((int) ViewType.DefaultWorker, R.DefaultWorker);
        dict.Add((int) ViewType.ExtraWorker, R.ExtraWorker);
        dict.Add((int) ViewType.TutorialMergeFinger, R.TutorialMergeFinger);
        dict.Add((int) ViewType.PieceRemover, R.PieceRemover);
        dict.Add((int) ViewType.RewardsBubble, R.RewardsBubbleView);
        dict.Add((int) ViewType.GodRay, R.GodRayView);
        dict.Add((int) ViewType.UIContainer, R.UIBoardViewContainer);
        dict.Add((int) ViewType.FogProgress, R.FogProgressView);
        
#endregion
        
#region Default
        
        dict.Add(PieceType.Generic.Id, R.GenericPiece);
        dict.Add(PieceType.LockedEmpty.Id, R.LockedEmpty);
        
#endregion
        
#region Characters
        
        dict.Add(PieceType.NPC_A.Id, R.NPC_APiece);
        dict.Add(PieceType.NPC_SleepingBeautyPlaid.Id, R.NPC_SleepingBeautyPlaidPiece);
        dict.Add(PieceType.NPC_B.Id, R.NPC_BPiece);
        dict.Add(PieceType.NPC_C.Id, R.NPC_CPiece);
        dict.Add(PieceType.NPC_Gnome.Id, R.NPC_GnomePiece);
        dict.Add(PieceType.NPC_E.Id, R.NPC_EPiece);
        dict.Add(PieceType.NPC_F.Id, R.NPC_FPiece);
        dict.Add(PieceType.NPC_D.Id, R.NPC_DPiece);
        dict.Add(PieceType.NPC_8.Id, R.NPC_8Piece);
        dict.Add(PieceType.NPC_9.Id, R.NPC_9Piece);
        dict.Add(PieceType.NPC_10.Id, R.NPC_10Piece);
        dict.Add(PieceType.NPC_11.Id, R.NPC_11Piece);
        dict.Add(PieceType.NPC_12.Id, R.NPC_12Piece);
        dict.Add(PieceType.NPC_13.Id, R.NPC_13Piece);
        dict.Add(PieceType.NPC_14.Id, R.NPC_14Piece);
        dict.Add(PieceType.NPC_15.Id, R.NPC_15Piece);
        dict.Add(PieceType.NPC_16.Id, R.NPC_16Piece);
        dict.Add(PieceType.NPC_17.Id, R.NPC_17Piece);
        dict.Add(PieceType.NPC_18.Id, R.NPC_18Piece);
        dict.Add(PieceType.NPC_19.Id, R.NPC_19Piece);
        
#endregion
   
#region Character Pieces

        #region NPC_B

        dict.Add(PieceType.NPC_B1.Id, R.NPC_B1Piece);
        dict.Add(PieceType.NPC_B2.Id, R.NPC_B2Piece);
        dict.Add(PieceType.NPC_B3.Id, R.NPC_B3Piece);
        dict.Add(PieceType.NPC_B4.Id, R.NPC_B4Piece);
        dict.Add(PieceType.NPC_B5.Id, R.NPC_B5Piece);
        dict.Add(PieceType.NPC_B6.Id, R.NPC_B6Piece);
        dict.Add(PieceType.NPC_B7.Id, R.NPC_B7Piece);
        dict.Add(PieceType.NPC_B8.Id, R.NPC_B8Piece);

        #endregion
        
        #region NPC_C

        dict.Add(PieceType.NPC_C1.Id, R.NPC_C1Piece);
        dict.Add(PieceType.NPC_C2.Id, R.NPC_C2Piece);
        dict.Add(PieceType.NPC_C3.Id, R.NPC_C3Piece);
        dict.Add(PieceType.NPC_C4.Id, R.NPC_C4Piece);
        dict.Add(PieceType.NPC_C5.Id, R.NPC_C5Piece);
        dict.Add(PieceType.NPC_C6.Id, R.NPC_C6Piece);
        dict.Add(PieceType.NPC_C7.Id, R.NPC_C7Piece);
        dict.Add(PieceType.NPC_C8.Id, R.NPC_C8Piece);

        #endregion
        
        #region NPC_D

        dict.Add(PieceType.NPC_D1.Id, R.NPC_D1Piece);
        dict.Add(PieceType.NPC_D2.Id, R.NPC_D2Piece);
        dict.Add(PieceType.NPC_D3.Id, R.NPC_D3Piece);
        dict.Add(PieceType.NPC_D4.Id, R.NPC_D4Piece);
        dict.Add(PieceType.NPC_D5.Id, R.NPC_D5Piece);
        dict.Add(PieceType.NPC_D6.Id, R.NPC_D6Piece);
        dict.Add(PieceType.NPC_D7.Id, R.NPC_D7Piece);
        dict.Add(PieceType.NPC_D8.Id, R.NPC_D8Piece);

        #endregion
        
        #region NPC_E

        dict.Add(PieceType.NPC_E1.Id, R.NPC_E1Piece);
        dict.Add(PieceType.NPC_E2.Id, R.NPC_E2Piece);
        dict.Add(PieceType.NPC_E3.Id, R.NPC_E3Piece);
        dict.Add(PieceType.NPC_E4.Id, R.NPC_E4Piece);
        dict.Add(PieceType.NPC_E5.Id, R.NPC_E5Piece);
        dict.Add(PieceType.NPC_E6.Id, R.NPC_E6Piece);
        dict.Add(PieceType.NPC_E7.Id, R.NPC_E7Piece);
        dict.Add(PieceType.NPC_E8.Id, R.NPC_E8Piece);

        #endregion
        
        #region NPC_F

        dict.Add(PieceType.NPC_F1.Id, R.NPC_F1Piece);
        dict.Add(PieceType.NPC_F2.Id, R.NPC_F2Piece);
        dict.Add(PieceType.NPC_F3.Id, R.NPC_F3Piece);
        dict.Add(PieceType.NPC_F4.Id, R.NPC_F4Piece);
        dict.Add(PieceType.NPC_F5.Id, R.NPC_F5Piece);
        dict.Add(PieceType.NPC_F6.Id, R.NPC_F6Piece);
        dict.Add(PieceType.NPC_F7.Id, R.NPC_F7Piece);
        dict.Add(PieceType.NPC_F8.Id, R.NPC_F8Piece);

        #endregion
        
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
        
        dict.Add(PieceType.Mana1.Id, R.Mana1Piece);
        dict.Add(PieceType.Mana2.Id, R.Mana2Piece);
        dict.Add(PieceType.Mana3.Id, R.Mana3Piece);
        dict.Add(PieceType.Mana4.Id, R.Mana4Piece);
        dict.Add(PieceType.Mana5.Id, R.Mana5Piece);
        dict.Add(PieceType.Mana6.Id, R.Mana6Piece);
        
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
        dict.Add(PieceType.MN_F.Id, R.MN_FPiece);
        
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
        
        dict.Add(PieceType.CH1_E.Id, R.CH1_EPiece);
        dict.Add(PieceType.CH2_E.Id, R.CH2_EPiece);
        dict.Add(PieceType.CH3_E.Id, R.CH3_EPiece);
        
        dict.Add(PieceType.CH1_F.Id, R.CH1_FPiece);
        dict.Add(PieceType.CH2_F.Id, R.CH2_FPiece);
        dict.Add(PieceType.CH3_F.Id, R.CH3_FPiece);
        
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
        dict.Add(PieceType.OB_PR_F.Id, R.OB_PR_FPiece);
        dict.Add(PieceType.OB_PR_G.Id, R.OB_PR_GPiece);
        
        dict.Add(PieceType.OB1_E.Id, R.OB1_EPiece);
        dict.Add(PieceType.OB2_E.Id, R.OB2_EPiece);
        dict.Add(PieceType.OB3_E.Id, R.OB3_EPiece);
        dict.Add(PieceType.OB4_E.Id, R.OB4_EPiece);
        dict.Add(PieceType.OB5_E.Id, R.OB5_EPiece);
        
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
        
        #region E
        
        dict.Add(PieceType.E1.Id, R.E1Piece);
        dict.Add(PieceType.E2.Id, R.E2Piece);
        dict.Add(PieceType.E3Fake.Id, R.E3Piece);
        dict.Add(PieceType.E3.Id, R.E3Piece);
        dict.Add(PieceType.E4Fake.Id, R.E4Piece);
        dict.Add(PieceType.E4.Id, R.E4Piece);
        dict.Add(PieceType.E5Fake.Id, R.E5Piece);
        dict.Add(PieceType.E5.Id, R.E5Piece);
        dict.Add(PieceType.E6Fake.Id, R.E6Piece);
        dict.Add(PieceType.E6.Id, R.E6Piece);
        dict.Add(PieceType.E7Fake.Id, R.E7Piece);
        dict.Add(PieceType.E7.Id, R.E7Piece);
        dict.Add(PieceType.E8Fake.Id, R.E8Piece);
        dict.Add(PieceType.E8.Id, R.E8Piece);
        dict.Add(PieceType.E9Fake.Id, R.E9Piece);
        dict.Add(PieceType.E9.Id, R.E9Piece);
            
        #endregion
        
        #region F
        
        dict.Add(PieceType.F1.Id, R.F1Piece);
        dict.Add(PieceType.F2.Id, R.F2Piece);
        dict.Add(PieceType.F3Fake.Id, R.F3Piece);
        dict.Add(PieceType.F3.Id, R.F3Piece);
        dict.Add(PieceType.F4Fake.Id, R.F4Piece);
        dict.Add(PieceType.F4.Id, R.F4Piece);
        dict.Add(PieceType.F5Fake.Id, R.F5Piece);
        dict.Add(PieceType.F5.Id, R.F5Piece);
        dict.Add(PieceType.F6Fake.Id, R.F6Piece);
        dict.Add(PieceType.F6.Id, R.F6Piece);
        dict.Add(PieceType.F7Fake.Id, R.F7Piece);
        dict.Add(PieceType.F7.Id, R.F7Piece);
        dict.Add(PieceType.F8Fake.Id, R.F8Piece);
        dict.Add(PieceType.F8.Id, R.F8Piece);
        dict.Add(PieceType.F9Fake.Id, R.F9Piece);
        dict.Add(PieceType.F9.Id, R.F9Piece);
            
        #endregion
        
        #region G
        
        dict.Add(PieceType.G1.Id, R.G1Piece);
        dict.Add(PieceType.G2.Id, R.G2Piece);
        dict.Add(PieceType.G3Fake.Id, R.G3Piece);
        dict.Add(PieceType.G3.Id, R.G3Piece);
        dict.Add(PieceType.G4Fake.Id, R.G4Piece);
        dict.Add(PieceType.G4.Id, R.G4Piece);
        dict.Add(PieceType.G5Fake.Id, R.G5Piece);
        dict.Add(PieceType.G5.Id, R.G5Piece);
        dict.Add(PieceType.G6Fake.Id, R.G6Piece);
        dict.Add(PieceType.G6.Id, R.G6Piece);
        dict.Add(PieceType.G7Fake.Id, R.G7Piece);
        dict.Add(PieceType.G7.Id, R.G7Piece);
        dict.Add(PieceType.G8Fake.Id, R.G8Piece);
        dict.Add(PieceType.G8.Id, R.G8Piece);
        dict.Add(PieceType.G9Fake.Id, R.G9Piece);
        dict.Add(PieceType.G9.Id, R.G9Piece);
            
        #endregion
        
        #region H
        
        dict.Add(PieceType.H1.Id, R.H1Piece);
        dict.Add(PieceType.H2.Id, R.H2Piece);
        dict.Add(PieceType.H3Fake.Id, R.H3Piece);
        dict.Add(PieceType.H3.Id, R.H3Piece);
        dict.Add(PieceType.H4Fake.Id, R.H4Piece);
        dict.Add(PieceType.H4.Id, R.H4Piece);
        dict.Add(PieceType.H5Fake.Id, R.H5Piece);
        dict.Add(PieceType.H5.Id, R.H5Piece);
        dict.Add(PieceType.H6Fake.Id, R.H6Piece);
        dict.Add(PieceType.H6.Id, R.H6Piece);
        dict.Add(PieceType.H7Fake.Id, R.H7Piece);
        dict.Add(PieceType.H7.Id, R.H7Piece);
        dict.Add(PieceType.H8Fake.Id, R.H8Piece);
        dict.Add(PieceType.H8.Id, R.H8Piece);
        dict.Add(PieceType.H9Fake.Id, R.H9Piece);
        dict.Add(PieceType.H9.Id, R.H9Piece);
            
        #endregion
        
        #region I
        
        dict.Add(PieceType.I1.Id, R.I1Piece);
        dict.Add(PieceType.I2.Id, R.I2Piece);
        dict.Add(PieceType.I3Fake.Id, R.I3Piece);
        dict.Add(PieceType.I3.Id, R.I3Piece);
        dict.Add(PieceType.I4Fake.Id, R.I4Piece);
        dict.Add(PieceType.I4.Id, R.I4Piece);
        dict.Add(PieceType.I5Fake.Id, R.I5Piece);
        dict.Add(PieceType.I5.Id, R.I5Piece);
        dict.Add(PieceType.I6Fake.Id, R.I6Piece);
        dict.Add(PieceType.I6.Id, R.I6Piece);
        dict.Add(PieceType.I7Fake.Id, R.I7Piece);
        dict.Add(PieceType.I7.Id, R.I7Piece);
        dict.Add(PieceType.I8Fake.Id, R.I8Piece);
        dict.Add(PieceType.I8.Id, R.I8Piece);
        dict.Add(PieceType.I9Fake.Id, R.I9Piece);
        dict.Add(PieceType.I9.Id, R.I9Piece);
            
        #endregion
        
        #region J
        
        dict.Add(PieceType.J1.Id, R.J1Piece);
        dict.Add(PieceType.J2.Id, R.J2Piece);
        dict.Add(PieceType.J3Fake.Id, R.J3Piece);
        dict.Add(PieceType.J3.Id, R.J3Piece);
        dict.Add(PieceType.J4Fake.Id, R.J4Piece);
        dict.Add(PieceType.J4.Id, R.J4Piece);
        dict.Add(PieceType.J5Fake.Id, R.J5Piece);
        dict.Add(PieceType.J5.Id, R.J5Piece);
        dict.Add(PieceType.J6Fake.Id, R.J6Piece);
        dict.Add(PieceType.J6.Id, R.J6Piece);
        dict.Add(PieceType.J7Fake.Id, R.J7Piece);
        dict.Add(PieceType.J7.Id, R.J7Piece);
        dict.Add(PieceType.J8Fake.Id, R.J8Piece);
        dict.Add(PieceType.J8.Id, R.J8Piece);
        dict.Add(PieceType.J9Fake.Id, R.J9Piece);
        dict.Add(PieceType.J9.Id, R.J9Piece);
            
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
            
        #region PR_F
        
        dict.Add(PieceType.PR_F1.Id, R.PR_F1Piece);
        dict.Add(PieceType.PR_F2.Id, R.PR_F2Piece);
        dict.Add(PieceType.PR_F3.Id, R.PR_F3Piece);
        dict.Add(PieceType.PR_F4.Id, R.PR_F4Piece);
        dict.Add(PieceType.PR_F5.Id, R.PR_F5Piece);
        
        #endregion
            
        #region PR_G
        
        dict.Add(PieceType.PR_G1.Id, R.PR_G1Piece);
        dict.Add(PieceType.PR_G2.Id, R.PR_G2Piece);
        dict.Add(PieceType.PR_G3.Id, R.PR_G3Piece);
        dict.Add(PieceType.PR_G4.Id, R.PR_G4Piece);
        dict.Add(PieceType.PR_G5.Id, R.PR_G5Piece);
        
        #endregion
        
#endregion
        
        return dict;
    }
}