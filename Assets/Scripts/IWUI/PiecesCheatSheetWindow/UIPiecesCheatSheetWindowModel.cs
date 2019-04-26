using System.Collections.Generic;
using System.Linq;

public class UIPiecesCheatSheetWindowModel : IWWindowModel 
{
    public string Title => "Cheat Sheet";

    public List<int> GetPieceIdsBy(PieceTypeFilter typeFilter)
    {
        var exclude = new HashSet<int>
        {
            0, 
            -1,
            1,
            PieceType.Fog.Id
        };
        
        var ret = PieceType.GetIdsByFilter(typeFilter);

        ret = ret.Where(e => !PieceType.GetDefById(e).Filter.Has(PieceTypeFilter.Fake) &&  exclude.Contains(e) == false)
                 .ToList();
        
        return ret;
    }
    
    public List<int> GetPieceIds(string tab)
    {
        var ids = new List<int>();
        
        switch (tab)
        {
            case "Characters":
                ids.AddRange(PieceType.GetIdsByFilter(PieceTypeFilter.Character, PieceTypeFilter.Fake));
                break;
            case "Boosters":
                ids.AddRange(GetPieceIds(PieceType.Boost_CR1.Id, PieceType.Boost_CR3.Id));
                ids.Add(PieceType.Boost_CR.Id);
                
                ids.AddRange(GetPieceIds(PieceType.Boost_WR1.Id, PieceType.Boost_WR3.Id));
                ids.Add(PieceType.Boost_WR.Id);
                break;
            case "Currencies":
                ids.AddRange(GetPieceIds(PieceType.Mana1.Id, PieceType.Mana5.Id));
                ids.AddRange(GetPieceIds(PieceType.Soft1.Id, PieceType.Soft8.Id));
                ids.AddRange(GetPieceIds(PieceType.Hard1.Id, PieceType.Hard6.Id));
                ids.AddRange(GetPieceIds(PieceType.Token1.Id, PieceType.Token3.Id));
                break;
            case "Mines":
                ids.AddRange(GetPieceIds(PieceType.MN_A.Id, PieceType.MN_A3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_B.Id, PieceType.MN_B3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_C.Id, PieceType.MN_C3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_D.Id, PieceType.MN_D3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_E.Id, PieceType.MN_E3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_F.Id, PieceType.MN_F3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_G.Id, PieceType.MN_G3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_H.Id, PieceType.MN_H3.Id));
                ids.AddRange(GetPieceIds(PieceType.MN_I.Id, PieceType.MN_I3.Id));
                break;
            case "Chests":
                ids.Add(PieceType.CH_Free.Id);
                ids.Add(PieceType.CH_NPC.Id);
                ids.AddRange(GetPieceIds(PieceType.SK1_PR.Id, PieceType.SK3_PR.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_A.Id, PieceType.CH3_A.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_B.Id, PieceType.CH3_B.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_C.Id, PieceType.CH3_C.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_D.Id, PieceType.CH3_D.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_E.Id, PieceType.CH3_E.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_F.Id, PieceType.CH3_F.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_G.Id, PieceType.CH3_G.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_H.Id, PieceType.CH3_H.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_I.Id, PieceType.CH3_I.Id));
                break;
            case "Obstacles":
                ids.AddRange(GetPieceIds(PieceType.OB1_TT.Id, PieceType.OB2_TT.Id));
                ids.AddRange(GetPieceIds(PieceType.OB1_A.Id, PieceType.OB9_A.Id));
                ids.AddRange(GetPieceIds(PieceType.OB1_D.Id, PieceType.OB9_D.Id));
                ids.AddRange(GetPieceIds(PieceType.OB1_G.Id, PieceType.OB5_G.Id));
                
                ids.Add(PieceType.OB_PR_A.Id);
                ids.Add(PieceType.OB_PR_B.Id);
                ids.Add(PieceType.OB_PR_C.Id);
                ids.Add(PieceType.OB_PR_D.Id);
                ids.Add(PieceType.OB_PR_E.Id);
                ids.Add(PieceType.OB_PR_F.Id);
                ids.Add(PieceType.OB_PR_G.Id);
                break;
            case "Orders":
                ids.AddRange(PieceType.GetIdsByFilter(PieceTypeFilter.OrderPiece));
                break;
            case "Simple A":
                ids.AddRange(GetPieceIds(PieceType.A1.Id, PieceType.A8.Id));
                ids.Add(PieceType.AM.Id);
                break;
            case "Simple B":
                ids.AddRange(GetPieceIds(PieceType.B1.Id, PieceType.B9.Id));
                ids.Add(PieceType.BM.Id);
                break;
            case "Simple C":
                ids.AddRange(GetPieceIds(PieceType.C1.Id, PieceType.C9.Id));
                ids.Add(PieceType.CM.Id);
                break;
            case "Simple D":
                ids.AddRange(GetPieceIds(PieceType.D1.Id, PieceType.D9.Id));
                ids.Add(PieceType.DM.Id);
                break;
            case "Simple E":
                ids.AddRange(GetPieceIds(PieceType.E1.Id, PieceType.E9.Id));
                ids.Add(PieceType.EM.Id);
                break;
            case "Simple F":
                ids.AddRange(GetPieceIds(PieceType.F1.Id, PieceType.F9.Id));
                ids.Add(PieceType.FM.Id);
                break;
            case "Simple G":
                ids.AddRange(GetPieceIds(PieceType.G1.Id, PieceType.G9.Id));
                ids.Add(PieceType.GM.Id);
                break;
            case "Simple H":
                ids.AddRange(GetPieceIds(PieceType.H1.Id, PieceType.H9.Id));
                ids.Add(PieceType.HM.Id);
                break;
            case "Simple I":
                ids.AddRange(GetPieceIds(PieceType.I1.Id, PieceType.I9.Id));
                ids.Add(PieceType.IM.Id);
                break;
            case "Simple J":
                ids.AddRange(GetPieceIds(PieceType.J1.Id, PieceType.J9.Id));
                ids.Add(PieceType.JM.Id);
                break;
            case "Extended A":
                ids.AddRange(GetPieceIds(PieceType.EXT_A1.Id, PieceType.EXT_A8.Id));
                ids.Add(PieceType.EXT_AM.Id);
                break;
            case "Extended B":
                ids.AddRange(GetPieceIds(PieceType.EXT_B1.Id, PieceType.EXT_B8.Id));
                ids.Add(PieceType.EXT_BM.Id);
                break;
            case "Extended C":
                ids.AddRange(GetPieceIds(PieceType.EXT_C1.Id, PieceType.EXT_C8.Id));
                ids.Add(PieceType.EXT_CM.Id);
                break;
            case "Extended D":
                ids.AddRange(GetPieceIds(PieceType.EXT_D1.Id, PieceType.EXT_D8.Id));
                ids.Add(PieceType.EXT_DM.Id);
                break;
            case "Extended E":
                ids.AddRange(GetPieceIds(PieceType.EXT_E1.Id, PieceType.EXT_E8.Id));
                ids.Add(PieceType.EXT_EM.Id);
                break;
            case "Ingredient A":
                ids.AddRange(GetPieceIds(PieceType.PR_A1.Id, PieceType.PR_A5.Id));
                break;
            case "Ingredient B":
                ids.AddRange(GetPieceIds(PieceType.PR_B1.Id, PieceType.PR_B5.Id));
                break;
            case "Ingredient C":
                ids.AddRange(GetPieceIds(PieceType.PR_C1.Id, PieceType.PR_C5.Id));
                break;
            case "Ingredient D":
                ids.AddRange(GetPieceIds(PieceType.PR_D1.Id, PieceType.PR_D5.Id));
                break;
            case "Ingredient E":
                ids.AddRange(GetPieceIds(PieceType.PR_E1.Id, PieceType.PR_E5.Id));
                break;
            case "Ingredient F":
                ids.AddRange(GetPieceIds(PieceType.PR_F1.Id, PieceType.PR_F5.Id));
                break;
            case "Ingredient G":
                ids.AddRange(GetPieceIds(PieceType.PR_G1.Id, PieceType.PR_G5.Id));
                break;
            case "Character Pieces":
                ids.AddRange(GetPieceIds(PieceType.NPC_B1.Id, PieceType.NPC_B3.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_C1.Id, PieceType.NPC_C6.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_D1.Id, PieceType.NPC_D8.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_E1.Id, PieceType.NPC_E8.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_F1.Id, PieceType.NPC_F8.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_G1.Id, PieceType.NPC_G8.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_H1.Id, PieceType.NPC_H8.Id));
                ids.AddRange(GetPieceIds(PieceType.NPC_I1.Id, PieceType.NPC_I8.Id));
                break;
        }
        
        return ids;
    }

    private List<int> GetPieceIds(int idMin, int idMax)
    {
        var ids = new List<int>();

        for (var i = idMin; i <= idMax; i++)
        {
            var def = PieceType.GetDefById(i);
            
            if (def == null || def.Filter.Has(PieceTypeFilter.Fake)) continue;
            
            ids.Add(i);
        }

        return ids;
    }
}
