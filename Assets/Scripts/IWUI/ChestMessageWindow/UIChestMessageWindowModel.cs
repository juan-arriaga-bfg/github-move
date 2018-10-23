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
        var icons = AddIcons(new List<string>(), Chest.Def.PieceWeights);
        icons = AddIcons(icons, GameDataService.Current.LevelsManager.PieceWeights);
        
        return icons;
    }

    private List<string> AddIcons(List<string> list, List<ItemWeight> weights)
    {
        if (weights == null) return list;
        
        foreach (var weight in weights)
        {
            if(weight.Piece == PieceType.Empty.Id || list.Contains(weight.Uid) || weight.Weight == 0) continue;
            
            list.Add(weight.Uid);
        }
        
        return list;
    }
}
