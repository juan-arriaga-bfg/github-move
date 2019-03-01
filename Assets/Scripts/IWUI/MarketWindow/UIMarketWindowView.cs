using System.Collections.Generic;
using BfgAnalytics;
using DG.Tweening;

public class UIMarketWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#TimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ResetButtonLabel")] private NSText btnResetLabel;
    [IWUIBinding("#ResetButton")] private UIButtonViewController btnReset;
    
    [IWUIBinding("#CloseMaskLeft")] private UIButtonViewController btnMaskLeft;
    [IWUIBinding("#CloseMaskRight")] private UIButtonViewController btnMaskRight;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMarketWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        btnResetLabel.Text = windowModel.ButtonReset;
        
        Fill(UpdateEntities(), content);
        
        content.GetScrollRect().horizontalNormalizedPosition = windowModel.IsTutorial ? 0 : 1;
        content.GetScrollRect().enabled = false;
        
        DOTween.Kill(content);
        
        content.CachedRectTransform.DOAnchorPosX(windowModel.IsTutorial ? -content.CachedRectTransform.sizeDelta.x : 0, 1.5f)
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() => { content.GetScrollRect().enabled = true; });
        
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnExecute += UpdateLabel;
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnComplete += UpdateSlots;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnReset, OnClickReset);
        
        InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        DOTween.Kill(content);
        
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnExecute -= UpdateLabel;
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnComplete -= UpdateSlots;
    }
    
    private void UpdateLabel()
    {
        var windowModel = Model as UIMarketWindowModel;
        
        timerLabel.Text =  BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.CompleteTime.GetTimeLeftText();
    }
    
    private List<IUIContainerElementEntity> UpdateEntities()
    {
        var defs = GameDataService.Current.MarketManager.Defs;
        var views = new List<IUIContainerElementEntity>(defs.Count);
        
        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];

            if (def.Current == null && def.State != MarketItemState.Lock) continue;
            
            var entity = new UIMarketElementEntity
            {
                ContentId = def.Reward.Currency,
                LabelText = $"x{def.Reward.Amount}",
                Name = LocalizationService.Get($"piece.name.{def.Reward.Currency}", $"piece.name.{def.Reward.Currency}"),
                Def = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }

    private void OnClickReset()
    {
        var windowModel = Model as UIMarketWindowModel;
        
        CurrencyHelper.Purchase(Currency.Timer.Name, 1, windowModel.Price, success =>
        {
            if(success == false) return;
            
            BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.Complete();
            Analytics.SendPurchase("skip_market", "item1", new List<CurrencyPair>{windowModel.Price}, null, false, false);
        });
    }

    private void UpdateSlots()
    {
        content.GetScrollRect().enabled = false;
        
        DOTween.Kill(content);

        var sequence = DOTween.Sequence().SetId(content);

        sequence.Append(content.CachedRectTransform.DOAnchorPosX(1200, 1f).SetEase(Ease.InBack));
        sequence.AppendCallback(() => { Fill(UpdateEntities(), content); });
        sequence.Append(content.CachedRectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack));
        sequence.AppendCallback(() => {content.GetScrollRect().enabled = true; });
    }
}