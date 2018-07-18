using System.Collections.Generic;
using UnityEngine;

public class PieceBuildersBuilder
{
    public Dictionary<int, IPieceBuilder> Build()
    {
        var dict = new Dictionary<int, IPieceBuilder>();

        dict = AddSimplePiece(dict);
        dict = AddObstaclePiece(dict);
        dict = AddOtherPiece(dict);
        
        dict = AddMulticellularPiece2x2(dict);
        
        dict = AddCastlePiece(dict);

        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.A1.Id, PieceType.A9.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.B1.Id, PieceType.B5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.C1.Id, PieceType.C9.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.D1.Id, PieceType.D3.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.E2.Id, PieceType.E5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.F1.Id, PieceType.F4.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.G1.Id, PieceType.G3.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.H1.Id, PieceType.H3.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.I1.Id, PieceType.I4.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.J1.Id, PieceType.J3.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.X1.Id, PieceType.X5.Id, dict);
        
        dict.Add(PieceType.D4.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.E1.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.F5.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.G4.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.H4.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.I5.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.J4.Id, new ResourcePieceBuilder());
        dict.Add(PieceType.J5.Id, new ResourcePieceBuilder());
        
        dict = AddSimplePiece<ResourcePieceBuilder>(PieceType.Coin1.Id, PieceType.Coin5.Id, dict);
        
        dict = AddSimplePiece<MineChestPieceBuilder>(PieceType.ChestA1.Id, PieceType.ChestA3.Id, dict);
        dict = AddSimplePiece<MineChestPieceBuilder>(PieceType.ChestX1.Id, PieceType.ChestX3.Id, dict);
        dict = AddSimplePiece<MineChestPieceBuilder>(PieceType.ChestC1.Id, PieceType.ChestC3.Id, dict);
        
        dict = AddSimplePiece<MineChestPieceBuilder>(PieceType.Chest1.Id, PieceType.Chest9.Id, dict);
        
//        dict.Add(PieceType.E6.Id, new SpawnPieceBuilder());
        dict.Add(PieceType.King.Id, new QuestGiverPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddObstaclePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.O1.Id, PieceType.O5.Id, dict);
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OX1.Id, PieceType.OX5.Id, dict);
        
        dict.Add(PieceType.Fog.Id, new FogPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddOtherPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Generic.Id, new GenericPieceBuilder());
        dict.Add(PieceType.Empty.Id, new EmptyPieceBuilder());
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddMulticellularPiece2x2(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = BoardPosition.GetRect(BoardPosition.Zero(), 2, 2);
        
        dict = AddMulticellularPiece<MinePieceBuilder>(PieceType.MineA.Id, PieceType.MineX.Id, mask, dict);
        
//        dict = AddMulticellularPiece<MarketPieceBuilder>(PieceType.Market1.Id, PieceType.Market9.Id, mask, dict);
//        dict = AddMulticellularPiece<StoragePieceBuilder>(PieceType.Storage1.Id, PieceType.Storage9.Id, mask, dict);
//        dict = AddMulticellularPiece<ProductionPieceBuilder>(PieceType.Factory1.Id, PieceType.Factory9.Id, mask, dict);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddCastlePiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = BoardPosition.GetRect(BoardPosition.Zero(), 4, 4);
        
        return AddMulticellularPiece<CastlePieceBuilder>(PieceType.Castle1.Id, PieceType.Castle9.Id, mask, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece<T>(int idMin, int idMax, Dictionary<int, IPieceBuilder> dict) where T : IPieceBuilder, new()
    {
        for (var id = idMin; id < idMax + 1; id++)
        {
            dict.Add(id, new T());
        }
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddMulticellularPiece<T>(int idMin, int idMax, List<BoardPosition> mask, Dictionary<int, IPieceBuilder> dict) where T : MulticellularPieceBuilder, new()
    {
        for (var id = idMin; id < idMax + 1; id++)
        {
            dict.Add(id, new T {Mask = mask});
        }
        
        return dict;
    }
}