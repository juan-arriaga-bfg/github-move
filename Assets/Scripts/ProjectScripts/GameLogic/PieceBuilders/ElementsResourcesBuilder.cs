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
        
        dict.Add(PieceType.E1.Id, R.E1Piece);
        dict.Add(PieceType.E2.Id, R.E2Piece);
        dict.Add(PieceType.E3.Id, R.E3Piece);

        dict.Add(PieceType.O1.Id, R.O1Piece);
        dict.Add(PieceType.O2.Id, R.O2Piece);
        dict.Add(PieceType.O3.Id, R.O3Piece);
        dict.Add(PieceType.O4.Id, R.O4Piece);
        
        dict.Add(PieceType.Fog.Id, R.FogPiece);
        
        dict.Add(PieceType.A1.Id, R.A1Piece);
        dict.Add(PieceType.A2.Id, R.A2Piece);
        dict.Add(PieceType.A3.Id, R.A3Piece);
        dict.Add(PieceType.A4.Id, R.A4Piece);
        dict.Add(PieceType.A5.Id, R.A5Piece);
        dict.Add(PieceType.A6.Id, R.A6Piece);
        dict.Add(PieceType.A7.Id, R.A7Piece);
        dict.Add(PieceType.A8.Id, R.A8Piece);

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
        
        dict.Add(PieceType.Gbox1.Id, R.GBox1Piece);
        
        dict.Add(PieceType.Chest1.Id, R.Chest1Piece);
        dict.Add(PieceType.Chest2.Id, R.Chest2Piece);
        dict.Add(PieceType.Chest3.Id, R.Chest3Piece);
        
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

        dict.Add(PieceType.Tavern1.Id, R.Tavern1Piece);
        dict.Add(PieceType.Tavern2.Id, R.Tavern2Piece);
        dict.Add(PieceType.Tavern3.Id, R.Tavern3Piece);
        dict.Add(PieceType.Tavern4.Id, R.Tavern4Piece);
        dict.Add(PieceType.Tavern5.Id, R.Tavern5Piece);
        dict.Add(PieceType.Tavern6.Id, R.Tavern6Piece);
        dict.Add(PieceType.Tavern7.Id, R.Tavern7Piece);
        dict.Add(PieceType.Tavern8.Id, R.Tavern8Piece);
        dict.Add(PieceType.Tavern9.Id, R.Tavern9Piece);

        return dict;
    }
    
    private Dictionary<int, string> AddView(Dictionary<int, string> dict)
    {
        dict.Add((int)ViewType.HitboxDamage, R.HitboxDamageView);
        dict.Add((int)ViewType.AddResource, R.AddResourceView);
        dict.Add((int)ViewType.HintArrow, R.HintArrow);
        dict.Add((int)ViewType.AddCards, R.AddCards);
        dict.Add((int)ViewType.StorageState, R.ChangeStorageStateView);
        dict.Add((int)ViewType.BoardTimer, R.BoardTimerView);
        dict.Add((int)ViewType.LevelLabel, R.PieceLevelView);
        dict.Add((int)ViewType.Menu, R.MenuView);
        dict.Add((int)ViewType.MenuButton, R.BoardButtonView);
        dict.Add((int)ViewType.SimpleObstacle, R.ChangeSimpleObstacleStateView);

        return dict;
    }
}