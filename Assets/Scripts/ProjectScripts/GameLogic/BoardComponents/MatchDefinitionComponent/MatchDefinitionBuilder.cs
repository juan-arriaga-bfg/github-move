using System.Collections.Generic;

public struct PieceMatchDef
{
    public int Next;
    public int Previous;
    public int Amount;
    public bool IsIgnore;

    public List<List<int>> Pattern;
}

public class MatchDefinitionBuilder
{
    public Dictionary<int, PieceMatchDef> Build()
    {
        var dict = new Dictionary<int, PieceMatchDef>();
        
#region Characters
        
        dict.Add(PieceType.NPC_A.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.None.Id, Amount = -1});
        
#endregion
        
#region Character Pieces

        #region NPC_B
        
        dict.Add(PieceType.NPC_B1.Id, new PieceMatchDef {Next = PieceType.NPC_B2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.NPC_B2.Id, new PieceMatchDef {Next = PieceType.NPC_B3.Id, Previous = PieceType.NPC_B1.Id, Amount = 3});
        dict.Add(PieceType.NPC_B3.Id, new PieceMatchDef {Next = PieceType.NPC_B.Id, Previous = PieceType.NPC_B2.Id, Amount = 3});
        dict.Add(PieceType.NPC_B.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.NPC_B3.Id, Amount = -1});
        
        #endregion
        
        #region NPC_C
        
        dict.Add(PieceType.NPC_C1.Id, new PieceMatchDef {Next = PieceType.NPC_C2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.NPC_C2.Id, new PieceMatchDef {Next = PieceType.NPC_C3.Id, Previous = PieceType.NPC_C1.Id, Amount = 3});
        dict.Add(PieceType.NPC_C3.Id, new PieceMatchDef {Next = PieceType.NPC_C4.Id, Previous = PieceType.NPC_C2.Id, Amount = 3});
        dict.Add(PieceType.NPC_C4.Id, new PieceMatchDef {Next = PieceType.NPC_C.Id, Previous = PieceType.NPC_C3.Id, Amount = 3});
//        dict.Add(PieceType.NPC_C5.Id, new PieceMatchDef {Next = PieceType.NPC_C6.Id, Previous = PieceType.NPC_C4.Id, Amount = 3});
//        dict.Add(PieceType.NPC_C6.Id, new PieceMatchDef {Next = PieceType.NPC_C7.Id, Previous = PieceType.NPC_C5.Id, Amount = 3});
//        dict.Add(PieceType.NPC_C7.Id, new PieceMatchDef {Next = PieceType.NPC_C8.Id, Previous = PieceType.NPC_C6.Id, Amount = 3});
//        dict.Add(PieceType.NPC_C8.Id, new PieceMatchDef {Next = PieceType.NPC_C.Id, Previous = PieceType.NPC_C7.Id, Amount = 3});
        dict.Add(PieceType.NPC_C.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.NPC_C4.Id, Amount = -1});
        
        #endregion
        
        #region NPC_D
        
        dict.Add(PieceType.NPC_D1.Id, new PieceMatchDef {Next = PieceType.NPC_D2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.NPC_D2.Id, new PieceMatchDef {Next = PieceType.NPC_D3.Id, Previous = PieceType.NPC_D1.Id, Amount = 3});
        dict.Add(PieceType.NPC_D3.Id, new PieceMatchDef {Next = PieceType.NPC_D4.Id, Previous = PieceType.NPC_D2.Id, Amount = 3});
        dict.Add(PieceType.NPC_D4.Id, new PieceMatchDef {Next = PieceType.NPC_D.Id, Previous = PieceType.NPC_D3.Id, Amount = 3});
//        dict.Add(PieceType.NPC_D5.Id, new PieceMatchDef {Next = PieceType.NPC_D6.Id, Previous = PieceType.NPC_D4.Id, Amount = 3});
//        dict.Add(PieceType.NPC_D6.Id, new PieceMatchDef {Next = PieceType.NPC_D7.Id, Previous = PieceType.NPC_D5.Id, Amount = 3});
//        dict.Add(PieceType.NPC_D7.Id, new PieceMatchDef {Next = PieceType.NPC_D8.Id, Previous = PieceType.NPC_D6.Id, Amount = 3});
//        dict.Add(PieceType.NPC_D8.Id, new PieceMatchDef {Next = PieceType.NPC_D.Id, Previous = PieceType.NPC_D7.Id, Amount = 3});
        dict.Add(PieceType.NPC_D.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.NPC_D4.Id, Amount = -1});
        
        #endregion
        
        #region NPC_E
        
        dict.Add(PieceType.NPC_E1.Id, new PieceMatchDef {Next = PieceType.NPC_E2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.NPC_E2.Id, new PieceMatchDef {Next = PieceType.NPC_E3.Id, Previous = PieceType.NPC_E1.Id, Amount = 3});
        dict.Add(PieceType.NPC_E3.Id, new PieceMatchDef {Next = PieceType.NPC_E4.Id, Previous = PieceType.NPC_E2.Id, Amount = 3});
        dict.Add(PieceType.NPC_E4.Id, new PieceMatchDef {Next = PieceType.NPC_E5.Id, Previous = PieceType.NPC_E3.Id, Amount = 3});
        dict.Add(PieceType.NPC_E5.Id, new PieceMatchDef {Next = PieceType.NPC_E6.Id, Previous = PieceType.NPC_E4.Id, Amount = 3});
        dict.Add(PieceType.NPC_E6.Id, new PieceMatchDef {Next = PieceType.NPC_E7.Id, Previous = PieceType.NPC_E5.Id, Amount = 3});
        dict.Add(PieceType.NPC_E7.Id, new PieceMatchDef {Next = PieceType.NPC_E8.Id, Previous = PieceType.NPC_E6.Id, Amount = 3});
        dict.Add(PieceType.NPC_E8.Id, new PieceMatchDef {Next = PieceType.NPC_E.Id, Previous = PieceType.NPC_E7.Id, Amount = 3});
        dict.Add(PieceType.NPC_E.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.NPC_E8.Id, Amount = -1});
        
