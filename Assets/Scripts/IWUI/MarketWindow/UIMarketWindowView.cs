using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class UIMarketWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#TimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ResetButtonLabel")] private NSText btnResetLabel;
    [IWUIBinding("#ResetButton")] private UIButtonViewController btnReset;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMarketWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        Fill(UpdateEntities(windowModel.Slots), content);
        
        content.CachedRectTransform.anchoredPosition = new Vector2(-375, 0);
        content.GetScrollRect().enabled = false;
        
        DOTween.Kill(content);
        
        content.CachedRectTransform.DOAnchorPosX(0, 1.5f)
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() => { content.GetScrollRect().enabled = true; });
        
        BoardService.Current.FirstBoard.MarketLogic.Timer.OnExecute += UpdateLabel;
        BoardService.Current.FirstBoard.MarketLogic.Timer.OnComplete += Reset;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnReset, OnClickReset);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        DOTween.Kill(content);
        
        BoardService.Current.FirstBoard.MarketLogic.Timer.OnExecute -= UpdateLabel;
        BoardService.Current.FirstBoard.MarketLogic.Timer.OnComplete -= Reset;
    }
    
    private void UpdateLabel()
    {
        var windowModel = Model as UIMarketWindowModel;
        
        timerLabel.Text =  BoardService.Current.FirstBoard.MarketLogic.Timer.CompleteTime.GetTimeLeftText();
        btnResetLabel.Text = windowModel.ButtonReset;
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<MarketDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];

            if (def == null) continue;
            
            var entity = new UIMarketElementEntity
            {
                ContentId = def.Weight.Uid,
                LabelText = $"x{def.Amount}",
                Name = LocalizationService.Get($"piece.name.{def.Weight.Uid}", $"piece.name.{def.Weight.Uid}"),
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
        BoardService.Current.FirstBoard.MarketLogic.Timer.FastComplete();
    }

    private void Reset()
    {
        BoardService.Current.FirstBoard.MarketLogic.Timer.Start();
        content.GetScrollRect().enabled = false;
        
        DOTween.Kill(content);

        var sequence = DOTween.Sequence().SetId(content);

        sequence.Append(content.CachedRectTransform.DOAnchorPosX(1200, 1f).SetEase(Ease.InBack));
        sequence.AppendCallback(() =>
        {
            var windowModel = Model as UIMarketWindowModel;

            Fill(UpdateEntities(windowModel.Slots), content);
        });
        
        sequence.Append(content.CachedRectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack));
        sequence.AppendCallback(() => {content.GetScrollRect().enabled = true; });
    }
}