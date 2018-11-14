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
        
#region Boosters
        
        dict.Add(PieceType.Boost_CR1.Id, new PieceMatchDef {Next = PieceType.Boost_CR2.Id, Previous = PieceType.None.Id,   Amount = 3});
        dict.Add(PieceType.Boost_CR2.Id, new PieceMatchDef {Next = PieceType.Boost_CR3.Id,  Previous = PieceType.Boost_CR1.Id, Amount = 3});
        dict.Add(PieceType.Boost_CR3.Id, new PieceMatchDef {Next = PieceType.Boost_CR.Id,  Previous = PieceType.Boost_CR2.Id, Amount = 3});
        dict.Add(PieceType.Boost_CR.Id,  new PieceMatchDef {Next = PieceType.None.Id,   Previous = PieceType.Boost_CR3.Id });
        
#endregion
        
#region Currencies
        
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
        
#endregion
        
#region Chests
        
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
        
#endregion
        
#region Reproduction Pieces
        
        #region PR_A
        
        dict.Add(PieceType.PR_A1.Id, new PieceMatchDef {Next = PieceType.PR_A2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_A2.Id, new PieceMatchDef {Next = PieceType.PR_A3.Id, Previous = PieceType.PR_A1.Id, Amount = 3});
        dict.Add(PieceType.PR_A3.Id, new PieceMatchDef {Next = PieceType.PR_A4.Id, Previous = PieceType.PR_A2.Id, Amount = 3});
        dict.Add(PieceType.PR_A4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_A3.Id});
        
        #endregion
        
        #region PR_B
        
        dict.Add(PieceType.PR_B1.Id, new PieceMatchDef {Next = PieceType.PR_B2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_B2.Id, new PieceMatchDef {Next = PieceType.PR_B3.Id, Previous = PieceType.PR_B1.Id, Amount = 3});
        dict.Add(PieceType.PR_B3.Id, new PieceMatchDef {Next = PieceType.PR_B4.Id, Previous = PieceType.PR_B2.Id, Amount = 3});
        dict.Add(PieceType.PR_B4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_B3.Id});
        
        #endregion
        
        #region PR_C
        
        dict.Add(PieceType.PR_C1.Id, new PieceMatchDef {Next = PieceType.PR_C2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_C2.Id, new PieceMatchDef {Next = PieceType.PR_C3.Id, Previous = PieceType.PR_C1.Id, Amount = 3});
        dict.Add(PieceType.PR_C3.Id, new PieceMatchDef {Next = PieceType.PR_C4.Id, Previous = PieceType.PR_C2.Id, Amount = 3});
        dict.Add(PieceType.PR_C4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_C3.Id});
        
        #endregion
        
        #region PR_D
        
        dict.Add(PieceType.PR_D1.Id, new PieceMatchDef {Next = PieceType.PR_D2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_D2.Id, new PieceMatchDef {Next = PieceType.PR_D3.Id, Previous = PieceType.PR_D1.Id, Amount = 3});
        dict.Add(PieceType.PR_D3.Id, new PieceMatchDef {Next = PieceType.PR_D4.Id, Previous = PieceType.PR_D2.Id, Amount = 3});
        dict.Add(PieceType.PR_D4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_D3.Id});
        
        #endregion
        
        #region PR_E
        
        dict.Add(PieceType.PR_E1.Id, new PieceMatchDef {Next = PieceType.PR_E2.Id, Previous = PieceType.None.Id, Amount = 3});
        dict.Add(PieceType.PR_E2.Id, new PieceMatchDef {Next = PieceType.PR_E3.Id, Previous = PieceType.PR_E1.Id, Amount = 3});
        dict.Add(PieceType.PR_E3.Id, new PieceMatchDef {Next = PieceType.PR_E4.Id, Previous = PieceType.PR_E2.Id, Amount = 3});
        dict.Add(PieceType.PR_E4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.PR_E3.Id});
        
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