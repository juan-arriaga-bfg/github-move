using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIPiecesCheatSheetWindowModel : IWWindowModel 
{
    public string Title => "Cheat Sheet";
    
    public List<int> PiecesList()
    {
        List<int> ret = PieceType.GetIdsByFilter(PieceTypeFilter.Default);

        ret = ret.Where(e => !PieceType.GetDefById(e).Filter.Has(PieceTypeFilter.Fake)).ToList();
        
        return ret;
    }
}
