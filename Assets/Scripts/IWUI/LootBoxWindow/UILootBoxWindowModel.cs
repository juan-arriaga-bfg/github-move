using System.Collections.Generic;

public class UILootBoxWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.market.hint.title", "window.market.hint.title");
    public string Message => LocalizationService.Get("window.market.hint.message", "window.market.hint.message");
    public string Button => LocalizationService.Get("common.button.ok", "common.button.ok");

    public string ItemAmount;
    public string ItemName;
    public string ItemIcon;

    public bool IsIsland;

    public List<KeyValuePair<string, string>> Probability;
}
