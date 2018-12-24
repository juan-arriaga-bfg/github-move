using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class UIMarketWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#ResetButtonLabel")] private NSText btnResetLabel;
    [IWUIBinding("#ResetButton")] private UIButtonViewController btnReset;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMarketWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnResetLabel.Text = windowModel.ButtonReset;
        
        Fill(UpdateEntities(windowModel.Chests), content);
        
        content.CachedRectTransform.anchoredPosition = new Vector2(-375, 0);
        content.GetScrollRect().enabled = false;
        
        DOTween.Kill(content);
        
        content.CachedRectTransform.DOAnchorPosX(0, 1.5f)
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() => { content.GetScrollRect().enabled = true; });
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnReset, OnClickReset);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIMarketWindowModel;
        
        DOTween.Kill(content);
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<ChestDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UIChestsShopElementEntity
            {
                ContentId = def.Uid,
                LabelText = LocalizationService.Get($"piece.name.{def.Uid}", $"piece.name.{def.Uid}"),
                Chest = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }

    private void OnClickReset()
    {
        
    }
}