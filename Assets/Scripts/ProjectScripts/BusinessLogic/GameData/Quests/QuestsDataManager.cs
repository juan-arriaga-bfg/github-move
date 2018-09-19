using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Quests;
using UnityEngine;

public class QuestsDataManager : IECSComponent, IDataManager, IDataLoader<System.Collections.Generic.List<QuestDefOld>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    private System.Collections.Generic.List<QuestBase> quests;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {		
        LoadData(new ResourceConfigDataMapper<System.Collections.Generic.List<QuestDefOld>>("configs/quests.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<System.Collections.Generic.List<QuestDefOld>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                // quests = data;
                quests = new List<QuestBase>();
                var text = File.ReadAllText("D:/quests.json");
                JsonConvert.DeserializeObject<List<QuestBase>>(text);
                
                var questSave = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);

                if (questSave != null)
                {

                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}