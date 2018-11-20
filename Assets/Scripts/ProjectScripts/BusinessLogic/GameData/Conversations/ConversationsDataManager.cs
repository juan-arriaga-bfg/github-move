using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityScript.Steps;

public class ConversationsDataManager : IECSComponent, IDataManager
{
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
        LoadData("configs/quests/questStartConversations.data");
    }
    
    private void LoadData(string path)
    {
        return;
        
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
        
        ConversationScenarioEntity scenario = new ConversationScenarioEntity();
        
        JToken participants = json["Participants"];
        scenario.RegisterComponent(BuildCharsList(participants));

        return null;
    }

    private ConversationScenarioCharsListComponent BuildCharsList(JToken json)
    {
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent();
        charsList.Characters = new Dictionary<CharacterPosition, string>
        {
            {CharacterPosition.LeftInner, UiCharacterData.CharSleepingBeauty},
            // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
        };

        return null;
    }

    private ConversationActionBubbleEntity BuildBubble(JToken json)
    {

        return null;
    }
}