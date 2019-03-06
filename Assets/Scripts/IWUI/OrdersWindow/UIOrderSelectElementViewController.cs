using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIOrderSelectElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#ResultAnchor")] private Transform resultAnchor;
    [IWUIBinding("#Hero")] private Transform iconHero;
    [IWUIBinding("#TutorAnchor")] private Transform tutorAnchor;
    
    [IWUIBinding("#TimerLabel")] private NSText timerLabel;
    
    [IWUIBinding("#CompleteButtonLabel")] private NSText btnCompleteLabel;
    [IWUIBinding("#BuyButtonLabel")] private NSText btnBuyLabel;
    
    [IWUIBinding("#CompleteButton")] private UIButtonViewController btnComplete;
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
    
    [IWUIBinding("#Timer")] private GameObject timer;
    [IWUIBinding("#Shine")] private GameObject shine;
    
    [IWUIBinding("#Content")] private UIContainerViewController container;
    
    private Transform result;
    private Order order;
    private CustomerComponent customer;
    private bool isComplete;
    
    public override void Init()
    {
        var contentEntity = entity as UIOrderElementEntity;
        
        RemoveListeners();
        
        order = contentEntity.Data;
        isComplete = false;

        CreateResultIcon(resultAnchor, order.Def.Uid);
        
        var board = BoardService.Current.FirstBoard;
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
        var piece = board.BoardLogic.GetPieceAt(position);
        
        customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
        
        if (customer.Timer != null)
        {
            customer.Timer.OnTimeChanged += UpdateTimer;
            customer.Timer.OnComplete += UpdateState;
            customer.Timer.OnComplete += PlaySound;
            UpdateTimer();
        }

        order.OnStateChange += UpdateState;
        order.OnStateChange += ShowTutorArrow;
        
        UpdateState();
        
        if((context as UIOrdersWindowView).IsShowComplete) ShowTutorArrow();
        
        btnBuy.OnClick(OnClickBuy);
        btnComplete.OnClick(OnClick);
        
        CreateIcon(iconHero, $"{PieceType.Parse(piece.PieceType)}Icon");
        
        base.Init();
    }

    private void PlaySound()
    {
        NSAudioService.Current.Play(SoundId.OrderComplete, false, 1);
    }
    
    private void CreateResultIcon(Transform parent, string id)
    {
        if (result != null)
        {
            UIService.Get.PoolContainer.Return(result.gameObject);
        }
        
        result = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        result.SetParentAndReset(parent);
    }
    
    private void Fill(List<CurrencyPair> entities)
    {
        if (entities == null || entities.Count <= 0)
        {
            container.Clear();
            return;
        }
        
        var views = new List<IUIContainerElementEntity>(entities.Count);
        var alpha = (order.State == OrderState.InProgress || order.State == OrderState.Complete) ? 0.7f : 1f;
            
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var current = ProfileService.Current.GetStorageItem(def.Currency).Amount;

            string message;
            string color;
            bool mark;

            if (order.State == OrderState.InProgress || order.State == OrderState.Complete)
            {
                mark = false;
                message = string.Empty;
            }
            else
            {
                if (current >= def.Amount)
                {
                    mark = true;
                    message = string.Empty;
                }
                else
                {
                    mark = false;
                    color = "EC5928";
                    message = $"<color=#{color}>{current}</color><size=45>/{def.Amount}</size>";
                } 
            }

            var entity = new UIOrderIngredientElementEntity
            {
                ContentId = def.Currency,
                LabelText = message,
                Alpha = alpha,
                Mark = mark,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        container.Create(views);
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        ShowTutorArrow();
    }

    private void ShowTutorArrow()
    {
        if(entity is UIOrderElementEntity == false || BoardService.Current.FirstBoard.TutorialLogic.CheckFirstOrder()) return;

        switch (order.State)
        {
            case OrderState.InProgress:
                (context as UIBaseWindowView).CachedHintArrowComponent.HideArrow(tutorAnchor);
                break;
            case OrderState.Waiting:
            case OrderState.Enough:
            case OrderState.Complete:
                (context as UIBaseWindowView).CachedHintArrowComponent.ShowArrow(tutorAnchor, 5f);
                break;
        }
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        RemoveListeners();
        
        if (isComplete) customer.GetReward();

        isComplete = false;
        customer = null;
    }

    private void RemoveListeners()
    {
        if (customer?.Timer != null)
        {
            customer.Timer.OnTimeChanged -= UpdateTimer;
            customer.Timer.OnComplete -= UpdateState;
            customer.Timer.OnComplete -= PlaySound;
        }

        if (order != null)
        {
            order.OnStateChange -= UpdateState;
            order.OnStateChange -= ShowTutorArrow;
        }
    }

    private void UpdateTimer()
    {
        timerLabel.Text = customer.Timer.CompleteTime.GetTimeLeftText();

        btnBuyLabel.Text = customer.Timer.IsFree() 
            ? LocalizationService.Get("common.button.free", "common.button.free") 
            : customer.Timer.GetPrice().ToStringIcon();
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
    }

    private void OnClickBuy()
    {
        customer.Timer.FastComplete("skip_order");
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
            var board = BoardService.Current.FirstBoard;
            var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
            
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, order.PiecesReward.Sum(e => e.Value)))
            {
                UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
                return;
            }

            NSAudioService.Current.Play(SoundId.OrderClaim, false, 1);
            
            isComplete = true;
            context.Controller.CloseCurrentWindow();
        }
    }
}