using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityScript.Steps;

public class ConversationsDataManager : IECSComponent, IDataManager
{
    private enum ConversationPurpose
    {
        Unknown,
        QuestStart,
        QuestFinish,
        Other
    }
    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private Dictionary<string, JToken> cache;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void Reload()
    {
        LoadData("configs/conversations.data");
    }
    
    private void LoadData(string path)
    {
        var dataMapper = new ResourceConfigDataMapper<object>(path, NSConfigsSettings.Instance.IsUseEncryption);
        var json = dataMapper.GetDataAsJson();
        
        cache = Parse(json);
    }
    
    private Dictionary<string, JToken> Parse(string json)
    {
        Dictionary<string, JToken> ret = new Dictionary<string, JToken>();

        JToken root = JToken.Parse(json);
        
        foreach (var node in root)
        {
            string id = node["Id"].Value<string>();  
            
#if DEBUG
            if (ret.ContainsKey(id))
            {
                Debug.LogError($"[ConversationsDataManager] => Parse: Duplicate ID '{id}' found in\n'{json}'!");
            }
#endif
            
            ret.Add(id, node);
        }
    
        return ret;
    }

    public ConversationScenarioEntity BuildScenario(string id)
    {
        JToken json;
        if (!cache.TryGetValue(id, out json))
        {
            Debug.LogError($"[ConversationsDataManager] => BuildScenario: config for id '{id}' not found");
            return null;
        }

        ConversationPurpose purpose = DetectPurpose(id);
        
        ConversationScenarioEntity scenario = new ConversationScenarioEntity();

        List<ConversationActionBubbleEntity> bubbles = new List<ConversationActionBubbleEntity>();
        
        var bubblesJson = json["Bubbles"];
        foreach (JToken bubbleJson in bubblesJson)
        {
            ConversationActionBubbleEntity bubble = BuildBubble(bubbleJson, purpose, id);
            scenario.RegisterComponent(bubble);

            bubbles.Add(bubble);
        }
        
        if (bubbles.Count == 0)
        {
            Debug.LogError($"[ConversationsDataManager] => BuildScenario: No bubbles specified!");
            return null;
        }

        // Chars list
        var charsList = BuildCharsList(json, bubbles, purpose);
        scenario.RegisterComponent(charsList);

        // Side and quest ids
        for (var i = 0; i < bubbles.Count; i++)
        {
            var bubble = bubbles[i];
            bubble.Side = charsList.GetCharacterSide(bubble.CharacterId);

            if (purpose == ConversationPurpose.QuestStart && i == bubbles.Count - 1)
            {
                var starter = GameDataService.Current.QuestsManager.GetQuestStarterById(id);
                if (starter == null)
                {
                    Debug.LogError($"[ConversationsDataManager] => BuildScenario: starter '{id}' not found!");
                    return null;
                }
                
                bubble.RegisterComponent(new ConversationActionPayloadShowQuestComponent
                {
                    QuestIds = starter.QuestToStartIds
                });
            }
        }

        if (purpose == ConversationPurpose.QuestFinish)
        {
            ConversationActionExternalActionEntity extAction = new ConversationActionExternalActionEntity();
            extAction.RegisterComponent(new ConversationActionPayloadProvideRewardComponent());
            scenario.RegisterComponent(extAction);
        }
        
        return scenario;
    }

    private ConversationPurpose DetectPurpose(string id)
    {
        var starter = GameDataService.Current.QuestsManager.GetQuestStarterById(id);
        if (starter != null)
        {
            return ConversationPurpose.QuestStart;
        }

        if (GameDataService.Current.QuestsManager.IsQuestDefined(id))
        {
            return ConversationPurpose.QuestFinish;
        }

        return ConversationPurpose.Unknown;
    }

    private ConversationScenarioCharacterListComponent BuildCharsList(JToken json, List<ConversationActionBubbleEntity> bubbles, ConversationPurpose purpose)
    {
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent();
        if (purpose == ConversationPurpose.QuestFinish)
        {
            characterList.ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner, bubbles[0].CharacterId},
            };
        }
        else
        {
            json.PopulateObject(characterList);
        }
        
        // Validate
#if DEBUG
        foreach (var bubble in bubbles)
        {
            var charId = bubble.CharacterId;
            if (!characterList.ConversationCharacters.ContainsValue(charId))
            {
                Debug.LogError($"[ConversationsDataManager] => BuildCharsList: Char with id '{charId}' not defined but found in bubble '{bubble.Message}'!");
            }
        }
#endif
        
        return characterList;
    }

    private ConversationActionBubbleEntity BuildBubble(JToken json, ConversationPurpose purpose, string scenarioId)
    {
        ConversationActionBubbleEntity bubble;
        
        switch (purpose)
        {
            case ConversationPurpose.QuestFinish:
                bubble = new ConversationActionBubbleQuestCompletedEntity
                {
                    BubbleView = R.UICharacterBubbleQuestCompletedView,
                    AllowTeleType = false,
                    QuestId = scenarioId
                };
                break;

            default:
                bubble = new ConversationActionBubbleEntity
                {
                    BubbleView = R.UICharacterBubbleMessageView
                };
                break;
        }

        json.PopulateObject(bubble);
        
        return bubble;
    }

    public List<string> GetAvailableScenarioIds()
    {
        return cache.Keys.ToList();
    }
}