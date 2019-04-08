using System.Collections.Generic;

public class UIChestMessageWindowModel : IWWindowModel
{
    public ChestPieceComponent ChestComponent;
    
    public string Title => LocalizationService.Get("window.chest.title", "window.chest.title");
    public string Message => LocalizationService.Get("window.chest.message", "window.chest.message");
    public string ButtonText => LocalizationService.Get("common.button.unlock", "common.button.unlock");
    public string DelimiterText => LocalizationService.Get("window.chest.chance", "window.chest.chance");

    public List<string> Icons()
    {
        var icons = new List<string>();

        if (ChestComponent.Def.Piece == PieceType.CH_NPC.Id && ChestComponent.Def.CharactersAmount.IsActive)
        {
            icons = AddIcons(icons, GameDataService.Current.CharactersManager.CharactersChestWeights);
        }

        if (ChestComponent.Def.PieceAmount > 0) icons = AddIcons(icons, ChestComponent.Def.PieceWeights);
        if (ChestComponent.Def.ProductionAmount.IsActive) icons = AddIcons(icons, GameDataService.Current.LevelsManager.PieceWeights);
        if (ChestComponent.Def.ResourcesAmount.IsActive) icons = AddIcons(icons, GameDataService.Current.LevelsManager.ResourcesWeights);
        
        if (ChestComponent.Def.Piece != PieceType.CH_NPC.Id && ChestComponent.Def.CharactersAmount.IsActive)
        {
            icons = AddIcons(icons, GameDataService.Current.CharactersManager.CharactersWeights);
        }
        
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
