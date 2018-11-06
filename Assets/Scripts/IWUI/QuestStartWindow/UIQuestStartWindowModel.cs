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

        Scenario = new ConversationScenarioEntity();
        Scenario.RegisterComponent(charsList);
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
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
            QuestIds = this.Quests.Select(e => e.Id).ToList()
        });
        Scenario.RegisterComponent(actBubble);
        
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char3Id,
                Message = "33333"
            }
        });
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char4Id,
                Message = "444444"
            }
        });
    }
}
