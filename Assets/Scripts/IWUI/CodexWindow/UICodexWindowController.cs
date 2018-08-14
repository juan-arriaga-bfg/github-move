using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Touch;

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

        CurrencyHellper.Purchase(new CurrencyPair {Currency = Currency.Coins.Name, Amount = GameDataService.Current.CodexManager.GetCodexContent().PendingRewardAmount},
                                 isSuccess =>
                                 {
                                     Save();
                                     CloseCurrentWindow();
                                     
                                     model.OnClaim?.Invoke();
                                 }, LeanTouch.Fingers.First()?.LastScreenPosition);
    }

    private void Save()
    {
        var codexManager = GameDataService.Current.CodexManager;
        
        var items = codexManager.Items;
        foreach (var item in items)
        {
            item.Value.PendingReward.Clear();
        }
        
        codexManager.ClearCodexContentCache();
        codexManager.CodexState = CodexState.Normal;
    }
}
