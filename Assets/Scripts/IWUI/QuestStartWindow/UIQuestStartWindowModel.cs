using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIQuestStartWindowModel : IWWindowModel
{
    public List<QuestEntity> Quests { get; private set; }
    public ConversationScenarioEntity Scenario;

    public void SetQuests(List<string> questsToStart)
    {
        var questManager = GameDataService.Current.QuestsManager;
        
        Quests = new List<QuestEntity>();
        foreach (var id in questsToStart)
        {
            var quest = questManager.InstantiateQuest(id);
            Quests.Add(quest);
        }
    }

    public void BuildTestConversation()
    {
        string char1Id = UiCharacterData.CharSleepingBeauty;
        string char2Id = UiCharacterData.CharRapunzel;
        string char3Id = UiCharacterData.CharGnomeWorker;
        string char4Id = UiCharacterData.CharPussInBoots;
        
        Scenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.RightOuter, char4Id},
                {CharacterPosition.RightInner, char3Id},
                {CharacterPosition.LeftOuter,  char2Id},
                {CharacterPosition.LeftInner,  char1Id},
            }
        };

        Scenario.RegisterComponent(charsList);
        
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char1Id,
                Message = "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid..."
            }
        });
               
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char3Id,
                Message = "Fog scared Gnomes? It's hard to believe this."
            }
        });
        
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char2Id,
                Message = "We really need your help right now. Let's clear here everything and clean up."
            }
        };
        actBubble.RegisterComponent(new ConversationActionPayloadShowQuestComponent
        {
            QuestIds = this.Quests.Select(e => e.Id).ToList()
        });
        Scenario.RegisterComponent(actBubble);
        
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char4Id,
                Message = "We've sweared that we'll help with any requests to the person who saves us. So thank you one more time."
            }
        });
    }
    
    public void BuildQuestCompletedConversation()
    {
        string char1Id = UiCharacterData.CharSleepingBeauty;
        
        var charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  char1Id},
            }
        };

        Scenario = new ConversationScenarioEntity();
        Scenario.RegisterComponent(charsList);

        var actBubble = new ConversationActionBubbleEntity
        {
            BubbleId = R.UICharacterBubbleQuestCompletedView,
            BubbleDef = new UiCharacterBubbleDefQuestCompleted
            {
                CharacterId = char1Id,
                Message = "I knew you would help me. I am indebted to you.\nOnly you can save the Kingdom!",
                QuestId = "1",
                AllowTeleType = false
            }
        };
        actBubble.RegisterComponent(new ConversationActionPayloadStartNewQuestsIfAnyComponent());
        
        Scenario.RegisterComponent(actBubble);
        
        // var externalAction = new ConversationActionExternalActionEntity();
        // externalAction.RegisterComponent(new ConversationActionPayloadStartNewQuestsIfAnyComponent());
        // Scenario.RegisterComponent(externalAction); 
    }
}
