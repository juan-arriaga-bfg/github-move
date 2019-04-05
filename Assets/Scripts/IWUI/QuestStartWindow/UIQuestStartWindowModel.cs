using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UIQuestStartWindowModel : IWWindowModel
{
    public HashSet<QuestEntity> QuestsToStart { get; private set; }
    public ConversationScenarioEntity QuestCompletedScenario { get;  private set; }
    public ConversationScenarioEntity QuestStartScenario { get;  private set; }
    public QuestEntity CompletedQuest { get;  private set; }
    public string StarterId { get; private set; }

    public bool TestMode;

    private List<string> commonStartMessages;
    private List<string> commonCompleteMessages;
    
    public void Init(QuestEntity completedQuest, HashSet<string> questsToStart, string starterId)
    {
        FillCommonMessagesCache();
        
        CompletedQuest = completedQuest;
        StarterId = starterId;
        TestMode = false;
        
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
            QuestsToStart = new HashSet<QuestEntity>();
            foreach (var id in questsToStart)
            {
                var quest = questManager.InstantiateQuest(id);
                QuestsToStart.Add(quest);
            }

            BuildQuestStartScenario();
        }
    }

    private void FillCommonMessagesCache()
    {
        if (commonStartMessages != null && commonCompleteMessages != null)
        {
            return;
        }
        
        commonStartMessages = new List<string>();
        commonCompleteMessages = new List<string>();

        int counter = 0;
        while (true)
        {
            counter++;
            string key = $"conversation.common.message{counter}";
            if (LocalizationService.Current.IsExists(key))
            {
                commonStartMessages.Add(key);
            }
            else
            {
                break;
            }
        }
        
        counter = 0;
        while (true)
        {
            counter++;
            string key = $"conversation.common.quest.complete{counter}";
            if (LocalizationService.Current.IsExists(key))
            {
                commonCompleteMessages.Add(key);
            }
            else
            {
                break;
            }
        }
    }
    
    private string GetRandomCommonQuestStartText()
    {
        var key = commonStartMessages[UnityEngine.Random.Range(0, commonStartMessages.Count)];
        var ret = LocalizationService.Get(key, key);
        return ret;
    }
    
    private string GetRandomCommonQuestCompleteText()
    {
        var key = commonCompleteMessages[UnityEngine.Random.Range(0, commonCompleteMessages.Count)];
        var ret = LocalizationService.Get(key, key);
        return ret;
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

        message = GetRandomCommonQuestCompleteText();
    }
    
    private ConversationActionBubbleEntity CreateStartBubble(string charId, string message, CharacterEmotion emotion = CharacterEmotion.Normal, HashSet<string> questIds = null)
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
        extAction.RegisterComponent(new ConversationActionPayloadProvideRewardComponent {QuestId = CompletedQuest.Id});
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
            GetRandomCommonQuestStartText(),
            CharacterEmotion.Normal,
            new HashSet<string>(QuestsToStart.Select(e => e.Id))
        ));
    }
    
    
}
