using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UIQuestStartWindowModel : IWWindowModel
{
    public List<QuestEntity> QuestsToStart { get; private set; }
    public ConversationScenarioEntity QuestCompletedScenario { get;  private set; }
    public ConversationScenarioEntity QuestStartScenario { get;  private set; }
    public QuestEntity CompletedQuest { get;  private set; }
    public string StarterId { get; private set; }

    public void Init(QuestEntity completedQuest, List<string> questsToStart, string starterId)
    {
        CompletedQuest = completedQuest;
        StarterId = starterId;
        
        var questManager = GameDataService.Current.QuestsManager;

        if (completedQuest != null)
        {
            BuildQuestCompletedScenario();
        }
        else
        {
            QuestCompletedScenario = null;
        }
        
        if (questsToStart == null)
        {
            QuestsToStart = null;
            QuestStartScenario = null;
        }
        else
        {
            QuestsToStart = new List<QuestEntity>();
            foreach (var id in questsToStart)
            {
                var quest = questManager.InstantiateQuest(id);
                QuestsToStart.Add(quest);
            }

            BuildQuestStartScenario();
        }
    }

    private void BuildQuestStartScenario()
    {
        QuestStartScenario = GameDataService.Current.ConversationsManager.BuildScenario(StarterId);
        if (QuestStartScenario != null)
        {
            return;
        }
        
        CreateAbstractStartScenario();
    }
    
    private void BuildQuestCompletedScenario()
    {
        QuestCompletedScenario = GameDataService.Current.ConversationsManager.BuildScenario(CompletedQuest.Id);
        if (QuestCompletedScenario != null)
        {
            return;
        }
        
        CreateAbstractCompleteScenario();
    }

    private void GetQuestCompletedData(string questId, out string characterId, out string message, out CharacterEmotion emotion)
    {
        emotion = CharacterEmotion.Happy;
        characterId = UiCharacterData.CharSleepingBeauty;
        
        message = LocalizationService.Get("conversation.error.quest.complete", "conversation.error.quest.complete");
    }
    
    private ConversationActionBubbleEntity CreateStartBubble(string charId, string message, CharacterEmotion emotion = CharacterEmotion.Normal, List<string> questIds = null)
    {
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity()
        {
             BubbleView = R.UICharacterBubbleMessageView,
             Emotion = emotion,
             CharacterId = charId,
             Message = message
        };

        if (questIds != null)
        {
            actBubble.RegisterComponent(new ConversationActionPayloadShowQuestComponent
            {
                QuestIds = questIds
            });
        }

        return actBubble;
    }

    private void CreateAbstractCompleteScenario()
    {
        QuestCompletedScenario = new ConversationScenarioEntity();

        string characterId;
        string message;
        CharacterEmotion emotion;
        GetQuestCompletedData(CompletedQuest.Id, out characterId, out message, out emotion);

        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner, characterId},
            }
        };

        QuestCompletedScenario.RegisterComponent(characterList);

        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleQuestCompletedEntity()
        {
            BubbleView = R.UICharacterBubbleQuestCompletedView,
            Emotion = emotion,
            CharacterId = characterId,
            Message = message,
            AllowTeleType = false,
            QuestId = CompletedQuest.Id
        };
        QuestCompletedScenario.RegisterComponent(actBubble);

        ConversationActionExternalActionEntity extAction = new ConversationActionExternalActionEntity();
        extAction.RegisterComponent(new ConversationActionPayloadProvideRewardComponent());
        QuestCompletedScenario.RegisterComponent(extAction);
    }

    private void CreateAbstractStartScenario()
    {
        QuestStartScenario = new ConversationScenarioEntity();

        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            LocalizationService.Get("conversation.error.message1", "conversation.error.message1"),
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    
}
