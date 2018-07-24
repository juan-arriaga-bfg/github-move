using System;
using System.Collections.Generic;

public class UIChestMessageWindowModel : IWWindowModel 
{
    public Chest Chest { get; set; }

    public Action OnOpen;
    
    public string Title
    {
        get { return "Unlock chest!"; }
    }
    
    public string Message
    {
        get { return "Do you want to unlock this chest?"; }
    }
    
    public string ButtonText
    {
        get { return "Unlock"; }
    }
    
    public string ChanceText
    {
        get { return "May contain:"; }
    }

    public List<string> Icons()
    {
        var icons = new List<string>();

        foreach (var piece in Chest.Def.PieceWeights)
        {
            if(piece.Piece == PieceType.Empty.Id || piece.Weight == 0) continue;
            
            icons.Add(piece.Uid);
        }
        
        return icons;
    }
}
