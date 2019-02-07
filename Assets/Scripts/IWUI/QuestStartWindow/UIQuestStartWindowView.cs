using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using DG.Tweening;

public class UIQuestStartWindowView : IWUIWindowView
{
    private enum Step
    {
        Unknown,
        QuestComplete,
        QuestStart
    }
    
    [SerializeField] [IWUIBinding("/canvas")] protected CanvasGroup rootCanvas;
    [SerializeField] [IWUIBinding("#BackLayer")] protected UIButtonViewController btnBackLayer;
    
    [SerializeField] private GameObject questCardPrefab;
    [SerializeField] private Transform characterConversationAnchor;
    [SerializeField] private IWUILayer uiLayer;
    [SerializeField] private QuestCardsLayoutHelper questCardsLayoutHelper;
    
    private readonly List<UIQuestCard> questCards = new List<UIQuestCard>();

    private UICharactersConversationViewController conversation;

    private bool isClickAllowed;

    private Step step;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        WarmUpPool();
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();

        CleanUp();
        
        UIQuestStartWindowModel model = Model as UIQuestStartWindowModel;
        
        step = model.CompletedQuest != null ? Step.QuestComplete : Step.QuestStart;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        if(btnBackLayer != null) btnBackLayer.ToState(GenericButtonState.Active).OnClick(OnClick);
    }

    private void CleanUp()
    {
        step = Step.Unknown;
        
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
        UIQuestStartWindowModel model = Model as UIQuestStartWindowModel;
        
        base.AnimateShow();
        
        rootCanvas.alpha = 0;

        var delay = model.QuestCompletedScenario != null ? 1f : 0f;
        
        DOTween.Sequence()
               .AppendInterval(delay)
               .AppendCallback(() =>
                {
                    //todo: remove hack
                    FindObjectOfType<UIMainWindowView>().FadeAuxButtons(false);
                    
                    PlayNextScenario();
                    rootCanvas.DOFade(1f, 0.1f);
                })
            ;
    }

    public override void AnimateClose()
    {
        base.AnimateClose();

        //todo: remove hack
        FindObjectOfType<UIMainWindowView>().FadeAuxButtons(true);
        
        rootCanvas.DOFade(0f, 0.5f);
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
        
        if (model.QuestsToStart != null)
        {
            questCardsLayoutHelper.FixLayout();

            var index = 0;
            
            // Create cards
            foreach (var quest in model.QuestsToStart)
            {
                if (payload.QuestIds != null && !payload.QuestIds.Contains(quest.Id))
                {
                    continue;
                }
                
                var card = pool.Create<UIQuestCard>(questCardPrefab);
                card.gameObject.SetActive(true);
                card.transform.SetParent(questCardPrefab.transform.parent, false);
                card.transform.SetAsLastSibling();
                card.Init(quest);

                questCards.Add(card);
                
                cardsAdded = true;

                // Animate
                
                var canvasGroup = card.GetCanvasGroup();
                var finalScale = questCardPrefab.transform.localScale;
                
                var sequence = DOTween.Sequence().SetId(this);
                
                canvasGroup.alpha = 0;
                card.transform.localScale = Vector3.zero;
                
                sequence.Insert(0.1f * index, canvasGroup.DOFade(1, ANIMATION_TIME).SetEase(Ease.OutSine));
                sequence.Insert(0.1f * index, card.transform.DOScale(finalScale, ANIMATION_TIME).SetEase(Ease.OutElastic));
                
                index++;
            }

            //questCardsLayoutHelper.FixLayoutAtTheNextFrame();
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
        if (model.QuestsToStart == null || model.QuestsToStart.Count == 0)
        {
            return;
        }
        
        List<string> ids = model.QuestsToStart.Select(e => e.Id).ToList();
        GameDataService.Current.QuestsManager.StartQuests(ids);

        foreach (var id in ids)
        {
            Analytics.SendQuestStartEvent(id);
        }
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
    
    private void PlayNextScenario()
    {
        var model = Model as UIQuestStartWindowModel;
        
        if (conversation == null)
        {
            conversation = UIService.Get.GetCachedObject<UICharactersConversationViewController>(R.UICharacterConversationView);
            conversation.transform.SetParent(characterConversationAnchor, false);
        }

        if (step == Step.QuestComplete)
        {
            NSAudioService.Current.Play(SoundId.QuestCompleted);
        }
        
        var scenario = step == Step.QuestComplete ? model.QuestCompletedScenario : model.QuestStartScenario;
        conversation.PlayScenario(scenario, OnActionStarted, OnActionEnded, OnScenarioComplete);
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
            NSAudioService.Current.Play(SoundId.QuestNew);
            CreateQuestCards(Model as UIQuestStartWindowModel, payload1, () => { isClickAllowed = true; });
            return;
        }
        
        var payload2 = act.GetComponent<ConversationActionPayloadProvideRewardComponent>(ConversationActionPayloadComponent.ComponentGuid);
        if (payload2 != null)
        {
            Analytics.SendQuestCompletedEvent(payload2.QuestId);
            
            ProvideReward(() =>
            {
                isClickAllowed = true;
                conversation.OnClick();
            });
            return;
        }

        isClickAllowed = true;
    }
    
    private void OnScenarioComplete()
    {
        var model = Model as UIQuestStartWindowModel;
        
        isClickAllowed = false;

        if (step == Step.QuestComplete)
        {
            if (model.QuestsToStart != null && model.QuestsToStart.Count > 0)
            {
                
            }
            else
            {
                var questManager = GameDataService.Current.QuestsManager;
            
                string starterId;
                List<string> questsToStart = questManager.CheckConditions(out starterId);
                if (questsToStart.Count > 0)
                {
                    model.Init(null, questsToStart, starterId);
                    model.QuestStartScenario.Continuation = true;
                    step = Step.QuestStart;
                }
                else
                {
                    Controller.CloseCurrentWindow();
                    return;
                }
            }

            PlayNextScenario();
        }
        else
        {
            Controller.CloseCurrentWindow();
        }

    }

    private void ProvideReward(Action onComplete)
    {
        var model = Model as UIQuestStartWindowModel;
        
        isClickAllowed = false;

        var quest = model.CompletedQuest;

        quest.SetClaimedState();
        GameDataService.Current.QuestsManager.FinishQuest(quest.Id);

        List<CurrencyPair> reward = quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        int curLayer = uiLayer.CurrentLayer;
        uiLayer.CurrentLayer = 10;

        Vector3 pos = conversation.GetLeftBubbleAnchor().position;
        var point = uiLayer.ViewCamera.WorldToScreenPoint(pos);
        point.x -= 350;
        point.x += 70;
        
        CurrencyHelper.Purchase(reward, success =>
        {
            uiLayer.CurrentLayer = curLayer;
            onComplete();
        },
        point);
    }
}