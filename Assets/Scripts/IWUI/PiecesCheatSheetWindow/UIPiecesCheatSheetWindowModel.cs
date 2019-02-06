using UnityEngine;
using System.Collections;
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
                ids.AddRange(GetPieceIds(PieceType.NPC_SleepingBeauty.Id, PieceType.NPC_19.Id));
                break;
            case "Boosters":
                ids.AddRange(GetPieceIds(PieceType.Boost_CR1.Id, PieceType.Boost_CR.Id));
                ids.Add(PieceType.Boost_WR.Id);
                break;
            case "Currencies":
                ids.AddRange(GetPieceIds(PieceType.Mana1.Id, PieceType.Mana3.Id));
                ids.AddRange(GetPieceIds(PieceType.Soft1.Id, PieceType.Soft6.Id));
                ids.AddRange(GetPieceIds(PieceType.Hard1.Id, PieceType.Hard6.Id));
                break;
            case "Mines":
                ids.AddRange(GetPieceIds(PieceType.MN_B.Id, PieceType.MN_D.Id));
                break;
            case "Chests":
                ids.Add(PieceType.CH_Free.Id);
                ids.AddRange(GetPieceIds(PieceType.SK1_PR.Id, PieceType.SK3_PR.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_A.Id, PieceType.CH3_A.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_B.Id, PieceType.CH3_B.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_C.Id, PieceType.CH3_C.Id));
                ids.AddRange(GetPieceIds(PieceType.CH1_D.Id, PieceType.CH3_D.Id));
                break;
            case "Obstacles":
                ids.AddRange(GetPieceIds(PieceType.OB1_TT.Id, PieceType.OB2_TT.Id));
                ids.AddRange(GetPieceIds(PieceType.OB1_A.Id, PieceType.OB9_A.Id));
                ids.AddRange(GetPieceIds(PieceType.OB_PR_A.Id, PieceType.OB_PR_G.Id));
                break;
            case "Simple A":
                ids.AddRange(GetPieceIds(PieceType.A1.Id, PieceType.A9.Id));
                break;
            case "Simple B":
                ids.AddRange(GetPieceIds(PieceType.B1.Id, PieceType.B11.Id));
                break;
            case "Simple C":
                ids.AddRange(GetPieceIds(PieceType.C1.Id, PieceType.C9.Id));
                break;
            case "Simple D":
                ids.AddRange(GetPieceIds(PieceType.D1.Id, PieceType.D9.Id));
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
        }
        
        return ids;
    }

    private List<int> GetPieceIds(int idMin, int idMax)
    {
        var ids = new List<int>();

        for (var i = idMin; i < idMax + 1; i++)
        {
            if(PieceType.GetDefById(i).Filter.Has(PieceTypeFilter.Fake)) continue;
            
            ids.Add(i);
        }

        return ids;
    }
}
