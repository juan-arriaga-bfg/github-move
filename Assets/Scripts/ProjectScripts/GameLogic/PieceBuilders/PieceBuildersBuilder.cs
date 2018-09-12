using System.Collections.Generic;
using UnityEngine;

public class PieceBuildersBuilder
{
    public Dictionary<int, IPieceBuilder> Build()
    {
        var dict = new Dictionary<int, IPieceBuilder>();

        dict = AddSimplePiece(dict);
        dict = AddEnergyBranchPiece(dict);
        dict = AddObstaclePiece(dict);
        dict = AddOtherPiece(dict);
        
        dict = AddMulticellularPiece2x2(dict);

        dict = AddEnemyPiece(dict);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece(Dictionary<int, IPieceBuilder> dict)
    {
        
        dict = AddSimplePiece<CharacterPieceBuilder>(PieceType.Char1.Id, PieceType.Char9.Id, dict);
        
        dict = AddBuildingBranchPiece(dict, PieceType.A1.Id, PieceType.A10.Id);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.B1.Id, PieceType.B5.Id, dict);
        
        dict = AddBuildingBranchPiece(dict, PieceType.C1.Id, PieceType.C12.Id);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.X1.Id, PieceType.X5.Id, dict);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Experience1.Id, PieceType.Experience5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Mana1.Id, PieceType.Mana5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Coin1.Id, PieceType.Coin5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Crystal1.Id, PieceType.Crystal5.Id, dict);
        
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.ChestA1.Id, PieceType.ChestA6.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.ChestX1.Id, PieceType.ChestX3.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.ChestC1.Id, PieceType.ChestC3.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.ChestZ1.Id, PieceType.ChestZ3.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.Basket1.Id, PieceType.Basket3.Id, dict);
        
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.Chest1.Id, PieceType.Chest9.Id, dict);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Magic1.Id, PieceType.Magic.Id, dict);
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddEnergyBranchPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.D1.Id, PieceType.D5.Id);
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.E1.Id, PieceType.E5.Id);
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.F1.Id, PieceType.F5.Id);
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.G1.Id, PieceType.G5.Id);
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.H1.Id, PieceType.H5.Id);
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.I1.Id, PieceType.I5.Id);
        dict = AddEnergyBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.J1.Id, PieceType.J5.Id);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddObstaclePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.O1.Id, PieceType.O9.Id, dict);
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OX1.Id, PieceType.OX9.Id, dict);
        
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
        
        dict = AddMulticellularPiece<MinePieceBuilder>(PieceType.MineC.Id, PieceType.MineZ.Id, mask, dict);
        
        return dict;
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
    
    private Dictionary<int, IPieceBuilder> AddEnergyBranchPiece<TA, TB>(Dictionary<int, IPieceBuilder> dict, int idMin, int idMax)
        where TA : IPieceBuilder, new()
        where TB : IPieceBuilder, new()
    {
        dict = AddSimplePiece<TA>(idMin, idMin + 1, dict);
        dict.Add(idMin + 2, new TB());
        dict = AddSimplePiece<TA>(idMax - 1, idMax, dict);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddBuildingBranchPiece(Dictionary<int, IPieceBuilder> dict, int idMin, int idMax)
    {
        dict.Add(idMin, new SimplePieceBuilder());

        var flag = true;
        var mask = BoardPosition.GetRect(BoardPosition.Zero(), 2, 2);

        for (var i = idMin + 1; i < idMax - 2; i++)
        {
            if (flag) dict.Add(i, new SimplePieceBuilder());
            else dict.Add(i, new BuildingPieceBuilder());
            
            flag = !flag;
        }
        
        dict.Add(idMax - 2, new PartPieceBuilder());
        dict.Add(idMax - 1, new BuildingBigPieceBuilder{Mask = mask});
        dict.Add(idMax, new MulticellularDraggablePieceBuilder{Mask = mask});
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddEnemyPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<EnemyPieceBuilder>(PieceType.Enemy1.Id,  PieceType.Enemy1.Id,  dict);
        
        // dict.Add(PieceType.Fog.Id, new FogPieceBuilder());
        
        return dict;
    }
}