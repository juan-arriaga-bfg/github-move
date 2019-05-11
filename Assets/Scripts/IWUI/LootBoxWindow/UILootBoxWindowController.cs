using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILootBoxWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UILootBoxWindowModel windowModel = new UILootBoxWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }

    public static void OpenChestWindow(int chest)
    {
        var data = GameDataService.Current.ChestsManager.GetChest(chest);
        var model = UIService.Get.GetCachedModel<UILootBoxWindowModel>(UIWindowType.LootBoxWindow);
        var def = PieceType.GetDefById(chest);
        
        model.ItemAmount = $"x{1}";
        model.ItemName = LocalizationService.Get($"piece.name.{def.Abbreviations[0]}", $"piece.name.{def.Abbreviations[0]}");
        model.ItemIcon = def.Abbreviations[0];
        
        model.Probability = new List<KeyValuePair<string, string>>();

        if (data.PieceAmount > 0)
        {
            var branchAmount = 0;
            var otherAmount = 0;

            foreach (var weight in data.PieceWeights)
            {
                if (def.Branch == PieceTypeBranch.Default) continue;
                
                if (PieceType.GetDefById(weight.Piece).Branch.Has(def.Branch)) branchAmount += weight.Weight;
                else otherAmount += weight.Weight;
            }

            if (branchAmount != 0)
            {
                var amount = otherAmount == 0 ? data.PieceAmount : (int)((1 - otherAmount / (float)branchAmount) * data.PieceAmount);
                var branch = def.Branch.ToString();
                var icon = $"{branch}1";
                var message = $"x{amount} {LocalizationService.Get($"window.market.hint.{branch}", $"window.market.hint.{branch}")}";
                
                model.Probability.Add(new KeyValuePair<string, string>(icon, message));
            }
        }

        if (data.CharactersAmount.Min > 0)
        {
            var message = $"x{data.CharactersAmount.Min} {LocalizationService.Get($"window.market.hint.characters", $"window.market.hint.characters")}";
                
            model.Probability.Add(new KeyValuePair<string, string>("NPC", message));
        }

        if (data.ResourcesAmount.Min > 0)
        {
            var message = $"x{data.ResourcesAmount.Min} {LocalizationService.Get($"window.market.hint.ingredient", $"window.market.hint.ingredient")}";
                
            model.Probability.Add(new KeyValuePair<string, string>("Ingredient", message));
        }
        
        if (data.ProductionAmount.Min > 0)
        {
            var message = $"x{data.ProductionAmount.Min} {LocalizationService.Get($"window.market.hint.production", $"window.market.hint.production")}";
                
            model.Probability.Add(new KeyValuePair<string, string>("Production", message));
        }

        if (model.Probability.Count > 3)
        {
            model.Probability.RemoveRange(2, model.Probability.Count - 3);
        }
        
        UIService.Get.ShowWindow(UIWindowType.LootBoxWindow);
    }
    
    public static void OpenIslandWindow()
    {
        var model = UIService.Get.GetCachedModel<UILootBoxWindowModel>(UIWindowType.LootBoxWindow);
        
        UIService.Get.ShowWindow(UIWindowType.LootBoxWindow);
    }
}