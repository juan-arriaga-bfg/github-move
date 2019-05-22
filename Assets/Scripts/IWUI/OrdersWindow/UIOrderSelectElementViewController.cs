using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    [IWUIBinding("#OrderReadyMark")] private GameObject readyMark;
    
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
            
            if(customer.Timer.IsStarted) UpdateTimer();
        }

        order.OnStateChange += UpdateState;
        order.OnStateChange += ShowTutorArrow;
        order.OnStageChangeFromTo += OnOrderStageChangeFromTo;
        
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
        var alpha = (order.State == OrderState.InProgress || order.State == OrderState.Complete) ? 0.5f : 1f;
            
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var current = ProfileService.Current.GetStorageItem(def.Currency).Amount;

            string message;

            if (order.State == OrderState.InProgress || order.State == OrderState.Complete)
            {
                message = string.Empty;
            }
            else
            {
                
                var color = current >= def.Amount ? "00D46C" : "EC5928";
                var currentMessage = $"<color=#{color}><font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR {(current >= def.Amount ? "SubtitleGreenFinal" : "SubtitleFinal")}\">{current}</font></color>";
                message = $"{currentMessage}<size=45>/{def.Amount}</size>";
            }

            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Currency,
                LabelText = message,
                Alpha = alpha,
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
        if(entity is UIOrderElementEntity == false || GameDataService.Current.TutorialDataManager.CheckFirstOrder()) return;

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
    
    private void OnOrderStageChangeFromTo(Order order, OrderState fromState, OrderState toState)
    {
        var contentEntity = entity as UIOrderElementEntity;
        
        if (contentEntity.OnOrderStageChangeFromTo != null)
        {
            contentEntity.OnOrderStageChangeFromTo(order, fromState, toState);
        }
        
        if (fromState == OrderState.Enough && toState == OrderState.InProgress
            || fromState == OrderState.Waiting && toState == OrderState.InProgress)
        {
            JumpIngredients();
            
            btnComplete.CachedTransform.localScale = Vector3.zero;
            DOTween.Kill(btnComplete);
            btnComplete.CachedTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.InOutSine).SetId(btnComplete);
        
            btnBuy.CachedTransform.localScale = Vector3.zero;
            DOTween.Kill(btnBuy);
            btnBuy.CachedTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.InOutSine).SetId(btnBuy);
        }
        else if (fromState == OrderState.InProgress && toState == OrderState.Complete)
        {
            btnComplete.CachedTransform.localScale = Vector3.zero;
            DOTween.Kill(btnComplete);
            btnComplete.CachedTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.InOutSine).SetId(btnComplete);
        
            btnBuy.CachedTransform.localScale = Vector3.zero;
            DOTween.Kill(btnBuy);
            btnBuy.CachedTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.InOutSine).SetId(btnBuy);
        }
    }

    protected virtual void JumpIngredients()
    {
        var innerElements = container.Tabs;

            var jumpSequence = DOTween.Sequence().SetId(order);

            float jumpDuration = 1f;
            float jumpDelay = 0.1f;
            Vector3 upScale = new Vector3(2f, 2f, 2f);
            Vector3 downScale = new Vector3(1f, 1f, 1f);
            var jumpItems = new List<Transform>();

            List<Transform> particles = new List<Transform>();
            
            for (int i = innerElements.size - 1; i >= 0; i--)
            {
                var innerElement = innerElements[i] as UISimpleScrollElementViewController;
                var innerElementEntity = innerElement.Entity as UISimpleScrollElementEntity;
                
                var index = innerElements.size - 1 - i;
                
                var innerElementCopy = UIService.Get.PoolContainer.Create<RectTransform>((GameObject) ContentService.Current.GetObjectByName(innerElementEntity.ContentId));
                innerElementCopy.SetParentAndReset(innerElement.CachedTransform);
                innerElementCopy.SetParent((context as UIBaseWindowView).GetCanvas().transform, true);
                innerElementCopy.localScale = Vector3.zero;
                
                jumpItems.Add(innerElementCopy);
                
                jumpSequence.InsertCallback(Time.deltaTime * 2f, () =>
                {
                    innerElementCopy.anchorMin = innerElementCopy.anchorMax = new Vector2(0.5f, 0.5f);
                    innerElementCopy.position = innerElement.CachedTransform.position;
                    innerElementCopy.localScale = innerElement.Anchor.localScale;
                });
                jumpSequence.Insert(Time.deltaTime * 2f + index * jumpDelay, innerElementCopy.DOJump(resultAnchor.position, 3f, 1, jumpDuration).SetEase(Ease.InSine));
                jumpSequence.Insert(Time.deltaTime * 2f + index * jumpDelay, innerElementCopy.DOScale(upScale, jumpDuration * 0.5f).SetEase(Ease.InSine));
                jumpSequence.Insert(Time.deltaTime * 2f + index * jumpDelay + jumpDuration * 0.5f, innerElementCopy.DOScale(downScale, jumpDuration * 0.4f).SetEase(Ease.InSine));
                jumpSequence.Insert(Time.deltaTime * 2f + index * jumpDelay + jumpDuration * 0.9f, innerElementCopy.DOScale(Vector3.zero, jumpDuration * 0.1f).SetEase(Ease.InSine));
                jumpSequence.InsertCallback(Time.deltaTime * 2f + index * jumpDelay + jumpDuration * 0.9f, () =>
                {
                    var uiParticle = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(R.IngredientFlyUIParticle));
                    uiParticle.SetParentAndReset(resultAnchor);
                    uiParticle.position = resultAnchor.position;
                    particles.Add(uiParticle);
                });
            }
            
            jumpSequence.AppendInterval(1.8f);

            jumpSequence.OnComplete(() =>
            {
                while (particles.Count > 0)
                {
                    UIService.Get.PoolContainer.Return(particles[0].gameObject);
                    particles.RemoveAt(0);
                }
                
                for (int i = 0; i < jumpItems.Count; i++)
                {
                    var jumpItem = jumpItems[i];
                    UIService.Get.PoolContainer.Return(jumpItem.gameObject);
                }
            });
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            JumpIngredients();
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
            order.OnStageChangeFromTo -= OnOrderStageChangeFromTo;
            DOTween.Kill(order, true);
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
        readyMark.SetActive(order.State == OrderState.Complete);
        
        Fill(order.Def.Prices);
        
        if (customer?.Timer != null && order.State == OrderState.InProgress) UpdateTimer();
    }

    private void OnClickBuy()
    {
        customer.Timer.FastComplete("skip_order", order.Def.Uid);
    }

    private void OnClick()
    {
        if (order.State == OrderState.Waiting)
        {
            if (customer.Exchange()) return;
            
            order.State = OrderState.Enough;
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
            
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, order.GetAmountOfResult()))
            {
                UIErrorWindowController.AddNoFreeSpaceError();
                return;
            }

            NSAudioService.Current.Play(SoundId.OrderClaim, false, 1);
            
            isComplete = true;
            context.Controller.CloseCurrentWindow();
        }
    }
}