using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class UIQuestStartWindowView : IWUIWindowView
{
    [SerializeField] private GameObject questCardPrefab;
    [SerializeField] private Transform characterConversationAnchor;
    
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
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        CreateConversation(windowModel);
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
            }
        }
        
        onComplete?.Invoke();
    }

    public override void OnViewClose()
    {
        StartQuests();
        
        base.OnViewClose();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        
    }

    private void StartQuests()
    {
        List<string> ids = (Model as UIQuestStartWindowModel).Quests.Select(e => e.Id).ToList();
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
    
    private void CreateConversation(UIQuestStartWindowModel windowModel)
    {
        conversation = UIService.Get.GetCachedObject<UICharactersConversationViewController>(R.UICharacterConversationView);
        conversation.transform.SetParent(characterConversationAnchor, false);

        string char1Id = UiCharacterData.CharSleepingBeauty;
        string char2Id = UiCharacterData.CharRapunzel;
        string char3Id = UiCharacterData.CharGnomeWorker;
        string char4Id = UiCharacterData.CharPussInBoots;
        
        conversation.AddCharacter(char4Id, CharacterPosition.RightOuter, false, false);
        conversation.AddCharacter(char3Id, CharacterPosition.RightInner, false, false);
        conversation.AddCharacter(char2Id, CharacterPosition.LeftOuter,  false, false);
        conversation.AddCharacter(char1Id, CharacterPosition.LeftInner,  false, false);

        ConversationScenarioEntity scenario = new ConversationScenarioEntity();
        scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char1Id,
                Message = "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid..."
            }
        });
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char2Id,
                Message = "2222"
            }
        };
        actBubble.RegisterComponent(new ConversationActionPayloadShowQuestComponent
        {
            QuestIds = windowModel.Quests.Select(e => e.Id).ToList()
        });
        scenario.RegisterComponent(actBubble);
        
        scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char3Id,
                Message = "33333"
            }
        });
        scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char4Id,
                Message = "444444"
            }
        });
            
        
        conversation.PlayScenario(scenario, OnActionStarted, OnActionEnded, OnScenarioComplete);
    }

    private void OnScenarioComplete()
    {
        Controller.CloseCurrentWindow();
    }

    private void OnActionEnded(ConversationActionEntity act)
    {
        var payload = act.GetComponent<ConversationActionPayloadShowQuestComponent>(ConversationActionPayloadComponent.ComponentGuid);
        if (payload == null)
        {
            isClickAllowed = true;
            return;
        }

        CreateQuestCards(Model as UIQuestStartWindowModel, payload, () =>
        {
            isClickAllowed = true;
        });
    }

    private void OnActionStarted(ConversationActionEntity act)
    {
        isClickAllowed = false;
    }
}