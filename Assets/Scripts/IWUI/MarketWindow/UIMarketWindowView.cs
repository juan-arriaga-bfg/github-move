using System.Collections.Generic;
using BfgAnalytics;
using DG.Tweening;
using UnityEngine;

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

        Scroll(windowModel.IsTutorial ? content.Tabs.size : 0);
        
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnExecute += UpdateLabel;
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnComplete += UpdateSlots;
    }

    public void Scroll(int index)
    {
        DOTween.Kill(content);

        var posX = Mathf.Clamp((-content.CachedRectTransform.sizeDelta.x / content.Tabs.size) * index, -content.CachedRectTransform.sizeDelta.x, 0);
        
        content.GetScrollRect().enabled = false;
        content.CachedRectTransform.DOAnchorPosX(posX, 1.5f)
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() => { content.GetScrollRect().enabled = true; });
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnReset, OnClickReset);
        
        InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);
        
        TackleBoxEvents.SendMarketOpen();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        DOTween.Kill(content);
        
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnExecute -= UpdateLabel;
        BoardService.Current.FirstBoard.MarketLogic.ResetMarketTimer.OnComplete -= UpdateSlots;
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        TackleBoxEvents.SendMarketClosed();
    }

    private void UpdateLabel()
    {
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
                ContentId = def.Icon,
                LabelText = $"x{def.Reward.Amount}",
                Name = LocalizationService.Get(def.Name, def.Name),
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