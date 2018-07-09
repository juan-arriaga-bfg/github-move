using System.Collections.Generic;

public class ElementsResourcesBuilder
{
    public Dictionary<int, string> Build()
    {
        var dict = new Dictionary<int, string>();
        
        dict = AddPiece(dict);
        dict = AddMulticellularPiece(dict);
        dict = AddView(dict);
        
        return dict;
    }
    
    private Dictionary<int, string> AddPiece(Dictionary<int, string> dict)
    {
        dict.Add(PieceType.Generic.Id, R.GenericPiece);
        
        dict.Add(PieceType.O1.Id, R.O1Piece);
        dict.Add(PieceType.O2.Id, R.O2Piece);
        dict.Add(PieceType.O3.Id, R.O3Piece);
        dict.Add(PieceType.O4.Id, R.O4Piece);
        dict.Add(PieceType.O5.Id, R.O5Piece);
        
        dict.Add(PieceType.OX1.Id, R.OX1Piece);
        dict.Add(PieceType.OX2.Id, R.OX2Piece);
        dict.Add(PieceType.OX3.Id, R.OX3Piece);
        dict.Add(PieceType.OX4.Id, R.OX4Piece);
        dict.Add(PieceType.OX5.Id, R.OX5Piece);
        
        dict.Add(PieceType.King.Id, R.KingPiece);
        
        dict.Add(PieceType.Fog.Id, R.FogPiece);
        
        dict.Add(PieceType.A1.Id, R.A1Piece);
        dict.Add(PieceType.A2.Id, R.A2Piece);
        dict.Add(PieceType.A3.Id, R.A3Piece);
        dict.Add(PieceType.A4.Id, R.A4Piece);
        dict.Add(PieceType.A5.Id, R.A5Piece);
        dict.Add(PieceType.A6.Id, R.A6Piece);
        dict.Add(PieceType.A7.Id, R.A7Piece);
        dict.Add(PieceType.A8.Id, R.A8Piece);
        dict.Add(PieceType.A9.Id, R.A9Piece);
        
        dict.Add(PieceType.B1.Id, R.B1Piece);
        dict.Add(PieceType.B2.Id, R.B2Piece);
        dict.Add(PieceType.B3.Id, R.B3Piece);
        dict.Add(PieceType.B4.Id, R.B4Piece);
        dict.Add(PieceType.B5.Id, R.B5Piece);
        
        dict.Add(PieceType.C1.Id, R.C1Piece);
        dict.Add(PieceType.C2.Id, R.C2Piece);
        dict.Add(PieceType.C3.Id, R.C3Piece);
        dict.Add(PieceType.C4.Id, R.C4Piece);
        dict.Add(PieceType.C5.Id, R.C5Piece);
        dict.Add(PieceType.C6.Id, R.C6Piece);
        dict.Add(PieceType.C7.Id, R.C7Piece);
        dict.Add(PieceType.C8.Id, R.C8Piece);
        dict.Add(PieceType.C9.Id, R.C9Piece);
        
        dict.Add(PieceType.D1.Id, R.D1Piece);
        dict.Add(PieceType.D2.Id, R.D2Piece);
        dict.Add(PieceType.D3.Id, R.D3Piece);
        dict.Add(PieceType.D4.Id, R.D4Piece);
        dict.Add(PieceType.D5.Id, R.D5Piece);
        
        dict.Add(PieceType.E1.Id, R.E1Piece);
        dict.Add(PieceType.E2.Id, R.E2Piece);
        dict.Add(PieceType.E3.Id, R.E3Piece);
        dict.Add(PieceType.E4.Id, R.E4Piece);
        dict.Add(PieceType.E5.Id, R.E5Piece);
        dict.Add(PieceType.E6.Id, R.E6Piece);
        
        dict.Add(PieceType.F1.Id, R.F1Piece);
        dict.Add(PieceType.F2.Id, R.F2Piece);
        dict.Add(PieceType.F3.Id, R.F3Piece);
        dict.Add(PieceType.F4.Id, R.F4Piece);
        dict.Add(PieceType.F5.Id, R.F5Piece);
        
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
        dict.Add(PieceType.I5.Id, R.I5Piece);
        
        dict.Add(PieceType.J1.Id, R.J1Piece);
        dict.Add(PieceType.J2.Id, R.J2Piece);
        dict.Add(PieceType.J3.Id, R.J3Piece);
        dict.Add(PieceType.J4.Id, R.J4Piece);
        dict.Add(PieceType.J5.Id, R.J5Piece);
        
        dict.Add(PieceType.ChestA1.Id, R.ChestA1Piece);
        dict.Add(PieceType.ChestA2.Id, R.ChestA2Piece);
        dict.Add(PieceType.ChestA3.Id, R.ChestA3Piece);
        
        dict.Add(PieceType.ChestB1.Id, R.ChestB1Piece);
        dict.Add(PieceType.ChestB2.Id, R.ChestB2Piece);
        dict.Add(PieceType.ChestB3.Id, R.ChestB3Piece);
        
        dict.Add(PieceType.ChestC1.Id, R.ChestC1Piece);
        dict.Add(PieceType.ChestC2.Id, R.ChestC2Piece);
        dict.Add(PieceType.ChestC3.Id, R.ChestC3Piece);
        
        dict.Add(PieceType.Chest1.Id, R.Chest1Piece);
        dict.Add(PieceType.Chest2.Id, R.Chest2Piece);
        dict.Add(PieceType.Chest3.Id, R.Chest3Piece);
        dict.Add(PieceType.Chest4.Id, R.Chest4Piece);
        dict.Add(PieceType.Chest5.Id, R.Chest5Piece);
        dict.Add(PieceType.Chest6.Id, R.Chest6Piece);
        dict.Add(PieceType.Chest7.Id, R.Chest7Piece);
        dict.Add(PieceType.Chest8.Id, R.Chest8Piece);
        dict.Add(PieceType.Chest9.Id, R.Chest9Piece);
        
        dict.Add(PieceType.Coin1.Id, R.Coin1Piece);
        dict.Add(PieceType.Coin2.Id, R.Coin2Piece);
        dict.Add(PieceType.Coin3.Id, R.Coin3Piece);
        dict.Add(PieceType.Coin4.Id, R.Coin4Piece);
        dict.Add(PieceType.Coin5.Id, R.Coin5Piece);
        
        return dict;
    }

