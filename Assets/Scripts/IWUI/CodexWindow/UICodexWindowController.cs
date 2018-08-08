using UnityEngine;
using System.Collections;

public class UICodexWindowController : IWWindowController 
{
    public override IWWindowModel CreateModel()
    {
        UICodexWindowModel windowModel = new UICodexWindowModel();
        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }

    public void OnRewardClick()
    {
        var model = (UICodexWindowModel)Model;

        CurrencyHellper.Purchase(new CurrencyPair {Currency = Currency.Coins.Name, Amount = model.PendingRewardAmount},
                                 isSuccess =>
                                 {
                                     Save();
                                     CloseCurrentWindow();
                                 });
    }

    private void Save()
    {
        var items = GameDataService.Current.CodexManager.Items;
        foreach (var item in items)
        {
            item.Value.PendingReward = item.Value.Unlocked;
        }
    }
}
