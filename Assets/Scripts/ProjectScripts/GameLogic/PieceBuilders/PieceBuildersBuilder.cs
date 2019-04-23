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
        dict = AddMinePiece(dict);

        dict = AddEnemyPiece(dict);
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Boost_WR.Id, new SimplePieceBuilder());
        
        dict.Add(PieceType.NPC_SleepingBeautyPlaid.Id, new SimplePieceBuilder());
        dict.Add(PieceType.NPC_Gnome.Id, new SimplePieceBuilder());
        
        dict.Add(PieceType.NPC_A.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_B.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_C.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_D.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_E.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_F.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_G.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_H.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_I.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_J.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_K.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_L.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_M.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_N.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_O.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_P.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_Q.Id, new CharacterPieceBuilder());
        dict.Add(PieceType.NPC_R.Id, new CharacterPieceBuilder());
        
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_B1.Id, PieceType.NPC_B3.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_C1.Id, PieceType.NPC_C8.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.NPC_D1.Id, PieceType.NPC_D8.Id, dict);
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
        
        dict = AddBuildingBranchPiece(dict, PieceType.A1.Id, PieceType.A8.Id, PieceType.AM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.B1.Id, PieceType.B9.Id, PieceType.BM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.C1.Id, PieceType.C9.Id, PieceType.CM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.D1.Id, PieceType.D9.Id, PieceType.DM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.E1.Id, PieceType.E9.Id, PieceType.EM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.F1.Id, PieceType.F9.Id, PieceType.FM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.G1.Id, PieceType.G9.Id, PieceType.GM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.H1.Id, PieceType.H9.Id, PieceType.HM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.I1.Id, PieceType.I9.Id, PieceType.IM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.J1.Id, PieceType.J9.Id, PieceType.JM.Id);
        
        dict = AddBuildingBranchPiece(dict, PieceType.EXT_A1.Id, PieceType.EXT_A8.Id, PieceType.EXT_AM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.EXT_B1.Id, PieceType.EXT_B8.Id, PieceType.EXT_BM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.EXT_C1.Id, PieceType.EXT_C8.Id, PieceType.EXT_CM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.EXT_D1.Id, PieceType.EXT_D8.Id, PieceType.EXT_DM.Id);
        dict = AddBuildingBranchPiece(dict, PieceType.EXT_E1.Id, PieceType.EXT_E8.Id, PieceType.EXT_EM.Id);
        
        dict = AddSimplePiece<ManaPieceBuilder>(PieceType.Mana1.Id, PieceType.Mana6.Id, dict);
        dict = AddSimplePiece<ResourcePieceBuilder>(PieceType.Soft1.Id, PieceType.Soft8.Id, dict);
        dict = AddSimplePiece<ResourcePieceBuilder>(PieceType.Hard1.Id, PieceType.Hard6.Id, dict);
        dict = AddSimplePiece<ResourcePieceBuilder>(PieceType.Token1.Id, PieceType.Token3.Id, dict);
        
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
        
        dict.Add(PieceType.RC_A.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_B.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_C.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_D.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_E.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_F.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_G.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_H.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_I.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_J.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_K.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_L.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_M.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_N.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_O.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_P.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_Q.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_R.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_S.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_T.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_U.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_V.Id, new OrderPieceBuilder());
        dict.Add(PieceType.RC_W.Id, new OrderPieceBuilder());
        
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
        
        dict.Add(PieceType.OB_PR_A.Id, new MovedObstaclePieceBuilder());
        dict.Add(PieceType.OB_PR_B.Id, new MovedObstaclePieceBuilder());
        dict.Add(PieceType.OB_PR_C.Id, new MovedObstaclePieceBuilder());
        dict.Add(PieceType.OB_PR_D.Id, new MovedObstaclePieceBuilder());
        dict.Add(PieceType.OB_PR_E.Id, new MovedObstaclePieceBuilder());
        dict.Add(PieceType.OB_PR_F.Id, new MovedObstaclePieceBuilder());
        dict.Add(PieceType.OB_PR_G.Id, new MovedObstaclePieceBuilder());
        
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
        dict.Add(PieceType.Enemy1.Id, new EnemyPieceBuilder());
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddMinePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddMineBranchPiece(dict, PieceType.MN_A.Id, PieceType.MN_A4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_B.Id, PieceType.MN_B4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_C.Id, PieceType.MN_C4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_D.Id, PieceType.MN_D4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_E.Id, PieceType.MN_E4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_F.Id, PieceType.MN_F4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_G.Id, PieceType.MN_G4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_H.Id, PieceType.MN_H4Fake.Id);
        dict = AddMineBranchPiece(dict, PieceType.MN_I.Id, PieceType.MN_I4Fake.Id);
        
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
    
    private Dictionary<int, IPieceBuilder> AddBuildingBranchPiece(Dictionary<int, IPieceBuilder> dict, int idMin, int idMax, int idMonument)
    {
        dict.Add(idMin, new SimplePieceBuilder());

        var flag = true;
        var mask = BoardPosition.GetRect(BoardPosition.Zero(), 2, 2);

        for (var i = idMin + 1; i < idMax; i++)
        {
            if (flag) dict.Add(i, new SimplePieceBuilder());
            else dict.Add(i, new BuildingPieceBuilder());
            
            flag = !flag;
        }
        
        dict.Add(idMax, new PartPieceBuilder());
        
        dict.Add(idMonument - 1, new BuildingBigPieceBuilder {Mask = mask});
        dict.Add(idMonument, new MakingPieceBuilder {Mask = mask});
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddMineBranchPiece(Dictionary<int, IPieceBuilder> dict, int idMin, int idMax)
    {
        var flag = false;
        var mask = BoardPosition.GetRect(BoardPosition.Zero(), 2, 2);

        for (var i = idMin; i <= idMax; i++)
        {
            if (flag) dict.Add(i, new MinePieceBuilder {Mask = mask});
            else dict.Add(i, new BuildingBigPieceBuilder {Mask = mask, StartState = BuildingState.Waiting});
            
            flag = !flag;
        }
        
        return dict;
    }
}