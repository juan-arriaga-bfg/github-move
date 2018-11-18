using System.Collections.Generic;

public class UIChestMessageWindowModel : IWWindowModel
{
    public ChestPieceComponent ChestComponent;
    
    public string Title => LocalizationService.Instance.Manager.GetTextByUid("window.chest.title", "Unlock chest!");

    public string Message => LocalizationService.Instance.Manager.GetTextByUid("window.chest.message", "Do you want to unlock this chest?");

    public string ButtonText => LocalizationService.Instance.Manager.GetTextByUid("common.button.unlock", "Unlock");

    public string ChanceText => LocalizationService.Instance.Manager.GetTextByUid("window.chest.chance", "May contain:");

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
