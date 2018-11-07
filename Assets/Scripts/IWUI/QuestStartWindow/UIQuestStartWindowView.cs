using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;

public class UIQuestStartWindowView : IWUIWindowView
{
    [SerializeField] private GameObject questCardPrefab;
    [SerializeField] private Transform characterConversationAnchor;
    [SerializeField] private CanvasGroup questCardsCanvasGroup;
    [SerializeField] private CanvasGroup rootCanvasGroup;
    
    private readonly List<UIQuestCard> questCards = new List<UIQuestCard>();

    private UICharactersConversationViewController conversation;

    private bool isClickAllowed;
       
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        WarmUpPool();
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();

        CleanUp();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;

    }

    private void CleanUp()
    {
        var pool = UIService.Get.PoolContainer;
        
        foreach (var card in questCards)
        {
            pool.Return(card.gameObject);
        }
        
        questCards.Clear();
    }

    public override void OnViewClose()
    {
        StartQuests();
        
        base.OnViewClose();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        
    }

    public override void AnimateShow()
    {
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        
        base.AnimateShow();
        
        rootCanvasGroup.alpha = 0;

        DOTween.Sequence()
               .AppendInterval(1f)
               .AppendCallback(() =>
                {
                    CreateConversation(windowModel);
                    rootCanvasGroup.DOFade(1f, 0.5f);
                })
            ;
    }

    public override void AnimateClose()
    {
        base.AnimateClose();

        rootCanvasGroup.DOFade(0f, 0.5f);
    }

    private void CreateQuestCards(UIQuestStartWindowModel model, ConversationActionPayloadShowQuestComponent payload, Action onComplete)
    {
        var pool = UIService.Get.PoolContainer;
        
        // Cleanup
        foreach (var card in questCards)
        {
            pool.Return(card.gameObject);
        }
        questCards.Clear();

        bool cardsAdded = false;

        const float ANIMATION_TIME = 0.5f;
        
        if (model.Quests != null)
        {
            // Create cards
            foreach (var quest in model.Quests)
            {
                if (payload.QuestIds != null && !payload.QuestIds.Contains(quest.Id))
                {
                    continue;
                }
                
                UIQuestCard card = pool.Create<UIQuestCard>(questCardPrefab);
                card.gameObject.SetActive(true);
                card.transform.SetParent(questCardPrefab.transform.parent, false);
                card.Init(quest);

                questCards.Add(card);
                
                cardsAdded = true;

                // Animate
                var canvasGroup = card.GetCanvasGroup();
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, ANIMATION_TIME)
                           .SetEase(Ease.OutSine);
                
                card.transform.localScale = Vector3.zero;
                card.transform.DOScale(Vector3.one, ANIMATION_TIME)
                    .SetEase(Ease.OutElastic);

            }
        }

        DOTween.Sequence()
               .AppendInterval(cardsAdded ? ANIMATION_TIME : 0)
               .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
    }

    private void StartQuests()
    {
        var model = (Model as UIQuestStartWindowModel);
        if (model.Quests == null || model.Quests.Count == 0)
        {
            return;
        }
        
        List<string> ids = model.Quests.Select(e => e.Id).ToList();
        GameDataService.Current.QuestsManager.StartQuests(ids);
    }

    private void WarmUpPool()
    {
        var pool = UIService.Get.PoolContainer;

        for (int i = 1; i <= 3; i++)
        {
            UIQuestCard tabInstance = pool.Create<UIQuestCard>(questCardPrefab);
            pool.Return(tabInstance.gameObject);
        }
        
        questCardPrefab.SetActive(false);
    }

    public void OnClick()
    {
        if (!isClickAllowed)
        {
            return;
        }
        
        if (conversation != null)
        {
            conversation.OnClick();
        }
    }
    
    private void CreateConversation(UIQuestStartWindowModel model)
    {
        if (conversation == null)
        {
            conversation = UIService.Get.GetCachedObject<UICharactersConversationViewController>(R.UICharacterConversationView);
            conversation.transform.SetParent(characterConversationAnchor, false);
        }

        conversation.PlayScenario(model.Scenario, OnActionStarted, OnActionEnded, OnScenarioComplete);
    }

    private void OnActionStarted(ConversationActionEntity act)
    {
        isClickAllowed = false;
    }
    
    private void OnActionEnded(ConversationActionEntity act)
    {
        var payload1 = act.GetComponent<ConversationActionPayloadShowQuestComponent>(ConversationActionPayloadComponent.ComponentGuid);
        if (payload1 != null)
        {
            CreateQuestCards(Model as UIQuestStartWindowModel, payload1, () => { isClickAllowed = true; });
            return;
        }
        //
        // var payload2 = act.GetComponent<ConversationActionPayloadStartNewQuestsIfAnyComponent>(ConversationActionPayloadComponent.ComponentGuid);
        // if (payload2 != null)
        // {
        //     GameDataService.Current.QuestsManager.StartNewQuestsIfAny();
        //     isClickAllowed = true;
        //     return;
        // }

        isClickAllowed = true;
    }
    
    private void OnScenarioComplete()
    {
        isClickAllowed = false;
        Controller.CloseCurrentWindow();
    }

}