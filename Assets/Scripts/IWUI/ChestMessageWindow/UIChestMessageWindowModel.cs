using System;
using System.Collections.Generic;

public class UIChestMessageWindowModel : IWWindowModel 
{
    public Chest Chest { get; set; }

    public Action OnOpen;
    
    public string Title => "Unlock chest!";

    public string Message => "Do you want to unlock this chest?";

    public string ButtonText => "Unlock";

    public string ChanceText => "May contain:";

    public List<string> Icons()
    {
        var icons = new List<string>();

        if (Chest.Def.PieceWeights == null) return icons;
        
        foreach (var piece in Chest.Def.PieceWeights)
        {
            if(piece.Piece == PieceType.Empty.Id || piece.Weight == 0) continue;
            
            icons.Add(piece.Uid);
        }
        
        return icons;
    }
}
