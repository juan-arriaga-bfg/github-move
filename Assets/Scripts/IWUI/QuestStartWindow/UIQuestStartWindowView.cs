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
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        WarmUpPool();
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        CreateQuestCards(windowModel);
        CreateConversation(windowModel);
    }

    private void CreateQuestCards(UIQuestStartWindowModel model)
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
                UIQuestCard card = pool.Create<UIQuestCard>(questCardPrefab);
                card.gameObject.SetActive(true);
                card.transform.SetParent(questCardPrefab.transform.parent, false);
                card.Init(quest);

                questCards.Add(card);
            }
        }
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

    private void CreateConversation(UIQuestStartWindowModel windowModel)
    {
        UICharactersConversationViewController conversation = UIService.Get.GetCachedObject<UICharactersConversationViewController>(R.UICharacterConversationView);
        conversation.transform.SetParent(characterConversationAnchor, false);

        string char1Id = UiCharacterData.CharSleepingBeauty;
        string char2Id = UiCharacterData.CharRapunzel;
        string char3Id = UiCharacterData.CharGnomeWorker;
        string char4Id = UiCharacterData.CharPussInBoots;
        
        conversation.AddCharacter(char4Id, CharacterPosition.RightOuter, false, false);
        conversation.AddCharacter(char3Id, CharacterPosition.RightInner, false, false);
        conversation.AddCharacter(char2Id, CharacterPosition.LeftOuter,  false, false);
        conversation.AddCharacter(char1Id, CharacterPosition.LeftInner,  false, false);
        
        conversation.NextBubble(R.UICharacterBubbleMessageView, new UiCharacterBubbleDefMessage {CharacterId = char1Id, 
            Message = "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid..."}, () =>
        {
            conversation.NextBubble(R.UICharacterBubbleMessageView, new UiCharacterBubbleDefMessage {CharacterId = char2Id, 
                Message = "22222"}, () =>
            {
                conversation.NextBubble(R.UICharacterBubbleMessageView, new UiCharacterBubbleDefMessage {CharacterId = char3Id, 
                    Message = "3333"}, () =>
                {
                    conversation.NextBubble(R.UICharacterBubbleMessageView, new UiCharacterBubbleDefMessage {CharacterId = char4Id, 
                        Message = "4444"}, () =>
                    {
                        // Done!
                    });
                });
            });
        });
    }
}

public class ScenarioAction
{
    
}

public class ScenarioActionAddChar
{
    public CharacterPosition Position;
    public string CharId;
}

public class ScenarioActionBubble
{
    public string Message;
}

public class Scenario
{
    [JsonIgnore] private int index;
    
    private List<ScenarioAction> actions;

    public ScenarioAction GetNextAction()
    {
        if (actions == null || index >= actions.Count - 1)
        {
            return null;
        }

        var ret = actions[index];
        index++;

        return ret;
    }
}