        #endregion

        #region NPC_F
        
        dict.Add(PieceType.NPC_F1.Id, new PieceMatchDef {Next = PieceType.NPC_F2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.NPC_F2.Id, new PieceMatchDef {Next = PieceType.NPC_F3.Id, Previous = PieceType.NPC_F1.Id, Amount = 3});
        dict.Add(PieceType.NPC_F3.Id, new PieceMatchDef {Next = PieceType.NPC_F4.Id, Previous = PieceType.NPC_F2.Id, Amount = 3});
        dict.Add(PieceType.NPC_F4.Id, new PieceMatchDef {Next = PieceType.NPC_F5.Id, Previous = PieceType.NPC_F3.Id, Amount = 3});
        dict.Add(PieceType.NPC_F5.Id, new PieceMatchDef {Next = PieceType.NPC_F6.Id, Previous = PieceType.NPC_F4.Id, Amount = 3});
        dict.Add(PieceType.NPC_F6.Id, new PieceMatchDef {Next = PieceType.NPC_F7.Id, Previous = PieceType.NPC_F5.Id, Amount = 3});
        dict.Add(PieceType.NPC_F7.Id, new PieceMatchDef {Next = PieceType.NPC_F8.Id, Previous = PieceType.NPC_F6.Id, Amount = 3});
        dict.Add(PieceType.NPC_F8.Id, new PieceMatchDef {Next = PieceType.NPC_F.Id, Previous = PieceType.NPC_F7.Id, Amount = 3});
        dict.Add(PieceType.NPC_F.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.NPC_F8.Id, Amount = -1});
        
        #endregion

        #region NPC_G
        
        dict.Add(PieceType.NPC_G1.Id, new PieceMatchDef {Next = PieceType.NPC_G2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.NPC_G2.Id, new PieceMatchDef {Next = PieceType.NPC_G3.Id, Previous = PieceType.NPC_G1.Id, Amount = 3});
        dict.Add(PieceType.NPC_G3.Id, new PieceMatchDef {Next = PieceType.NPC_G4.Id, Previous = PieceType.NPC_G2.Id, Amount = 3});
        dict.Add(PieceType.NPC_G4.Id, new PieceMatchDef {Next = PieceType.NPC_G5.Id, Previous = PieceType.NPC_G3.Id, Amount = 3});
        dict.Add(PieceType.NPC_G5.Id, new PieceMatchDef {Next = PieceType.NPC_G6.Id, Previous = PieceType.NPC_G4.Id, Amount = 3});
        dict.Add(PieceType.NPC_G6.Id, new PieceMatchDef {Next = PieceType.NPC_G.Id, Previous = PieceType.NPC_G5.Id, Amount = 3});
        dict.Add(PieceType.NPC_G.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.NPC_G6.Id, Amount = -1});
        
        #endregion
        
#endregion

#region Boosters
        
        dict.Add(PieceType.Boost_CR1.Id, new PieceMatchDef {Next = PieceType.Boost_CR2.Id, Previous = PieceType.None.Id,   Amount = 3});
        dict.Add(PieceType.Boost_CR2.Id, new PieceMatchDef {Next = PieceType.Boost_CR3.Id,  Previous = PieceType.Boost_CR1.Id, Amount = 3});
        dict.Add(PieceType.Boost_CR3.Id, new PieceMatchDef {Next = PieceType.Boost_CR.Id,  Previous = PieceType.Boost_CR2.Id, Amount = 3});
        dict.Add(PieceType.Boost_CR.Id,  new PieceMatchDef {Next = PieceType.None.Id,   Previous = PieceType.Boost_CR3.Id });
        
#endregion
        
#region Currencies
        
        dict.Add(PieceType.Mana1.Id, new PieceMatchDef {Next = PieceType.Mana2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.Mana2.Id, new PieceMatchDef {Next = PieceType.Mana3.Id, Previous = PieceType.Mana1.Id, Amount = 3});
        dict.Add(PieceType.Mana3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Mana2.Id});
        /*dict.Add(PieceType.Mana3.Id, new PieceMatchDef {Next = PieceType.Mana4.Id, Previous = PieceType.Mana2.Id, Amount = 3});
        dict.Add(PieceType.Mana4.Id, new PieceMatchDef {Next = PieceType.Mana5.Id, Previous = PieceType.Mana3.Id, Amount = 3});
        dict.Add(PieceType.Mana5.Id, new PieceMatchDef {Next = PieceType.Mana6.Id, Previous = PieceType.Mana4.Id, Amount = 3});
        dict.Add(PieceType.Mana6.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Mana5.Id});*/
        
        dict.Add(PieceType.Soft1.Id, new PieceMatchDef {Next = PieceType.Soft2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.Soft2.Id, new PieceMatchDef {Next = PieceType.Soft3.Id, Previous = PieceType.Soft1.Id, Amount = 3});
        dict.Add(PieceType.Soft3.Id, new PieceMatchDef {Next = PieceType.Soft4.Id, Previous = PieceType.Soft2.Id, Amount = 3});
        dict.Add(PieceType.Soft4.Id, new PieceMatchDef {Next = PieceType.Soft5.Id, Previous = PieceType.Soft3.Id, Amount = 3});
        dict.Add(PieceType.Soft5.Id, new PieceMatchDef {Next = PieceType.Soft6.Id, Previous = PieceType.Soft4.Id, Amount = 3});
        dict.Add(PieceType.Soft6.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Soft5.Id});
        
