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
}
