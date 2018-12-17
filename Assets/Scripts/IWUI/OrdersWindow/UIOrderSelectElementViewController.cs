﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderSelectElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#ResultIcon")] private Image iconResult;
    [IWUIBinding("#Hero")] private Transform iconHero;
    [IWUIBinding("#TutorAnchor")] private Transform tutorAnchor;
    
    [IWUIBinding("#TimerLabel")] private NSText timerLabel;
    
    [IWUIBinding("#CompleteButtonLabel")] private NSText btnCompleteLabel;
    [IWUIBinding("#BuyButtonLabel")] private NSText btnBuyLabel;
    
    [IWUIBinding("#CompleteButton")] private UIButtonViewController btnComplete;
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
    
    [IWUIBinding("#Timer")] private GameObject timer;
    [IWUIBinding("#Shine")] private GameObject shine;
    
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    private Order order;
    private CustomerComponent customer;
    private bool isComplete;
    
    public override void Init()
    {
        var contentEntity = entity as UIOrderElementEntity;
        
        RemoveListeners();
        
        order = contentEntity.Data;
        isComplete = false;
        
        iconResult.sprite = IconService.Current.GetSpriteById(order.Def.Uid);
        
        var board = BoardService.Current.GetBoardById(0);
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
        var piece = board.BoardLogic.GetPieceAt(position);
        
        customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
        
        if (customer.Timer != null)
        {
            customer.Timer.OnExecute += UpdateTimer;
            customer.Timer.OnComplete += UpdateState;
        }

        order.OnStateChange += UpdateState;
        order.OnStateChange += ShowTestArrow;
        UpdateState();
        
        btnBuy.Init().OnClick(OnClickBuy);
        btnComplete.Init().OnClick(OnClick);
        
        CreateIcon(iconHero, PieceType.Parse(piece.PieceType));
        
        base.Init();
    }
    
    private void Fill(List<CurrencyPair> entities)
    {
        if (entities == null || entities.Count <= 0)
        {
            content.Clear();
            return;
        }
        
        var views = new List<IUIContainerElementEntity>(entities.Count);
        var alpha = (order.State == OrderState.InProgress || order.State == OrderState.Complete) ? 0.7f : 1f;
            
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var current = ProfileService.Current.GetStorageItem(def.Currency).Amount;
            var color = current == def.Amount ? "FFFFFF" : (current > def.Amount ? "28EC6D" : "EC5928");
            var message = current >= def.Amount ? $"<sprite name={OrderState.Complete}>" :  $"<color=#{color}>{current}</color><size=45>/{def.Amount}</size>";
            
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Currency,
                LabelText = (order.State == OrderState.InProgress || order.State == OrderState.Complete) ? "" : message,
                Alpha = alpha,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        content.Create(views);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();

        ShowTestArrow();
    }

    private void ShowTestArrow()
    {
        var contentEntity = entity as UIOrderElementEntity;

        if (contentEntity == null || order.State != OrderState.Complete || BoardService.Current.FirstBoard.TutorialLogic.CheckFirstOrder()) return;
        
        (context as UIBaseWindowView).CachedHintArrowComponent.ShowArrow(tutorAnchor, 5f);
    }

    public override void OnViewCloseCompleted()
    {
        RemoveListeners();

        if (isComplete) customer.GetReward();

        isComplete = false;
        customer = null;
    }

    private void RemoveListeners()
    {
        if (customer?.Timer != null)
        {
            customer.Timer.OnExecute -= UpdateTimer;
            customer.Timer.OnComplete -= UpdateState;
        }

        if (order != null)
        {
            order.OnStateChange -= UpdateState;
            order.OnStateChange -= ShowTestArrow;
        }
    }

    private void UpdateTimer()
    {
        timerLabel.Text = customer.Timer.CompleteTime.GetTimeLeftText();
        btnBuyLabel.Text = customer.Timer.GetPrise().ToStringIcon();
    }
    
    private void UpdateState()
    {
        btnCompleteLabel.Text = order.State != OrderState.Complete
            ? LocalizationService.Get("common.button.produce", "common.button.produce")
            : LocalizationService.Get("common.button.claim", "common.button.claim");
        
        timer.SetActive(order.State == OrderState.InProgress);
        btnBuy.gameObject.SetActive(order.State == OrderState.InProgress);
        btnComplete.gameObject.SetActive(order.State != OrderState.InProgress);
        shine.SetActive(order.State == OrderState.Complete);
        
        Fill(order.Def.Prices);
        
        (context as UIOrdersWindowView).UpdateOrders();
    }

    private void OnClickBuy()
    {
        customer.Timer.FastComplete();
    }

    private void OnClick()
    {
        if (order.State == OrderState.Waiting)
        {
            customer.Exchange();
            return;
        }

        if (order.State == OrderState.Enough)
        {
            customer.Buy();
            return;
        }

        if (order.State == OrderState.Complete)
        {
            var board = BoardService.Current.GetBoardById(0);
            var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
            
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, order.PiecesReward.Sum(e => e.Value)))
            {
                UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
                return;
            }
            
            isComplete = true;
            context.Controller.CloseCurrentWindow();
        }
    }
}