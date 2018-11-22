using System.Collections.Generic;

public class UIChestMessageWindowModel : IWWindowModel
{
    public ChestPieceComponent ChestComponent;
    
    public string Title => LocalizationService.Get("window.chest.title", "window.chest.title");
    public string Message => LocalizationService.Get("window.chest.message", "window.chest.message");
    public string ButtonText => LocalizationService.Get("common.button.unlock", "common.button.unlock");
    public string ChanceText => LocalizationService.Get("window.chest.chance", "window.chest.chance");

    public List<string> Icons()
    {
        var icons = AddIcons(new List<string>(), ChestComponent.Chest.Def.PieceWeights);
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
