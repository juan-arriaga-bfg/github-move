using System.Collections.Generic;

public struct PieceMatchDef
{
    public int Next;
    public int Previous;
    public int Count;
    public bool IsIgnore;

    public List<List<int>> Pattern;
}

public class MatchDefinitionBuilder
{
    public Dictionary<int, PieceMatchDef> Build()
    {
        var dict = new Dictionary<int, PieceMatchDef>();
        
        dict = AddPiece(dict);
        
        return dict;
    }

    private Dictionary<int, PieceMatchDef> AddPiece(Dictionary<int, PieceMatchDef> dict)
    {
        // ------------------ A -------------
        
        dict.Add(PieceType.A1.Id, new PieceMatchDef {Next = PieceType.A2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.A2.Id, new PieceMatchDef {Next = PieceType.A3Fake.Id, Previous = PieceType.A1.Id, Count = 3});
        dict.Add(PieceType.A3Fake.Id, new PieceMatchDef {Next = PieceType.A3.Id, Previous = PieceType.A2.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.A3.Id, new PieceMatchDef {Next = PieceType.A4Fake.Id, Previous = PieceType.A2.Id, Count = 3});
        dict.Add(PieceType.A4Fake.Id, new PieceMatchDef {Next = PieceType.A4.Id, Previous = PieceType.A3.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.A4.Id, new PieceMatchDef {Next = PieceType.A5Fake.Id, Previous = PieceType.A3.Id, Count = 3});
        dict.Add(PieceType.A5Fake.Id, new PieceMatchDef {Next = PieceType.A5.Id, Previous = PieceType.A4.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.A5.Id, new PieceMatchDef {Next = PieceType.A6Fake.Id, Previous = PieceType.A4.Id, Count = 3});
        dict.Add(PieceType.A6Fake.Id, new PieceMatchDef {Next = PieceType.A6.Id, Previous = PieceType.A5.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.A6.Id, new PieceMatchDef {Next = PieceType.A7Fake.Id, Previous = PieceType.A5.Id, Count = 3});
        dict.Add(PieceType.A7Fake.Id, new PieceMatchDef {Next = PieceType.A7.Id, Previous = PieceType.A6.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.A7.Id, new PieceMatchDef {Next = PieceType.A8Fake.Id, Previous = PieceType.A6.Id, Count = 3});
        dict.Add(PieceType.A8Fake.Id, new PieceMatchDef {Next = PieceType.A8.Id, Previous = PieceType.A7.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.A8.Id, new PieceMatchDef {Next = PieceType.A9Fake.Id, Previous = PieceType.A7.Id, Count = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.A9Fake.Id, PieceType.A9.Id, PieceType.A8.Id);
        
        dict.Add(PieceType.A9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.A8.Id});
        
        // ------------------ C -------------
        
        dict.Add(PieceType.C1.Id, new PieceMatchDef {Next = PieceType.C2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.C2.Id, new PieceMatchDef {Next = PieceType.C3Fake.Id, Previous = PieceType.C1.Id, Count = 3});
        dict.Add(PieceType.C3Fake.Id, new PieceMatchDef {Next = PieceType.C3.Id, Previous = PieceType.C2.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C3.Id, new PieceMatchDef {Next = PieceType.C4Fake.Id, Previous = PieceType.C2.Id, Count = 3});
        dict.Add(PieceType.C4Fake.Id, new PieceMatchDef {Next = PieceType.C4.Id, Previous = PieceType.C3.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C4.Id, new PieceMatchDef {Next = PieceType.C5Fake.Id, Previous = PieceType.C3.Id, Count = 3});
        dict.Add(PieceType.C5Fake.Id, new PieceMatchDef {Next = PieceType.C5.Id, Previous = PieceType.C4.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C5.Id, new PieceMatchDef {Next = PieceType.C6Fake.Id, Previous = PieceType.C4.Id, Count = 3});
        dict.Add(PieceType.C6Fake.Id, new PieceMatchDef {Next = PieceType.C6.Id, Previous = PieceType.C5.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C6.Id, new PieceMatchDef {Next = PieceType.C7Fake.Id, Previous = PieceType.C5.Id, Count = 3});
        dict.Add(PieceType.C7Fake.Id, new PieceMatchDef {Next = PieceType.C7.Id, Previous = PieceType.C6.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C7.Id, new PieceMatchDef {Next = PieceType.C8Fake.Id, Previous = PieceType.C6.Id, Count = 3});
        dict.Add(PieceType.C8Fake.Id, new PieceMatchDef {Next = PieceType.C8.Id, Previous = PieceType.C7.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C8.Id, new PieceMatchDef {Next = PieceType.C9Fake.Id, Previous = PieceType.C7.Id, Count = 3});
        dict.Add(PieceType.C9Fake.Id, new PieceMatchDef {Next = PieceType.C9.Id, Previous = PieceType.C8.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C9.Id, new PieceMatchDef {Next = PieceType.C10Fake.Id, Previous = PieceType.C8.Id, Count = 3});
        dict.Add(PieceType.C10Fake.Id, new PieceMatchDef {Next = PieceType.C10.Id, Previous = PieceType.C9.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.C10.Id, new PieceMatchDef {Next = PieceType.C11Fake.Id, Previous = PieceType.C9.Id, Count = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.C11Fake.Id, PieceType.C11.Id, PieceType.C10.Id);
        
        dict.Add(PieceType.C11.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.C10.Id});
        
        // ------------------ D -------------
        
        dict.Add(PieceType.D1.Id, new PieceMatchDef {Next = PieceType.D2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.D2.Id, new PieceMatchDef {Next = PieceType.D3.Id, Previous = PieceType.D1.Id, Count = 3});
        dict.Add(PieceType.D3.Id, new PieceMatchDef {Next = PieceType.D4.Id, Previous = PieceType.D2.Id, Count = 3});
        dict.Add(PieceType.D4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.D3.Id});
        
        // ------------------ E -------------
        
        dict.Add(PieceType.E1.Id, new PieceMatchDef {Next = PieceType.E2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.E2.Id, new PieceMatchDef {Next = PieceType.E3.Id, Previous = PieceType.E1.Id, Count = 3});
        dict.Add(PieceType.E3.Id, new PieceMatchDef {Next = PieceType.E4.Id, Previous = PieceType.E2.Id, Count = 3});
        dict.Add(PieceType.E4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.E3.Id});
        
        // ------------------ F -------------
        
        dict.Add(PieceType.F1.Id, new PieceMatchDef {Next = PieceType.F2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.F2.Id, new PieceMatchDef {Next = PieceType.F3.Id, Previous = PieceType.F1.Id, Count = 3});
        dict.Add(PieceType.F3.Id, new PieceMatchDef {Next = PieceType.F4.Id, Previous = PieceType.F2.Id, Count = 3});
        dict.Add(PieceType.F4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.F3.Id});
        
        // ------------------ G -------------
        
        dict.Add(PieceType.G1.Id, new PieceMatchDef {Next = PieceType.G2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.G2.Id, new PieceMatchDef {Next = PieceType.G3.Id, Previous = PieceType.G1.Id, Count = 3});
        dict.Add(PieceType.G3.Id, new PieceMatchDef {Next = PieceType.G4.Id, Previous = PieceType.G2.Id, Count = 3});
        dict.Add(PieceType.G4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.G3.Id});
        
        // ------------------ H -------------
        
        dict.Add(PieceType.H1.Id, new PieceMatchDef {Next = PieceType.H2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.H2.Id, new PieceMatchDef {Next = PieceType.H3.Id, Previous = PieceType.H1.Id, Count = 3});
        dict.Add(PieceType.H3.Id, new PieceMatchDef {Next = PieceType.H4.Id, Previous = PieceType.H2.Id, Count = 3});
        dict.Add(PieceType.H4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.H3.Id});
        
        // ------------------ I -------------
        
        dict.Add(PieceType.I1.Id, new PieceMatchDef {Next = PieceType.I2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.I2.Id, new PieceMatchDef {Next = PieceType.I3.Id, Previous = PieceType.I1.Id, Count = 3});
        dict.Add(PieceType.I3.Id, new PieceMatchDef {Next = PieceType.I4.Id, Previous = PieceType.I2.Id, Count = 3});
        dict.Add(PieceType.I4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.I3.Id});
        
        // ------------------ J -------------
        
        dict.Add(PieceType.J1.Id, new PieceMatchDef {Next = PieceType.J2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.J2.Id, new PieceMatchDef {Next = PieceType.J3.Id, Previous = PieceType.J1.Id, Count = 3});
        dict.Add(PieceType.J3.Id, new PieceMatchDef {Next = PieceType.J4.Id, Previous = PieceType.J2.Id, Count = 3});
        dict.Add(PieceType.J4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.J3.Id});
        
        // ------------------ K -------------
        
        dict.Add(PieceType.K1.Id, new PieceMatchDef {Next = PieceType.K2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.K2.Id, new PieceMatchDef {Next = PieceType.K3Fake.Id, Previous = PieceType.K1.Id, Count = 3});
        dict.Add(PieceType.K3Fake.Id, new PieceMatchDef {Next = PieceType.K3.Id, Previous = PieceType.K2.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K3.Id, new PieceMatchDef {Next = PieceType.K4Fake.Id, Previous = PieceType.K2.Id, Count = 3});
        dict.Add(PieceType.K4Fake.Id, new PieceMatchDef {Next = PieceType.K4.Id, Previous = PieceType.K3.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K4.Id, new PieceMatchDef {Next = PieceType.K5Fake.Id, Previous = PieceType.K3.Id, Count = 3});
        dict.Add(PieceType.K5Fake.Id, new PieceMatchDef {Next = PieceType.K5.Id, Previous = PieceType.K4.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K5.Id, new PieceMatchDef {Next = PieceType.K6Fake.Id, Previous = PieceType.K4.Id, Count = 3});
        dict.Add(PieceType.K6Fake.Id, new PieceMatchDef {Next = PieceType.K6.Id, Previous = PieceType.K5.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K6.Id, new PieceMatchDef {Next = PieceType.K7Fake.Id, Previous = PieceType.K5.Id, Count = 3});
        dict.Add(PieceType.K7Fake.Id, new PieceMatchDef {Next = PieceType.K7.Id, Previous = PieceType.K6.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K7.Id, new PieceMatchDef {Next = PieceType.K8Fake.Id, Previous = PieceType.K6.Id, Count = 3});
        dict.Add(PieceType.K8Fake.Id, new PieceMatchDef {Next = PieceType.K8.Id, Previous = PieceType.K7.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K8.Id, new PieceMatchDef {Next = PieceType.K9Fake.Id, Previous = PieceType.K7.Id, Count = 3});
        dict.Add(PieceType.K9Fake.Id, new PieceMatchDef {Next = PieceType.K9.Id, Previous = PieceType.K8.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.K9.Id, new PieceMatchDef {Next = PieceType.K10Fake.Id, Previous = PieceType.K8.Id, Count = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.K10Fake.Id, PieceType.K10.Id, PieceType.K9.Id);
        
        dict.Add(PieceType.K10.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.K9.Id});
        
        // ------------------ L -------------
        
        dict.Add(PieceType.L1.Id, new PieceMatchDef {Next = PieceType.L2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.L2.Id, new PieceMatchDef {Next = PieceType.L3Fake.Id, Previous = PieceType.L1.Id, Count = 3});
        dict.Add(PieceType.L3Fake.Id, new PieceMatchDef {Next = PieceType.L3.Id, Previous = PieceType.L2.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.L3.Id, new PieceMatchDef {Next = PieceType.L4Fake.Id, Previous = PieceType.L2.Id, Count = 3});
        dict.Add(PieceType.L4Fake.Id, new PieceMatchDef {Next = PieceType.L4.Id, Previous = PieceType.L3.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.L4.Id, new PieceMatchDef {Next = PieceType.L5Fake.Id, Previous = PieceType.L3.Id, Count = 3});
        dict.Add(PieceType.L5Fake.Id, new PieceMatchDef {Next = PieceType.L5.Id, Previous = PieceType.L4.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.L5.Id, new PieceMatchDef {Next = PieceType.L6Fake.Id, Previous = PieceType.L4.Id, Count = 3});
        dict.Add(PieceType.L6Fake.Id, new PieceMatchDef {Next = PieceType.L6.Id, Previous = PieceType.L5.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.L6.Id, new PieceMatchDef {Next = PieceType.L7Fake.Id, Previous = PieceType.L5.Id, Count = 3});
        dict.Add(PieceType.L7Fake.Id, new PieceMatchDef {Next = PieceType.L7.Id, Previous = PieceType.L6.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.L7.Id, new PieceMatchDef {Next = PieceType.L8Fake.Id, Previous = PieceType.L6.Id, Count = 3});
        dict.Add(PieceType.L8Fake.Id, new PieceMatchDef {Next = PieceType.L8.Id, Previous = PieceType.L7.Id, Count = 1, IsIgnore = true});
        dict.Add(PieceType.L8.Id, new PieceMatchDef {Next = PieceType.L9Fake.Id, Previous = PieceType.L7.Id, Count = 3});
        
        dict = AddFakeMulticellularPiece(dict, PieceType.L9Fake.Id, PieceType.L9.Id, PieceType.L8.Id);
        
        dict.Add(PieceType.L9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.L8.Id});
        
        // ------------------ obstacle -------------
        
        dict.Add(PieceType.O1.Id, new PieceMatchDef {Next = PieceType.O2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.O2.Id, new PieceMatchDef {Next = PieceType.O3.Id, Previous = PieceType.O1.Id, Count = -1});
        dict.Add(PieceType.O3.Id, new PieceMatchDef {Next = PieceType.O4.Id, Previous = PieceType.O2.Id, Count = -1});
        dict.Add(PieceType.O4.Id, new PieceMatchDef {Next = PieceType.O5.Id, Previous = PieceType.O3.Id, Count = -1});
        dict.Add(PieceType.O5.Id, new PieceMatchDef {Next = PieceType.O6.Id, Previous = PieceType.O4.Id, Count = -1});
        dict.Add(PieceType.O6.Id, new PieceMatchDef {Next = PieceType.O7.Id, Previous = PieceType.O5.Id, Count = -1});
        dict.Add(PieceType.O7.Id, new PieceMatchDef {Next = PieceType.O8.Id, Previous = PieceType.O6.Id, Count = -1});
        dict.Add(PieceType.O8.Id, new PieceMatchDef {Next = PieceType.O9.Id, Previous = PieceType.O7.Id, Count = -1});
        dict.Add(PieceType.O9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.O8.Id, Count = -1});
        
        dict.Add(PieceType.OX1.Id, new PieceMatchDef {Next = PieceType.OX2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.OX2.Id, new PieceMatchDef {Next = PieceType.OX3.Id, Previous = PieceType.OX1.Id, Count = -1});
        dict.Add(PieceType.OX3.Id, new PieceMatchDef {Next = PieceType.OX4.Id, Previous = PieceType.OX2.Id, Count = -1});
        dict.Add(PieceType.OX4.Id, new PieceMatchDef {Next = PieceType.OX5.Id, Previous = PieceType.OX3.Id, Count = -1});
        dict.Add(PieceType.OX5.Id, new PieceMatchDef {Next = PieceType.OX6.Id, Previous = PieceType.OX4.Id, Count = -1});
        dict.Add(PieceType.OX6.Id, new PieceMatchDef {Next = PieceType.OX7.Id, Previous = PieceType.OX5.Id, Count = -1});
        dict.Add(PieceType.OX7.Id, new PieceMatchDef {Next = PieceType.OX8.Id, Previous = PieceType.OX6.Id, Count = -1});
        dict.Add(PieceType.OX8.Id, new PieceMatchDef {Next = PieceType.OX9.Id, Previous = PieceType.OX7.Id, Count = -1});
        dict.Add(PieceType.OX9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OX8.Id, Count = -1});
        
        dict.Add(PieceType.OEpic1.Id, new PieceMatchDef {Next = PieceType.OEpic2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.OEpic2.Id, new PieceMatchDef {Next = PieceType.OEpic3.Id, Previous = PieceType.OEpic1.Id, Count = -1});
        dict.Add(PieceType.OEpic3.Id, new PieceMatchDef {Next = PieceType.OEpic4.Id, Previous = PieceType.OEpic2.Id, Count = -1});
        dict.Add(PieceType.OEpic4.Id, new PieceMatchDef {Next = PieceType.OEpic5.Id, Previous = PieceType.OEpic3.Id, Count = -1});
        dict.Add(PieceType.OEpic5.Id, new PieceMatchDef {Next = PieceType.OEpic6.Id, Previous = PieceType.OEpic4.Id, Count = -1});
        dict.Add(PieceType.OEpic6.Id, new PieceMatchDef {Next = PieceType.OEpic7.Id, Previous = PieceType.OEpic5.Id, Count = -1});
        dict.Add(PieceType.OEpic7.Id, new PieceMatchDef {Next = PieceType.OEpic8.Id, Previous = PieceType.OEpic6.Id, Count = -1});
        dict.Add(PieceType.OEpic8.Id, new PieceMatchDef {Next = PieceType.OEpic9.Id, Previous = PieceType.OEpic7.Id, Count = -1});
        dict.Add(PieceType.OEpic9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OEpic8.Id, Count = -1});
        
        dict.Add(PieceType.Chest1.Id, new PieceMatchDef {Next = PieceType.Chest2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.Chest2.Id, new PieceMatchDef {Next = PieceType.Chest3.Id, Previous = PieceType.Chest1.Id, Count = 3});
        dict.Add(PieceType.Chest3.Id, new PieceMatchDef {Next = PieceType.Chest4.Id, Previous = PieceType.Chest2.Id, Count = 3});
        dict.Add(PieceType.Chest4.Id, new PieceMatchDef {Next = PieceType.Chest5.Id, Previous = PieceType.Chest3.Id, Count = 3});
        dict.Add(PieceType.Chest5.Id, new PieceMatchDef {Next = PieceType.Chest6.Id, Previous = PieceType.Chest4.Id, Count = 3});
        dict.Add(PieceType.Chest6.Id, new PieceMatchDef {Next = PieceType.Chest7.Id, Previous = PieceType.Chest5.Id, Count = 3});
        dict.Add(PieceType.Chest7.Id, new PieceMatchDef {Next = PieceType.Chest8.Id, Previous = PieceType.Chest6.Id, Count = 3});
        dict.Add(PieceType.Chest8.Id, new PieceMatchDef {Next = PieceType.Chest9.Id, Previous = PieceType.Chest7.Id, Count = 3});
        dict.Add(PieceType.Chest9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Chest8.Id});
        
        dict.Add(PieceType.ChestEpic1.Id, new PieceMatchDef {Next = PieceType.ChestEpic2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestEpic2.Id, new PieceMatchDef {Next = PieceType.ChestEpic3.Id, Previous = PieceType.ChestEpic1.Id, Count = 3});
        dict.Add(PieceType.ChestEpic3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestEpic2.Id});
        
        dict.Add(PieceType.ChestA1.Id, new PieceMatchDef {Next = PieceType.ChestA2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestA2.Id, new PieceMatchDef {Next = PieceType.ChestA3.Id, Previous = PieceType.ChestA1.Id, Count = 3});
        dict.Add(PieceType.ChestA3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestA2.Id});
        
        dict.Add(PieceType.ChestC1.Id, new PieceMatchDef {Next = PieceType.ChestC2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestC2.Id, new PieceMatchDef {Next = PieceType.ChestC3.Id, Previous = PieceType.ChestC1.Id, Count = 3});
        dict.Add(PieceType.ChestC3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestC2.Id});
        
        dict.Add(PieceType.ChestK1.Id, new PieceMatchDef {Next = PieceType.ChestK2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestK2.Id, new PieceMatchDef {Next = PieceType.ChestK3.Id, Previous = PieceType.ChestK1.Id, Count = 3});
        dict.Add(PieceType.ChestK3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestK2.Id});
        
        dict.Add(PieceType.ChestL1.Id, new PieceMatchDef {Next = PieceType.ChestL2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestL2.Id, new PieceMatchDef {Next = PieceType.ChestL3.Id, Previous = PieceType.ChestL1.Id, Count = 3});
        dict.Add(PieceType.ChestL3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestL2.Id});
        
        dict.Add(PieceType.Coin1.Id, new PieceMatchDef {Next = PieceType.Coin2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.Coin2.Id, new PieceMatchDef {Next = PieceType.Coin3.Id, Previous = PieceType.Coin1.Id, Count = 3});
        dict.Add(PieceType.Coin3.Id, new PieceMatchDef {Next = PieceType.Coin4.Id, Previous = PieceType.Coin2.Id, Count = 3});
        dict.Add(PieceType.Coin4.Id, new PieceMatchDef {Next = PieceType.Coin5.Id, Previous = PieceType.Coin3.Id, Count = 3});
        dict.Add(PieceType.Coin5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Coin4.Id});
        
        dict.Add(PieceType.Crystal1.Id, new PieceMatchDef {Next = PieceType.Crystal2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.Crystal2.Id, new PieceMatchDef {Next = PieceType.Crystal3.Id, Previous = PieceType.Crystal1.Id, Count = 3});
        dict.Add(PieceType.Crystal3.Id, new PieceMatchDef {Next = PieceType.Crystal4.Id, Previous = PieceType.Crystal2.Id, Count = 3});
        dict.Add(PieceType.Crystal4.Id, new PieceMatchDef {Next = PieceType.Crystal5.Id, Previous = PieceType.Crystal3.Id, Count = 3});
        dict.Add(PieceType.Crystal5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Crystal4.Id});
        
        dict.Add(PieceType.Magic1.Id, new PieceMatchDef {Next = PieceType.Magic2.Id, Previous = PieceType.None.Id,   Count = 3});
        dict.Add(PieceType.Magic2.Id, new PieceMatchDef {Next = PieceType.Magic3.Id,  Previous = PieceType.Magic1.Id, Count = 3});
        dict.Add(PieceType.Magic3.Id, new PieceMatchDef {Next = PieceType.Magic.Id,  Previous = PieceType.Magic2.Id, Count = 3});
        dict.Add(PieceType.Magic.Id,  new PieceMatchDef {Next = PieceType.None.Id,   Previous = PieceType.Magic3.Id });
        
        return dict;
    }

    private Dictionary<int, PieceMatchDef> AddFakeMulticellularPiece(Dictionary<int, PieceMatchDef> dict, int id, int next, int previous)
    {
        dict.Add(id, new PieceMatchDef {Next = next, Previous = previous, Count = 1, IsIgnore = true, Pattern = new List<List<int>>
        {
            new List<int> { previous, previous },
            new List<int> { previous, previous }
        }});
        
        return dict;
    }
}