    private Dictionary<int, string> AddMulticellularPiece(Dictionary<int, string> dict)
    {
        dict.Add(PieceType.Mine1.Id, R.Mine1Piece);
        dict.Add(PieceType.Mine2.Id, R.Mine2Piece);
        dict.Add(PieceType.Mine3.Id, R.Mine3Piece);
        dict.Add(PieceType.Mine4.Id, R.Mine4Piece);
        dict.Add(PieceType.Mine5.Id, R.Mine5Piece);
        dict.Add(PieceType.Mine6.Id, R.Mine6Piece);
        dict.Add(PieceType.Mine7.Id, R.Mine7Piece);
        
        dict.Add(PieceType.Sawmill1.Id, R.Sawmill1Piece);
        dict.Add(PieceType.Sawmill2.Id, R.Sawmill2Piece);
        dict.Add(PieceType.Sawmill3.Id, R.Sawmill3Piece);
        dict.Add(PieceType.Sawmill4.Id, R.Sawmill4Piece);
        dict.Add(PieceType.Sawmill5.Id, R.Sawmill5Piece);
        dict.Add(PieceType.Sawmill6.Id, R.Sawmill6Piece);
        dict.Add(PieceType.Sawmill7.Id, R.Sawmill7Piece);
        
        dict.Add(PieceType.Sheepfold1.Id, R.Sheepfold1Piece);
        dict.Add(PieceType.Sheepfold2.Id, R.Sheepfold2Piece);
        dict.Add(PieceType.Sheepfold3.Id, R.Sheepfold3Piece);
        dict.Add(PieceType.Sheepfold4.Id, R.Sheepfold4Piece);
        dict.Add(PieceType.Sheepfold5.Id, R.Sheepfold5Piece);
        dict.Add(PieceType.Sheepfold6.Id, R.Sheepfold6Piece);
        dict.Add(PieceType.Sheepfold7.Id, R.Sheepfold7Piece);
        
        dict.Add(PieceType.Castle1.Id, R.Castle1Piece);
        dict.Add(PieceType.Castle2.Id, R.Castle2Piece);
        dict.Add(PieceType.Castle3.Id, R.Castle3Piece);
        dict.Add(PieceType.Castle4.Id, R.Castle4Piece);
        dict.Add(PieceType.Castle5.Id, R.Castle5Piece);
        dict.Add(PieceType.Castle6.Id, R.Castle6Piece);
        dict.Add(PieceType.Castle7.Id, R.Castle7Piece);
        dict.Add(PieceType.Castle8.Id, R.Castle8Piece);
        dict.Add(PieceType.Castle9.Id, R.Castle9Piece);

        dict.Add(PieceType.Market1.Id, R.Market1Piece);
        dict.Add(PieceType.Market2.Id, R.Market2Piece);
        dict.Add(PieceType.Market3.Id, R.Market3Piece);
        dict.Add(PieceType.Market4.Id, R.Market4Piece);
        dict.Add(PieceType.Market5.Id, R.Market5Piece);
        dict.Add(PieceType.Market6.Id, R.Market6Piece);
        dict.Add(PieceType.Market7.Id, R.Market7Piece);
        dict.Add(PieceType.Market8.Id, R.Market8Piece);
        dict.Add(PieceType.Market9.Id, R.Market9Piece);
        
        dict.Add(PieceType.Storage1.Id, R.Storage1Piece);
        dict.Add(PieceType.Storage2.Id, R.Storage2Piece);
        dict.Add(PieceType.Storage3.Id, R.Storage3Piece);
        dict.Add(PieceType.Storage4.Id, R.Storage4Piece);
        dict.Add(PieceType.Storage5.Id, R.Storage5Piece);
        dict.Add(PieceType.Storage6.Id, R.Storage6Piece);
        dict.Add(PieceType.Storage7.Id, R.Storage7Piece);
        dict.Add(PieceType.Storage8.Id, R.Storage8Piece);
        dict.Add(PieceType.Storage9.Id, R.Storage9Piece);
        
        dict.Add(PieceType.Factory1.Id, R.Factory1Piece);
        dict.Add(PieceType.Factory2.Id, R.Factory2Piece);
        dict.Add(PieceType.Factory3.Id, R.Factory3Piece);
        dict.Add(PieceType.Factory4.Id, R.Factory4Piece);
        dict.Add(PieceType.Factory5.Id, R.Factory5Piece);
        dict.Add(PieceType.Factory6.Id, R.Factory6Piece);
        dict.Add(PieceType.Factory7.Id, R.Factory7Piece);
        dict.Add(PieceType.Factory8.Id, R.Factory8Piece);
        dict.Add(PieceType.Factory9.Id, R.Factory9Piece);
        
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
        dict.Add((int)ViewType.Obstacle, R.ChangeObstacleStateView);
        dict.Add((int)ViewType.FogState, R.ChangeFogStateView);
        dict.Add((int)ViewType.CastleUpgrade, R.CastleUpgradeView);
        dict.Add((int)ViewType.SimpleUpgrade, R.SimpleUpgradeView);
        dict.Add((int)ViewType.Production, R.ProductionView);
        dict.Add((int)ViewType.ProductionWarning, R.ProductionWarningView);

        return dict;
    }
}