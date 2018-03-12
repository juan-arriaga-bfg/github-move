using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : IDataLoader<List<PieceDef>>
{
    private Dictionary<int, PieceDef> pieces;
    
    public void LoadData(IDataMapper<List<PieceDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                pieces = new Dictionary<int, PieceDef>();
                
                foreach (var def in data)
                {
                    if (pieces.ContainsKey(def.Piece)) continue;
                
                    pieces.Add(def.Piece, def);
                }
            }
            else
            {
                Debug.LogWarning("[PiecesDataManager]: pieces config not loaded");
            }
        });
    }
    
    public PieceDef GetPieceDef(int id)
    {
        PieceDef def;

        return pieces.TryGetValue(id, out def) ? def : null;
    }

    public PieceDef GetPieceDefOrDefault(int id)
    {
        PieceDef def;
        
        return pieces.TryGetValue(id, out def) ? def : PieceDef.Default();
    }
}