using System.Collections.Generic;
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
        dict.Add(PieceType.Boost_WR.Id, new SimplePieceBuilder());
        
        dict = AddSimplePiece<CharacterPieceBuilder>(PieceType.NPC_SleepingBeauty.Id, PieceType.NPC_9.Id, dict);
        
        dict = AddBuildingBranchPiece(dict, PieceType.A1.Id, PieceType.A9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.B1.Id, PieceType.B11.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.C1.Id, PieceType.C10.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.D1.Id, PieceType.D9.Id);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Soft1.Id, PieceType.Soft6.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Hard1.Id, PieceType.Hard6.Id, dict);
        
        dict.Add(PieceType.CH_Free.Id, new ChestPieceBuilder());
        
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_A.Id, PieceType.CH3_A.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_B.Id, PieceType.CH3_B.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_C.Id, PieceType.CH3_C.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_D.Id, PieceType.CH3_D.Id, dict);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Boost_CR1.Id, PieceType.Boost_CR.Id, dict);
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddEnergyBranchPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.PR_A1.Id, PieceType.PR_A5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.PR_B1.Id, PieceType.PR_B5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.PR_C1.Id, PieceType.PR_C5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.PR_D1.Id, PieceType.PR_D5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder>(dict, PieceType.PR_E1.Id, PieceType.PR_E5.Id);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddObstaclePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OB1_A.Id, PieceType.OB9_A.Id, dict);
        dict = AddSimplePiece<MovedObstaclePieceBuilder>(PieceType.OB_PR_A.Id, PieceType.OB_PR_E.Id, dict);
        
        dict.Add(PieceType.Fog.Id, new FogPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddOtherPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Generic.Id, new GenericPieceBuilder());
        dict.Add(PieceType.Empty.Id, new EmptyPieceBuilder());
        dict.Add(PieceType.LockedEmpty.Id, new LockedEmptyPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddEnemyPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<EnemyPieceBuilder>(PieceType.Enemy1.Id,  PieceType.Enemy1.Id,  dict);
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddMulticellularPiece2x2(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = BoardPosition.GetRect(BoardPosition.Zero(), 2, 2);
        
        dict = AddMulticellularPiece<MinePieceBuilder>(PieceType.MN_B.Id, PieceType.MN_D.Id, mask, dict);
        
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
    
    private Dictionary<int, IPieceBuilder> AddReproductionBranchPiece<TA, TB>(Dictionary<int, IPieceBuilder> dict, int idMin, int idMax)
        where TA : IPieceBuilder, new()
        where TB : IPieceBuilder, new()
    {
        dict = AddSimplePiece<TA>(idMin, idMax - 2, dict);
        
        dict.Add(idMax - 1, new TB());
        dict.Add(idMax, new TA());
        
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
        dict.Add(idMax, new MakingPieceBuilder{Mask = mask});
        
        return dict;
    }
}