        dict.Add(PieceType.Hard1.Id, new PieceMatchDef {Next = PieceType.Hard2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.Hard2.Id, new PieceMatchDef {Next = PieceType.Hard3.Id, Previous = PieceType.Hard1.Id, Amount = 3});
        dict.Add(PieceType.Hard3.Id, new PieceMatchDef {Next = PieceType.Hard4.Id, Previous = PieceType.Hard2.Id, Amount = 3});
        dict.Add(PieceType.Hard4.Id, new PieceMatchDef {Next = PieceType.Hard5.Id, Previous = PieceType.Hard3.Id, Amount = 3});
        dict.Add(PieceType.Hard5.Id, new PieceMatchDef {Next = PieceType.Hard6.Id, Previous = PieceType.Hard4.Id, Amount = 3});
        dict.Add(PieceType.Hard6.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Hard5.Id});
        
#endregion
        
#region Obstacles
        
        dict.Add(PieceType.OB1_TT.Id, new PieceMatchDef {Next = PieceType.OB2_TT.Id, Previous = PieceType.None.Id, Amount = -1});
        dict.Add(PieceType.OB2_TT.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OB1_TT.Id, Amount = -1});
        
        dict.Add(PieceType.OB1_A.Id, new PieceMatchDef {Next = PieceType.OB2_A.Id, Previous = PieceType.None.Id, Amount = -1});
        dict.Add(PieceType.OB2_A.Id, new PieceMatchDef {Next = PieceType.OB3_A.Id, Previous = PieceType.OB1_A.Id, Amount = -1});
        dict.Add(PieceType.OB3_A.Id, new PieceMatchDef {Next = PieceType.OB4_A.Id, Previous = PieceType.OB2_A.Id, Amount = -1});
        dict.Add(PieceType.OB4_A.Id, new PieceMatchDef {Next = PieceType.OB5_A.Id, Previous = PieceType.OB3_A.Id, Amount = -1});
        dict.Add(PieceType.OB5_A.Id, new PieceMatchDef {Next = PieceType.OB6_A.Id, Previous = PieceType.OB4_A.Id, Amount = -1});
        dict.Add(PieceType.OB6_A.Id, new PieceMatchDef {Next = PieceType.OB7_A.Id, Previous = PieceType.OB5_A.Id, Amount = -1});
        dict.Add(PieceType.OB7_A.Id, new PieceMatchDef {Next = PieceType.OB8_A.Id, Previous = PieceType.OB6_A.Id, Amount = -1});
        dict.Add(PieceType.OB8_A.Id, new PieceMatchDef {Next = PieceType.OB9_A.Id, Previous = PieceType.OB7_A.Id, Amount = -1});
        dict.Add(PieceType.OB9_A.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OB8_A.Id, Amount = -1});
        
        dict.Add(PieceType.OB1_E.Id, new PieceMatchDef {Next = PieceType.OB2_E.Id, Previous = PieceType.None.Id, Amount = -1});
        dict.Add(PieceType.OB2_E.Id, new PieceMatchDef {Next = PieceType.OB3_E.Id, Previous = PieceType.OB1_E.Id, Amount = -1});
        dict.Add(PieceType.OB3_E.Id, new PieceMatchDef {Next = PieceType.OB4_E.Id, Previous = PieceType.OB2_E.Id, Amount = -1});
        dict.Add(PieceType.OB4_E.Id, new PieceMatchDef {Next = PieceType.OB5_E.Id, Previous = PieceType.OB3_E.Id, Amount = -1});
        dict.Add(PieceType.OB5_E.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OB4_E.Id, Amount = -1});
        
#endregion
        
#region Chests
        
        dict.Add(PieceType.SK1_PR.Id, new PieceMatchDef {Next = PieceType.SK2_PR.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.SK2_PR.Id, new PieceMatchDef {Next = PieceType.SK3_PR.Id, Previous = PieceType.SK1_PR.Id, Amount = 3});
        dict.Add(PieceType.SK3_PR.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.SK2_PR.Id});
        
        dict.Add(PieceType.CH1_A.Id, new PieceMatchDef {Next = PieceType.CH2_A.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_A.Id, new PieceMatchDef {Next = PieceType.CH3_A.Id, Previous = PieceType.CH1_A.Id, Amount = 3});
        dict.Add(PieceType.CH3_A.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_A.Id});
        
        dict.Add(PieceType.CH1_B.Id, new PieceMatchDef {Next = PieceType.CH2_B.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_B.Id, new PieceMatchDef {Next = PieceType.CH3_B.Id, Previous = PieceType.CH1_B.Id, Amount = 3});
        dict.Add(PieceType.CH3_B.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_B.Id});
        
        dict.Add(PieceType.CH1_C.Id, new PieceMatchDef {Next = PieceType.CH2_C.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_C.Id, new PieceMatchDef {Next = PieceType.CH3_C.Id, Previous = PieceType.CH1_C.Id, Amount = 3});
        dict.Add(PieceType.CH3_C.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_C.Id});
        
        dict.Add(PieceType.CH1_D.Id, new PieceMatchDef {Next = PieceType.CH2_D.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_D.Id, new PieceMatchDef {Next = PieceType.CH3_D.Id, Previous = PieceType.CH1_D.Id, Amount = 3});
        dict.Add(PieceType.CH3_D.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_D.Id});
        
        dict.Add(PieceType.CH1_E.Id, new PieceMatchDef {Next = PieceType.CH2_E.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_E.Id, new PieceMatchDef {Next = PieceType.CH3_E.Id, Previous = PieceType.CH1_E.Id, Amount = 3});
        dict.Add(PieceType.CH3_E.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_E.Id});
        
        dict.Add(PieceType.CH1_F.Id, new PieceMatchDef {Next = PieceType.CH2_F.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_F.Id, new PieceMatchDef {Next = PieceType.CH3_F.Id, Previous = PieceType.CH1_F.Id, Amount = 3});
        dict.Add(PieceType.CH3_F.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_F.Id});
        
        dict.Add(PieceType.CH1_G.Id, new PieceMatchDef {Next = PieceType.CH2_G.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.CH2_G.Id, new PieceMatchDef {Next = PieceType.CH3_G.Id, Previous = PieceType.CH1_G.Id, Amount = 3});
        dict.Add(PieceType.CH3_G.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.CH2_G.Id});
        
