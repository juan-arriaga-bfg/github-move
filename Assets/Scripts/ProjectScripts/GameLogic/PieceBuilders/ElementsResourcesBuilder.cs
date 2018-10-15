using System.Collections.Generic;

public class ElementsResourcesBuilder
{
    public Dictionary<int, string> Build()
    {
        var dict = new Dictionary<int, string>();
        
        dict = AddPiece(dict);
        dict = AddMulticellularPiece(dict);
        dict = AddView(dict);
        dict = AddEnemies(dict);
        
        return dict;
    }

    private Dictionary<int, string> AddPiece(Dictionary<int, string> dict)
    {
        dict.Add(PieceType.Generic.Id, R.GenericPiece);
        
        dict.Add(PieceType.Char1.Id, R.Char1Piece);
        dict.Add(PieceType.Char2.Id, R.Char2Piece);
        dict.Add(PieceType.Char3.Id, R.Char3Piece);
        dict.Add(PieceType.Char4.Id, R.Char4Piece);
        dict.Add(PieceType.Char5.Id, R.Char5Piece);
        dict.Add(PieceType.Char6.Id, R.Char6Piece);
        dict.Add(PieceType.Char7.Id, R.Char7Piece);
        dict.Add(PieceType.Char8.Id, R.Char8Piece);
        dict.Add(PieceType.Char9.Id, R.Char9Piece);
        
        dict.Add(PieceType.Fog.Id, R.FogPiece);
        
        dict.Add(PieceType.O1.Id, R.OPiece);
        dict.Add(PieceType.O2.Id, R.OPiece);
        dict.Add(PieceType.O3.Id, R.OPiece);
        dict.Add(PieceType.O4.Id, R.OPiece);
        dict.Add(PieceType.O5.Id, R.OPiece);
        dict.Add(PieceType.O6.Id, R.OPiece);
        dict.Add(PieceType.O7.Id, R.OPiece);
        dict.Add(PieceType.O8.Id, R.OPiece);
        dict.Add(PieceType.O9.Id, R.OPiece);
        
        dict.Add(PieceType.OX1.Id, R.OXPiece);
        dict.Add(PieceType.OX2.Id, R.OXPiece);
        dict.Add(PieceType.OX3.Id, R.OXPiece);
        dict.Add(PieceType.OX4.Id, R.OXPiece);
        dict.Add(PieceType.OX5.Id, R.OXPiece);
        dict.Add(PieceType.OX6.Id, R.OXPiece);
        dict.Add(PieceType.OX7.Id, R.OXPiece);
        dict.Add(PieceType.OX8.Id, R.OXPiece);
        dict.Add(PieceType.OX9.Id, R.OXPiece);
        
        dict.Add(PieceType.OEpic1.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic2.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic3.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic4.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic5.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic6.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic7.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic8.Id, R.OEpicPiece);
        dict.Add(PieceType.OEpic9.Id, R.OEpicPiece);
        
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
        
        dict.Add(PieceType.B1.Id, R.B1Piece);
        dict.Add(PieceType.B2.Id, R.B2Piece);
        dict.Add(PieceType.B3.Id, R.B3Piece);
        dict.Add(PieceType.B4.Id, R.B4Piece);
        dict.Add(PieceType.B5.Id, R.B5Piece);
        
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
        dict.Add(PieceType.C10Fake.Id, R.C10Piece);
        dict.Add(PieceType.C10.Id, R.C10Piece);
        dict.Add(PieceType.C11Fake.Id, R.C11Piece);
        dict.Add(PieceType.C11.Id, R.C11Piece);
        dict.Add(PieceType.C12Fake.Id, R.C12Piece);
        dict.Add(PieceType.C12.Id, R.C12Piece);
        
        dict.Add(PieceType.D1.Id, R.D1Piece);
        dict.Add(PieceType.D2.Id, R.D2Piece);
        dict.Add(PieceType.D3.Id, R.D3Piece);
        dict.Add(PieceType.D4.Id, R.D4Piece);
        
        dict.Add(PieceType.E1.Id, R.E1Piece);
        dict.Add(PieceType.E2.Id, R.E2Piece);
        dict.Add(PieceType.E3.Id, R.E3Piece);
        dict.Add(PieceType.E4.Id, R.E4Piece);
        
        dict.Add(PieceType.F1.Id, R.F1Piece);
        dict.Add(PieceType.F2.Id, R.F2Piece);
        dict.Add(PieceType.F3.Id, R.F3Piece);
        dict.Add(PieceType.F4.Id, R.F4Piece);
        
        dict.Add(PieceType.G1.Id, R.G1Piece);
        dict.Add(PieceType.G2.Id, R.G2Piece);
        dict.Add(PieceType.G3.Id, R.G3Piece);
        dict.Add(PieceType.G4.Id, R.G4Piece);
        
        dict.Add(PieceType.H1.Id, R.H1Piece);
        dict.Add(PieceType.H2.Id, R.H2Piece);
        dict.Add(PieceType.H3.Id, R.H3Piece);
        dict.Add(PieceType.H4.Id, R.H4Piece);
        
        dict.Add(PieceType.I1.Id, R.I1Piece);
        dict.Add(PieceType.I2.Id, R.I2Piece);
        dict.Add(PieceType.I3.Id, R.I3Piece);
        dict.Add(PieceType.I4.Id, R.I4Piece);
        
        dict.Add(PieceType.J1.Id, R.J1Piece);
        dict.Add(PieceType.J2.Id, R.J2Piece);
        dict.Add(PieceType.J3.Id, R.J3Piece);
        dict.Add(PieceType.J4.Id, R.J4Piece);
        
        dict.Add(PieceType.K1.Id, R.K1Piece);
        dict.Add(PieceType.K2.Id, R.K2Piece);
        dict.Add(PieceType.K3Fake.Id, R.K3Piece);
        dict.Add(PieceType.K3.Id, R.K3Piece);
        dict.Add(PieceType.K4Fake.Id, R.K4Piece);
        dict.Add(PieceType.K4.Id, R.K4Piece);
        dict.Add(PieceType.K5Fake.Id, R.K5Piece);
        dict.Add(PieceType.K5.Id, R.K5Piece);
        dict.Add(PieceType.K6Fake.Id, R.K6Piece);
        dict.Add(PieceType.K6.Id, R.K6Piece);
        dict.Add(PieceType.K7Fake.Id, R.K7Piece);
        dict.Add(PieceType.K7.Id, R.K7Piece);
        dict.Add(PieceType.K8Fake.Id, R.K8Piece);
        dict.Add(PieceType.K8.Id, R.K8Piece);
        dict.Add(PieceType.K9Fake.Id, R.K9Piece);
        dict.Add(PieceType.K9.Id, R.K9Piece);
        dict.Add(PieceType.K10Fake.Id, R.K10Piece);
        dict.Add(PieceType.K10.Id, R.K10Piece);
        
        dict.Add(PieceType.L1.Id, R.L1Piece);
        dict.Add(PieceType.L2.Id, R.L2Piece);
        dict.Add(PieceType.L3Fake.Id, R.L3Piece);
        dict.Add(PieceType.L3.Id, R.L3Piece);
        dict.Add(PieceType.L4Fake.Id, R.L4Piece);
        dict.Add(PieceType.L4.Id, R.L4Piece);
        dict.Add(PieceType.L5Fake.Id, R.L5Piece);
        dict.Add(PieceType.L5.Id, R.L5Piece);
        dict.Add(PieceType.L6Fake.Id, R.L6Piece);
        dict.Add(PieceType.L6.Id, R.L6Piece);
        dict.Add(PieceType.L7Fake.Id, R.L7Piece);
        dict.Add(PieceType.L7.Id, R.L7Piece);
        dict.Add(PieceType.L8Fake.Id, R.L8Piece);
        dict.Add(PieceType.L8.Id, R.L8Piece);
        dict.Add(PieceType.L9Fake.Id, R.L9Piece);
        dict.Add(PieceType.L9.Id, R.L9Piece);
        
        // ---------------------- Chests
        
        dict.Add(PieceType.Chest1.Id, R.Chest1Piece);
        dict.Add(PieceType.Chest2.Id, R.Chest2Piece);
        dict.Add(PieceType.Chest3.Id, R.Chest3Piece);
        dict.Add(PieceType.Chest4.Id, R.Chest4Piece);
        dict.Add(PieceType.Chest5.Id, R.Chest5Piece);
        dict.Add(PieceType.Chest6.Id, R.Chest6Piece);
        dict.Add(PieceType.Chest7.Id, R.Chest7Piece);
        dict.Add(PieceType.Chest8.Id, R.Chest8Piece);
        dict.Add(PieceType.Chest9.Id, R.Chest9Piece);
        
        dict.Add(PieceType.ChestEpic1.Id, R.ChestEpic1Piece);
        dict.Add(PieceType.ChestEpic2.Id, R.ChestEpic2Piece);
        dict.Add(PieceType.ChestEpic3.Id, R.ChestEpic3Piece);
        
        dict.Add(PieceType.ChestA1.Id, R.ChestA1Piece);
        dict.Add(PieceType.ChestA2.Id, R.ChestA2Piece);
        dict.Add(PieceType.ChestA3.Id, R.ChestA3Piece);
        
        dict.Add(PieceType.ChestC1.Id, R.ChestC1Piece);
        dict.Add(PieceType.ChestC2.Id, R.ChestC2Piece);
        dict.Add(PieceType.ChestC3.Id, R.ChestC3Piece);
        
        dict.Add(PieceType.ChestK1.Id, R.ChestK1Piece);
        dict.Add(PieceType.ChestK2.Id, R.ChestK2Piece);
        dict.Add(PieceType.ChestK3.Id, R.ChestK3Piece);
        
        dict.Add(PieceType.ChestL1.Id, R.ChestL1Piece);
        dict.Add(PieceType.ChestL2.Id, R.ChestL2Piece);
        dict.Add(PieceType.ChestL3.Id, R.ChestL3Piece);
        
        dict.Add(PieceType.Worker1.Id, R.Worker1Piece);
        
        dict.Add(PieceType.Coin1.Id, R.Coin1Piece);
        dict.Add(PieceType.Coin2.Id, R.Coin2Piece);
        dict.Add(PieceType.Coin3.Id, R.Coin3Piece);
        dict.Add(PieceType.Coin4.Id, R.Coin4Piece);
        dict.Add(PieceType.Coin5.Id, R.Coin5Piece);
        
        dict.Add(PieceType.Crystal1.Id, R.Crystal1Piece);
        dict.Add(PieceType.Crystal2.Id, R.Crystal2Piece);
        dict.Add(PieceType.Crystal3.Id, R.Crystal3Piece);
        dict.Add(PieceType.Crystal4.Id, R.Crystal4Piece);
        dict.Add(PieceType.Crystal5.Id, R.Crystal5Piece);
        
        dict.Add(PieceType.Magic1.Id, R.Magic1Piece);
        dict.Add(PieceType.Magic2.Id, R.Magic2Piece);
        dict.Add(PieceType.Magic3.Id, R.Magic3Piece);
        dict.Add(PieceType.Magic.Id, R.MagicPiece);
        
        return dict;
    }

    private Dictionary<int, string> AddMulticellularPiece(Dictionary<int, string> dict)
    {
        dict.Add(PieceType.MineC.Id, R.MineCPiece);
        dict.Add(PieceType.MineK.Id, R.MineKPiece);
        dict.Add(PieceType.MineL.Id, R.MineLPiece);
        
        return dict;
    }
    
    private Dictionary<int, string> AddEnemies(Dictionary<int, string> dict)
    {
        dict.Add(PieceType.Enemy1.Id, R.Enemy1Piece);
        
        return dict;
    }
    
    private Dictionary<int, string> AddView(Dictionary<int, string> dict)
    {
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
        
        return dict;
    }
}