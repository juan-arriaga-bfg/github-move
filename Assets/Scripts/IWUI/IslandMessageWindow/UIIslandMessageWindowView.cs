using System.Collections.Generic;

public class UIIslandMessageWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#ButtonBuyLabel")] private NSText btnBuyLabel;
    
    [IWUIBinding("#ButtonBuy")] private UIButtonViewController btnBuy;
    [IWUIBinding("#ButtonInfo")] private UIButtonViewController btnInfo;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIIslandMessageWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnBuyLabel.Text = windowModel.Button;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnBuy, OnBuyClick);
        InitButtonBase(btnInfo, OnInfoClick);
    }

    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIIslandMessageWindowModel;
        
        windowModel.OnClose?.Invoke();
        
        base.OnViewCloseCompleted();
    }

    private void OnBuyClick()
    {
        var windowModel = Model as UIIslandMessageWindowModel;
        
        windowModel.OnAccept?.Invoke();
        Controller.CloseCurrentWindow();
    }

    private void OnInfoClick()
    {
        var pieces = GameDataService.Current.FieldManager.IslandPieces;
        var result = new List<int>();

        foreach (var pair in pieces)
        {
            var def = PieceType.GetDefById(pair.Key);

            if (def.Filter.Has(PieceTypeFilter.Chest) == false) continue;

            for (var i = 0; i < pair.Value.Count; i++)
            {
                result.Add(pair.Key);
            }
        }
        
        UILootBoxWindowController.OpenProbabilityWindow(result);
    }
}