#endregion
        
#region Simple Pieces
        
        #region A
        
        dict.Add(PieceType.A1.Id, new PieceMatchDef {Next = PieceType.A2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.A2.Id, new PieceMatchDef {Next = PieceType.A3Fake.Id, Previous = PieceType.A1.Id, Amount = 3});
        dict.Add(PieceType.A3Fake.Id, new PieceMatchDef {Next = PieceType.A3.Id, Previous = PieceType.A2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.A3.Id, new PieceMatchDef {Next = PieceType.A4Fake.Id, Previous = PieceType.A2.Id, Amount = 3});
        dict.Add(PieceType.A4Fake.Id, new PieceMatchDef {Next = PieceType.A4.Id, Previous = PieceType.A3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.A4.Id, new PieceMatchDef {Next = PieceType.A5Fake.Id, Previous = PieceType.A3.Id, Amount = 3});
        dict.Add(PieceType.A5Fake.Id, new PieceMatchDef {Next = PieceType.A5.Id, Previous = PieceType.A4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.A5.Id, new PieceMatchDef {Next = PieceType.A6Fake.Id, Previous = PieceType.A4.Id, Amount = 3});
        dict.Add(PieceType.A6Fake.Id, new PieceMatchDef {Next = PieceType.A6.Id, Previous = PieceType.A5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.A6.Id, new PieceMatchDef {Next = PieceType.A7Fake.Id, Previous = PieceType.A5.Id, Amount = 3});
        dict.Add(PieceType.A7Fake.Id, new PieceMatchDef {Next = PieceType.A7.Id, Previous = PieceType.A6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.A7.Id, new PieceMatchDef {Next = PieceType.A8Fake.Id, Previous = PieceType.A6.Id, Amount = 3});
        dict.Add(PieceType.A8Fake.Id, new PieceMatchDef {Next = PieceType.A8.Id, Previous = PieceType.A7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.A8.Id, new PieceMatchDef {Next = PieceType.A9Fake.Id, Previous = PieceType.A7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.A9Fake.Id, PieceType.A9.Id, PieceType.A8.Id);
        
        dict.Add(PieceType.A9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.A8.Id});
        
        #endregion
        
        #region B
        
        dict.Add(PieceType.B1.Id, new PieceMatchDef {Next = PieceType.B2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.B2.Id, new PieceMatchDef {Next = PieceType.B3Fake.Id, Previous = PieceType.B1.Id, Amount = 3});
        dict.Add(PieceType.B3Fake.Id, new PieceMatchDef {Next = PieceType.B3.Id, Previous = PieceType.B2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B3.Id, new PieceMatchDef {Next = PieceType.B4Fake.Id, Previous = PieceType.B2.Id, Amount = 3});
        dict.Add(PieceType.B4Fake.Id, new PieceMatchDef {Next = PieceType.B4.Id, Previous = PieceType.B3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B4.Id, new PieceMatchDef {Next = PieceType.B5Fake.Id, Previous = PieceType.B3.Id, Amount = 3});
        dict.Add(PieceType.B5Fake.Id, new PieceMatchDef {Next = PieceType.B5.Id, Previous = PieceType.B4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B5.Id, new PieceMatchDef {Next = PieceType.B6Fake.Id, Previous = PieceType.B4.Id, Amount = 3});
        dict.Add(PieceType.B6Fake.Id, new PieceMatchDef {Next = PieceType.B6.Id, Previous = PieceType.B5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B6.Id, new PieceMatchDef {Next = PieceType.B7Fake.Id, Previous = PieceType.B5.Id, Amount = 3});
        dict.Add(PieceType.B7Fake.Id, new PieceMatchDef {Next = PieceType.B7.Id, Previous = PieceType.B6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B7.Id, new PieceMatchDef {Next = PieceType.B8Fake.Id, Previous = PieceType.B6.Id, Amount = 3});
        dict.Add(PieceType.B8Fake.Id, new PieceMatchDef {Next = PieceType.B8.Id, Previous = PieceType.B7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B8.Id, new PieceMatchDef {Next = PieceType.B9Fake.Id, Previous = PieceType.B7.Id, Amount = 3});
        dict.Add(PieceType.B9Fake.Id, new PieceMatchDef {Next = PieceType.B9.Id, Previous = PieceType.B8.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B9.Id, new PieceMatchDef {Next = PieceType.B10Fake.Id, Previous = PieceType.B8.Id, Amount = 3});
        dict.Add(PieceType.B10Fake.Id, new PieceMatchDef {Next = PieceType.B10.Id, Previous = PieceType.B9.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.B10.Id, new PieceMatchDef {Next = PieceType.B11Fake.Id, Previous = PieceType.B9.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.B11Fake.Id, PieceType.B11.Id, PieceType.B10.Id);
        
        dict.Add(PieceType.B11.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.B10.Id});
        
        #endregion
        
        #region C
        
        dict.Add(PieceType.C1.Id, new PieceMatchDef {Next = PieceType.C2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.C2.Id, new PieceMatchDef {Next = PieceType.C3Fake.Id, Previous = PieceType.C1.Id, Amount = 3});
        dict.Add(PieceType.C3Fake.Id, new PieceMatchDef {Next = PieceType.C3.Id, Previous = PieceType.C2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.C3.Id, new PieceMatchDef {Next = PieceType.C4Fake.Id, Previous = PieceType.C2.Id, Amount = 3});
        dict.Add(PieceType.C4Fake.Id, new PieceMatchDef {Next = PieceType.C4.Id, Previous = PieceType.C3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.C4.Id, new PieceMatchDef {Next = PieceType.C5Fake.Id, Previous = PieceType.C3.Id, Amount = 3});
        dict.Add(PieceType.C5Fake.Id, new PieceMatchDef {Next = PieceType.C5.Id, Previous = PieceType.C4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.C5.Id, new PieceMatchDef {Next = PieceType.C6Fake.Id, Previous = PieceType.C4.Id, Amount = 3});
        dict.Add(PieceType.C6Fake.Id, new PieceMatchDef {Next = PieceType.C6.Id, Previous = PieceType.C5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.C6.Id, new PieceMatchDef {Next = PieceType.C7Fake.Id, Previous = PieceType.C5.Id, Amount = 3});
        dict.Add(PieceType.C7Fake.Id, new PieceMatchDef {Next = PieceType.C7.Id, Previous = PieceType.C6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.C7.Id, new PieceMatchDef {Next = PieceType.C8Fake.Id, Previous = PieceType.C6.Id, Amount = 3});
        dict.Add(PieceType.C8Fake.Id, new PieceMatchDef {Next = PieceType.C8.Id, Previous = PieceType.C7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.C8.Id, new PieceMatchDef {Next = PieceType.C9Fake.Id, Previous = PieceType.C7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.C9Fake.Id, PieceType.C9.Id, PieceType.C8.Id);
        
        dict.Add(PieceType.C9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.C8.Id});
        
        #endregion
        
        #region D
        
        dict.Add(PieceType.D1.Id, new PieceMatchDef {Next = PieceType.D2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.D2.Id, new PieceMatchDef {Next = PieceType.D3Fake.Id, Previous = PieceType.D1.Id, Amount = 3});
        dict.Add(PieceType.D3Fake.Id, new PieceMatchDef {Next = PieceType.D3.Id, Previous = PieceType.D2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.D3.Id, new PieceMatchDef {Next = PieceType.D4Fake.Id, Previous = PieceType.D2.Id, Amount = 3});
        dict.Add(PieceType.D4Fake.Id, new PieceMatchDef {Next = PieceType.D4.Id, Previous = PieceType.D3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.D4.Id, new PieceMatchDef {Next = PieceType.D5Fake.Id, Previous = PieceType.D3.Id, Amount = 3});
        dict.Add(PieceType.D5Fake.Id, new PieceMatchDef {Next = PieceType.D5.Id, Previous = PieceType.D4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.D5.Id, new PieceMatchDef {Next = PieceType.D6Fake.Id, Previous = PieceType.D4.Id, Amount = 3});
        dict.Add(PieceType.D6Fake.Id, new PieceMatchDef {Next = PieceType.D6.Id, Previous = PieceType.D5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.D6.Id, new PieceMatchDef {Next = PieceType.D7Fake.Id, Previous = PieceType.D5.Id, Amount = 3});
        dict.Add(PieceType.D7Fake.Id, new PieceMatchDef {Next = PieceType.D7.Id, Previous = PieceType.D6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.D7.Id, new PieceMatchDef {Next = PieceType.D8Fake.Id, Previous = PieceType.D6.Id, Amount = 3});
        dict.Add(PieceType.D8Fake.Id, new PieceMatchDef {Next = PieceType.D8.Id, Previous = PieceType.D7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.D8.Id, new PieceMatchDef {Next = PieceType.D9Fake.Id, Previous = PieceType.D7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.D9Fake.Id, PieceType.D9.Id, PieceType.D8.Id);
        
        dict.Add(PieceType.D9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.D8.Id});
        
        #endregion
        
        #region E
        
        dict.Add(PieceType.E1.Id,     new PieceMatchDef {Next = PieceType.E2.Id,     Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.E2.Id,     new PieceMatchDef {Next = PieceType.E3Fake.Id, Previous = PieceType.E1.Id, Amount = 3});
        dict.Add(PieceType.E3Fake.Id, new PieceMatchDef {Next = PieceType.E3.Id,     Previous = PieceType.E2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.E3.Id,     new PieceMatchDef {Next = PieceType.E4Fake.Id, Previous = PieceType.E2.Id, Amount = 3});
        dict.Add(PieceType.E4Fake.Id, new PieceMatchDef {Next = PieceType.E4.Id,     Previous = PieceType.E3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.E4.Id,     new PieceMatchDef {Next = PieceType.E5Fake.Id, Previous = PieceType.E3.Id, Amount = 3});
        dict.Add(PieceType.E5Fake.Id, new PieceMatchDef {Next = PieceType.E5.Id,     Previous = PieceType.E4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.E5.Id,     new PieceMatchDef {Next = PieceType.E6Fake.Id, Previous = PieceType.E4.Id, Amount = 3});
        dict.Add(PieceType.E6Fake.Id, new PieceMatchDef {Next = PieceType.E6.Id,     Previous = PieceType.E5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.E6.Id,     new PieceMatchDef {Next = PieceType.E7Fake.Id, Previous = PieceType.E5.Id, Amount = 3});
        dict.Add(PieceType.E7Fake.Id, new PieceMatchDef {Next = PieceType.E7.Id,     Previous = PieceType.E6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.E7.Id,     new PieceMatchDef {Next = PieceType.E8Fake.Id, Previous = PieceType.E6.Id, Amount = 3});
        dict.Add(PieceType.E8Fake.Id, new PieceMatchDef {Next = PieceType.E8.Id,     Previous = PieceType.E7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.E8.Id,     new PieceMatchDef {Next = PieceType.E9Fake.Id, Previous = PieceType.E7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.E9Fake.Id, PieceType.E9.Id, PieceType.E8.Id);
        
        dict.Add(PieceType.E9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.E8.Id});
        
        #endregion
        
        #region F
        
        dict.Add(PieceType.F1.Id,     new PieceMatchDef {Next = PieceType.F2.Id,     Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.F2.Id,     new PieceMatchDef {Next = PieceType.F3Fake.Id, Previous = PieceType.F1.Id, Amount = 3});
        dict.Add(PieceType.F3Fake.Id, new PieceMatchDef {Next = PieceType.F3.Id,     Previous = PieceType.F2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.F3.Id,     new PieceMatchDef {Next = PieceType.F4Fake.Id, Previous = PieceType.F2.Id, Amount = 3});
        dict.Add(PieceType.F4Fake.Id, new PieceMatchDef {Next = PieceType.F4.Id,     Previous = PieceType.F3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.F4.Id,     new PieceMatchDef {Next = PieceType.F5Fake.Id, Previous = PieceType.F3.Id, Amount = 3});
        dict.Add(PieceType.F5Fake.Id, new PieceMatchDef {Next = PieceType.F5.Id,     Previous = PieceType.F4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.F5.Id,     new PieceMatchDef {Next = PieceType.F6Fake.Id, Previous = PieceType.F4.Id, Amount = 3});
        dict.Add(PieceType.F6Fake.Id, new PieceMatchDef {Next = PieceType.F6.Id,     Previous = PieceType.F5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.F6.Id,     new PieceMatchDef {Next = PieceType.F7Fake.Id, Previous = PieceType.F5.Id, Amount = 3});
        dict.Add(PieceType.F7Fake.Id, new PieceMatchDef {Next = PieceType.F7.Id,     Previous = PieceType.F6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.F7.Id,     new PieceMatchDef {Next = PieceType.F8Fake.Id, Previous = PieceType.F6.Id, Amount = 3});
        dict.Add(PieceType.F8Fake.Id, new PieceMatchDef {Next = PieceType.F8.Id,     Previous = PieceType.F7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.F8.Id,     new PieceMatchDef {Next = PieceType.F9Fake.Id, Previous = PieceType.F7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.F9Fake.Id, PieceType.F9.Id, PieceType.F8.Id);
        
        dict.Add(PieceType.F9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.F8.Id});
        
        #endregion
        
        #region G
        
        dict.Add(PieceType.G1.Id,     new PieceMatchDef {Next = PieceType.G2.Id,     Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.G2.Id,     new PieceMatchDef {Next = PieceType.G3Fake.Id, Previous = PieceType.G1.Id, Amount = 3});
        dict.Add(PieceType.G3Fake.Id, new PieceMatchDef {Next = PieceType.G3.Id,     Previous = PieceType.G2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.G3.Id,     new PieceMatchDef {Next = PieceType.G4Fake.Id, Previous = PieceType.G2.Id, Amount = 3});
        dict.Add(PieceType.G4Fake.Id, new PieceMatchDef {Next = PieceType.G4.Id,     Previous = PieceType.G3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.G4.Id,     new PieceMatchDef {Next = PieceType.G5Fake.Id, Previous = PieceType.G3.Id, Amount = 3});
        dict.Add(PieceType.G5Fake.Id, new PieceMatchDef {Next = PieceType.G5.Id,     Previous = PieceType.G4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.G5.Id,     new PieceMatchDef {Next = PieceType.G6Fake.Id, Previous = PieceType.G4.Id, Amount = 3});
        dict.Add(PieceType.G6Fake.Id, new PieceMatchDef {Next = PieceType.G6.Id,     Previous = PieceType.G5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.G6.Id,     new PieceMatchDef {Next = PieceType.G7Fake.Id, Previous = PieceType.G5.Id, Amount = 3});
        dict.Add(PieceType.G7Fake.Id, new PieceMatchDef {Next = PieceType.G7.Id,     Previous = PieceType.G6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.G7.Id,     new PieceMatchDef {Next = PieceType.G8Fake.Id, Previous = PieceType.G6.Id, Amount = 3});
        dict.Add(PieceType.G8Fake.Id, new PieceMatchDef {Next = PieceType.G8.Id,     Previous = PieceType.G7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.G8.Id,     new PieceMatchDef {Next = PieceType.G9Fake.Id, Previous = PieceType.G7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.G9Fake.Id, PieceType.G9.Id, PieceType.G8.Id);
        
        dict.Add(PieceType.G9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.G8.Id});
        
        #endregion
        
        #region H
        
        dict.Add(PieceType.H1.Id, new PieceMatchDef {Next = PieceType.H2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.H2.Id, new PieceMatchDef {Next = PieceType.H3Fake.Id, Previous = PieceType.H1.Id, Amount = 3});
        dict.Add(PieceType.H3Fake.Id, new PieceMatchDef {Next = PieceType.H3.Id, Previous = PieceType.H2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.H3.Id, new PieceMatchDef {Next = PieceType.H4Fake.Id, Previous = PieceType.H2.Id, Amount = 3});
        dict.Add(PieceType.H4Fake.Id, new PieceMatchDef {Next = PieceType.H4.Id, Previous = PieceType.H3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.H4.Id, new PieceMatchDef {Next = PieceType.H5Fake.Id, Previous = PieceType.H3.Id, Amount = 3});
        dict.Add(PieceType.H5Fake.Id, new PieceMatchDef {Next = PieceType.H5.Id, Previous = PieceType.H4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.H5.Id, new PieceMatchDef {Next = PieceType.H6Fake.Id, Previous = PieceType.H4.Id, Amount = 3});
        dict.Add(PieceType.H6Fake.Id, new PieceMatchDef {Next = PieceType.H6.Id, Previous = PieceType.H5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.H6.Id, new PieceMatchDef {Next = PieceType.H7Fake.Id, Previous = PieceType.H5.Id, Amount = 3});
        dict.Add(PieceType.H7Fake.Id, new PieceMatchDef {Next = PieceType.H7.Id, Previous = PieceType.H6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.H7.Id, new PieceMatchDef {Next = PieceType.H8Fake.Id, Previous = PieceType.H6.Id, Amount = 3});
        dict.Add(PieceType.H8Fake.Id, new PieceMatchDef {Next = PieceType.H8.Id, Previous = PieceType.H7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.H8.Id, new PieceMatchDef {Next = PieceType.H9Fake.Id, Previous = PieceType.H7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.H9Fake.Id, PieceType.H9.Id, PieceType.H8.Id);
        
        dict.Add(PieceType.H9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.H8.Id});
        
        #endregion
        
        #region I
        
        dict.Add(PieceType.I1.Id, new PieceMatchDef {Next = PieceType.I2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.I2.Id, new PieceMatchDef {Next = PieceType.I3Fake.Id, Previous = PieceType.I1.Id, Amount = 3});
        dict.Add(PieceType.I3Fake.Id, new PieceMatchDef {Next = PieceType.I3.Id, Previous = PieceType.I2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.I3.Id, new PieceMatchDef {Next = PieceType.I4Fake.Id, Previous = PieceType.I2.Id, Amount = 3});
        dict.Add(PieceType.I4Fake.Id, new PieceMatchDef {Next = PieceType.I4.Id, Previous = PieceType.I3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.I4.Id, new PieceMatchDef {Next = PieceType.I5Fake.Id, Previous = PieceType.I3.Id, Amount = 3});
        dict.Add(PieceType.I5Fake.Id, new PieceMatchDef {Next = PieceType.I5.Id, Previous = PieceType.I4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.I5.Id, new PieceMatchDef {Next = PieceType.I6Fake.Id, Previous = PieceType.I4.Id, Amount = 3});
        dict.Add(PieceType.I6Fake.Id, new PieceMatchDef {Next = PieceType.I6.Id, Previous = PieceType.I5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.I6.Id, new PieceMatchDef {Next = PieceType.I7Fake.Id, Previous = PieceType.I5.Id, Amount = 3});
        dict.Add(PieceType.I7Fake.Id, new PieceMatchDef {Next = PieceType.I7.Id, Previous = PieceType.I6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.I7.Id, new PieceMatchDef {Next = PieceType.I8Fake.Id, Previous = PieceType.I6.Id, Amount = 3});
        dict.Add(PieceType.I8Fake.Id, new PieceMatchDef {Next = PieceType.I8.Id, Previous = PieceType.I7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.I8.Id, new PieceMatchDef {Next = PieceType.I9Fake.Id, Previous = PieceType.I7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.I9Fake.Id, PieceType.I9.Id, PieceType.I8.Id);
        
        dict.Add(PieceType.I9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.I8.Id});
        
        #endregion
        
        #region J
        
        dict.Add(PieceType.J1.Id, new PieceMatchDef {Next = PieceType.J2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.J2.Id, new PieceMatchDef {Next = PieceType.J3Fake.Id, Previous = PieceType.J1.Id, Amount = 3});
        dict.Add(PieceType.J3Fake.Id, new PieceMatchDef {Next = PieceType.J3.Id, Previous = PieceType.J2.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.J3.Id, new PieceMatchDef {Next = PieceType.J4Fake.Id, Previous = PieceType.J2.Id, Amount = 3});
        dict.Add(PieceType.J4Fake.Id, new PieceMatchDef {Next = PieceType.J4.Id, Previous = PieceType.J3.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.J4.Id, new PieceMatchDef {Next = PieceType.J5Fake.Id, Previous = PieceType.J3.Id, Amount = 3});
        dict.Add(PieceType.J5Fake.Id, new PieceMatchDef {Next = PieceType.J5.Id, Previous = PieceType.J4.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.J5.Id, new PieceMatchDef {Next = PieceType.J6Fake.Id, Previous = PieceType.J4.Id, Amount = 3});
        dict.Add(PieceType.J6Fake.Id, new PieceMatchDef {Next = PieceType.J6.Id, Previous = PieceType.J5.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.J6.Id, new PieceMatchDef {Next = PieceType.J7Fake.Id, Previous = PieceType.J5.Id, Amount = 3});
        dict.Add(PieceType.J7Fake.Id, new PieceMatchDef {Next = PieceType.J7.Id, Previous = PieceType.J6.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.J7.Id, new PieceMatchDef {Next = PieceType.J8Fake.Id, Previous = PieceType.J6.Id, Amount = 3});
        dict.Add(PieceType.J8Fake.Id, new PieceMatchDef {Next = PieceType.J8.Id, Previous = PieceType.J7.Id, Amount = 1, IsIgnore = true});
        dict.Add(PieceType.J8.Id, new PieceMatchDef {Next = PieceType.J9Fake.Id, Previous = PieceType.J7.Id, Amount = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.J9Fake.Id, PieceType.J9.Id, PieceType.J8.Id);
        
        dict.Add(PieceType.J9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.J8.Id});
        
        #endregion
        
        
        
#endregion
        
#region Reproduction Pieces
        
        #region PR_A
        
        dict.Add(PieceType.PR_A1.Id, new PieceMatchDef {Next = PieceType.PR_A2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_A2.Id, new PieceMatchDef {Next = PieceType.PR_A3.Id, Previous = PieceType.PR_A1.Id, Amount = 3});
        dict.Add(PieceType.PR_A3.Id, new PieceMatchDef {Next = PieceType.PR_A4.Id, Previous = PieceType.PR_A2.Id, Amount = 3});
        dict.Add(PieceType.PR_A4.Id, new PieceMatchDef {Next = PieceType.PR_A5.Id, Previous = PieceType.PR_A3.Id, Amount = -1});
        dict.Add(PieceType.PR_A5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_A4.Id});
        
        #endregion
        
        #region PR_B
        
        dict.Add(PieceType.PR_B1.Id, new PieceMatchDef {Next = PieceType.PR_B2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_B2.Id, new PieceMatchDef {Next = PieceType.PR_B3.Id, Previous = PieceType.PR_B1.Id, Amount = 3});
        dict.Add(PieceType.PR_B3.Id, new PieceMatchDef {Next = PieceType.PR_B4.Id, Previous = PieceType.PR_B2.Id, Amount = 3});
        dict.Add(PieceType.PR_B4.Id, new PieceMatchDef {Next = PieceType.PR_B5.Id, Previous = PieceType.PR_B3.Id, Amount = -1});
        dict.Add(PieceType.PR_B5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_B4.Id});
        
        #endregion
        
        #region PR_C
        
        dict.Add(PieceType.PR_C1.Id, new PieceMatchDef {Next = PieceType.PR_C2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_C2.Id, new PieceMatchDef {Next = PieceType.PR_C3.Id, Previous = PieceType.PR_C1.Id, Amount = 3});
        dict.Add(PieceType.PR_C3.Id, new PieceMatchDef {Next = PieceType.PR_C4.Id, Previous = PieceType.PR_C2.Id, Amount = 3});
        dict.Add(PieceType.PR_C4.Id, new PieceMatchDef {Next = PieceType.PR_C5.Id, Previous = PieceType.PR_C3.Id, Amount = -1});
        dict.Add(PieceType.PR_C5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_C4.Id});
        
        #endregion
        
        #region PR_D
        
        dict.Add(PieceType.PR_D1.Id, new PieceMatchDef {Next = PieceType.PR_D2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_D2.Id, new PieceMatchDef {Next = PieceType.PR_D3.Id, Previous = PieceType.PR_D1.Id, Amount = 3});
        dict.Add(PieceType.PR_D3.Id, new PieceMatchDef {Next = PieceType.PR_D4.Id, Previous = PieceType.PR_D2.Id, Amount = 3});
        dict.Add(PieceType.PR_D4.Id, new PieceMatchDef {Next = PieceType.PR_D5.Id, Previous = PieceType.PR_D3.Id, Amount = -1});
        dict.Add(PieceType.PR_D5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_D4.Id});
        
        #endregion
        
        #region PR_E
        
        dict.Add(PieceType.PR_E1.Id, new PieceMatchDef {Next = PieceType.PR_E2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_E2.Id, new PieceMatchDef {Next = PieceType.PR_E3.Id, Previous = PieceType.PR_E1.Id, Amount = 3});
        dict.Add(PieceType.PR_E3.Id, new PieceMatchDef {Next = PieceType.PR_E4.Id, Previous = PieceType.PR_E2.Id, Amount = 3});
        dict.Add(PieceType.PR_E4.Id, new PieceMatchDef {Next = PieceType.PR_E5.Id, Previous = PieceType.PR_E3.Id, Amount = -1});
        dict.Add(PieceType.PR_E5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_E4.Id});
        
        #endregion
        
        #region PR_F
        
        dict.Add(PieceType.PR_F1.Id, new PieceMatchDef {Next = PieceType.PR_F2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_F2.Id, new PieceMatchDef {Next = PieceType.PR_F3.Id, Previous = PieceType.PR_F1.Id, Amount = 3});
        dict.Add(PieceType.PR_F3.Id, new PieceMatchDef {Next = PieceType.PR_F4.Id, Previous = PieceType.PR_F2.Id, Amount = 3});
        dict.Add(PieceType.PR_F4.Id, new PieceMatchDef {Next = PieceType.PR_F5.Id, Previous = PieceType.PR_F3.Id, Amount = -1});
        dict.Add(PieceType.PR_F5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_F4.Id});
        
        #endregion
        
        #region PR_G
        
        dict.Add(PieceType.PR_G1.Id, new PieceMatchDef {Next = PieceType.PR_G2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_G2.Id, new PieceMatchDef {Next = PieceType.PR_G3.Id, Previous = PieceType.PR_G1.Id, Amount = 3});
        dict.Add(PieceType.PR_G3.Id, new PieceMatchDef {Next = PieceType.PR_G4.Id, Previous = PieceType.PR_G2.Id, Amount = 3});
        dict.Add(PieceType.PR_G4.Id, new PieceMatchDef {Next = PieceType.PR_G5.Id, Previous = PieceType.PR_G3.Id, Amount = -1});
        dict.Add(PieceType.PR_G5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_G4.Id});
        
        #endregion
        
#endregion
        
        return dict;
    }

    private Dictionary<int, PieceMatchDef> AddFakeMulticellularPiece(Dictionary<int, PieceMatchDef> dict, int id, int next, int previous)
    {
        dict.Add(id, new PieceMatchDef {Next = next, Previous = previous, Amount = 1, IsIgnore = true, Pattern = new List<List<int>>
        {
            new List<int> { previous, previous },
            new List<int> { previous, previous }
        }});
        
        return dict;
    }
}