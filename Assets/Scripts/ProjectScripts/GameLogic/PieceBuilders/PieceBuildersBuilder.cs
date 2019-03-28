using System.Collections.Generic;
public class PieceBuildersBuilder
{
    public Dictionary<int, IPieceBuilder> Build()
    {
        var dict = new Dictionary<int, IPieceBuilder>();

        dict = AddSimplePiece(dict);
        dict = AddIngredientsBranchPiece(dict);
        dict = AddObstaclePiece(dict);
        dict = AddOtherPiece(dict);
        
        dict = AddMulticellularPiece2x2(dict);

        dict = AddEnemyPiece(dict);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Boost_WR.Id, new SimplePieceBuilder());
        
        dict = AddSimplePiece<CharacterPieceBuilder>(PieceType.NPC_A.Id, PieceType.NPC_R.Id, dict);
        
        dict = AddBuildingBranchPiece(dict, PieceType.A1.Id, PieceType.A9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.B1.Id, PieceType.B11.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.C1.Id, PieceType.C9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.D1.Id, PieceType.D9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.E1.Id, PieceType.E9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.F1.Id, PieceType.F9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.G1.Id, PieceType.G9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.H1.Id, PieceType.H9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.I1.Id, PieceType.I9.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.J1.Id, PieceType.J9.Id);
        
        dict = AddSimplePiece<ManaPieceBuilder>(PieceType.Mana1.Id, PieceType.Mana6.Id, dict);
        dict = AddSimplePiece<ResourcePieceBuilder>(PieceType.Soft1.Id, PieceType.Soft6.Id, dict);
        dict = AddSimplePiece<ResourcePieceBuilder>(PieceType.Hard1.Id, PieceType.Hard6.Id, dict);
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_B1.Id, PieceType.NPC_B3.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_C1.Id, PieceType.NPC_C5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_D1.Id, PieceType.NPC_D6.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_E1.Id, PieceType.NPC_E8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_F1.Id, PieceType.NPC_F8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_G1.Id, PieceType.NPC_G8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_H1.Id, PieceType.NPC_H8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_I1.Id, PieceType.NPC_I8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_J1.Id, PieceType.NPC_J8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_K1.Id, PieceType.NPC_K8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_L1.Id, PieceType.NPC_L8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_M1.Id, PieceType.NPC_M8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_N1.Id, PieceType.NPC_N8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_O1.Id, PieceType.NPC_O8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_P1.Id, PieceType.NPC_P8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_Q1.Id, PieceType.NPC_Q8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_R1.Id, PieceType.NPC_R8.Id, dict);
        
        dict.Add(PieceType.CH_Free.Id, new ChestPieceBuilder());
        dict.Add(PieceType.CH_NPC.Id, new ChestPieceBuilder());
        
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.SK1_PR.Id, PieceType.SK3_PR.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_A.Id, PieceType.CH3_A.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_B.Id, PieceType.CH3_B.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_C.Id, PieceType.CH3_C.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_D.Id, PieceType.CH3_D.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_E.Id, PieceType.CH3_E.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_F.Id, PieceType.CH3_F.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_G.Id, PieceType.CH3_G.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_H.Id, PieceType.CH3_H.Id, dict);
        dict = AddSimplePiece<ChestPieceBuilder>(PieceType.CH1_I.Id, PieceType.CH3_I.Id, dict);
        
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order1.Id, PieceType.Order1.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order2.Id, PieceType.Order2.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order3.Id, PieceType.Order3.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order4.Id, PieceType.Order4.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order5.Id, PieceType.Order5.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order6.Id, PieceType.Order6.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order7.Id, PieceType.Order7.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order8.Id, PieceType.Order8.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order9.Id, PieceType.Order9.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order10.Id, PieceType.Order10.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order11.Id, PieceType.Order11.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order12.Id, PieceType.Order12.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order13.Id, PieceType.Order13.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order14.Id, PieceType.Order14.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order15.Id, PieceType.Order15.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order16.Id, PieceType.Order16.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order17.Id, PieceType.Order17.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order18.Id, PieceType.Order18.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order19.Id, PieceType.Order19.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order20.Id, PieceType.Order20.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order21.Id, PieceType.Order21.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order22.Id, PieceType.Order22.Id, dict);
        dict = AddSimplePiece<OrderPieceBuilder>(PieceType.Order23.Id, PieceType.Order23.Id, dict);
        
        dict.Add(PieceType.Boost_CR.Id, new CrystalPieceBuilder());
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.Boost_CR1.Id, PieceType.Boost_CR3.Id, dict);
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddIngredientsBranchPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_A1.Id, PieceType.PR_A5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_B1.Id, PieceType.PR_B5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_C1.Id, PieceType.PR_C5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_D1.Id, PieceType.PR_D5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_E1.Id, PieceType.PR_E5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_F1.Id, PieceType.PR_F5.Id);
        dict = AddReproductionBranchPiece<SimplePieceBuilder, ReproductionPieceBuilder, ResourcePieceBuilder>(dict, PieceType.PR_G1.Id, PieceType.PR_G5.Id);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddObstaclePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OB1_TT.Id, PieceType.OB2_TT.Id, dict);
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OB1_A.Id, PieceType.OB9_A.Id, dict);
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OB1_D.Id, PieceType.OB9_D.Id, dict);
        dict = AddSimplePiece<ObstaclePieceBuilder>(PieceType.OB1_G.Id, PieceType.OB5_G.Id, dict);
        dict = AddSimplePiece<MovedObstaclePieceBuilder>(PieceType.OB_PR_A.Id, PieceType.OB_PR_G.Id, dict);
        
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
        
        dict = AddMulticellularPiece<MinePieceBuilder>(PieceType.MN_B.Id, PieceType.MN_I.Id, mask, dict);
        
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
    
    private Dictionary<int, IPieceBuilder> AddReproductionBranchPiece<Ta, Tb, Tc>(Dictionary<int, IPieceBuilder> dict, int idMin, int idMax)
        where Ta : IPieceBuilder, new()
        where Tb : IPieceBuilder, new()
        where Tc : IPieceBuilder, new()
    {
        dict = AddSimplePiece<Ta>(idMin, idMax - 2, dict);
        
        dict.Add(idMax - 1, new Tb());
        dict.Add(idMax, new Tc());
        
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