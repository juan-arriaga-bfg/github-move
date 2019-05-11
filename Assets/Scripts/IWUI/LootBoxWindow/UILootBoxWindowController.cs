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

    public static void OpenProbabilityWindow(List<int> chests)
    {
        var model = UIService.Get.GetCachedModel<UILootBoxWindowModel>(UIWindowType.LootBoxWindow);
        var probability = new Dictionary<string, KeyValuePair<int, string>>();
        
        foreach (var chest in chests)
        {
            var data = GameDataService.Current.ChestsManager.GetChest(chest);

            var def = PieceType.GetDefById(chest);
            
            model.ItemAmount = $"x{chests.Count}";
            model.ItemName = LocalizationService.Get($"piece.name.{def.Abbreviations[0]}", $"piece.name.{def.Abbreviations[0]}");
            model.ItemIcon = def.Abbreviations[0];

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
                    var amount = otherAmount == 0 ? data.PieceAmount : (int) ((1 - otherAmount / (float) branchAmount) * data.PieceAmount);
                    var branch = def.Branch.ToString();
                    var key = $"{branch}1";

                    Replace(key, amount, $"window.market.hint.{branch}", probability);
                }
            }

            if (data.CharactersAmount.Min > 0)
            {
                Replace("NPC", data.CharactersAmount.Min, "window.market.hint.characters", probability);
            }

            if (data.ResourcesAmount.Min > 0)
            {
                Replace("Ingredient", data.ResourcesAmount.Min, "window.market.hint.ingredient", probability);
            }

            if (data.ProductionAmount.Min > 0)
            {
                Replace("Production", data.ProductionAmount.Min, "window.market.hint.production", probability);
            }
        }
        
        model.Probability = new List<KeyValuePair<string, string>>();

        AddSlot("NPC", probability, model.Probability);
        AddSlot("Ingredient", probability, model.Probability);
        AddSlot("Production", probability, model.Probability);
        
        foreach (var pair in probability)
        {
            model.Probability.Insert(0, new KeyValuePair<string, string>(pair.Key, pair.Value.Value));
        }

        if (model.Probability.Count > 3)
        {
            model.Probability.RemoveRange(2, model.Probability.Count - 3);
        }
        
        UIService.Get.ShowWindow(UIWindowType.LootBoxWindow);
    }

    private static void Replace(string key, int amount, string messageKey, Dictionary<string, KeyValuePair<int, string>> dict)
    {
        if (dict.TryGetValue(key, out var value) == false)
        {
            value = new KeyValuePair<int, string>(amount, $"x{amount} {LocalizationService.Get(messageKey, messageKey)}");
            dict.Add(key, value);
            return;
        }
        
        amount += value.Key;
        dict[key] = new KeyValuePair<int, string>(amount, $"x{amount} {LocalizationService.Get(messageKey, messageKey)}");
    }

    private static void AddSlot(string key, Dictionary<string, KeyValuePair<int, string>> dict, List<KeyValuePair<string, string>> list)
    {
        if (dict.TryGetValue(key, out var data) == false) return;
        
        list.Add(new KeyValuePair<string, string>(key, data.Value));
        dict.Remove(key);
    }
}