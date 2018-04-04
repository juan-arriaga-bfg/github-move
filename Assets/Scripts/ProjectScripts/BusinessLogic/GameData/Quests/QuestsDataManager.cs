using System.Collections.Generic;
using UnityEngine;

public class QuestsDataManager : IDataLoader<List<QuestDef>>
{
	private List<QuestDef> quests;
	public readonly List<Quest> ActiveQuests = new List<Quest>();

	private int current = -1;
    
	public void LoadData(IDataMapper<List<QuestDef>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				quests = data;
				AddActiveQuest();
			}
			else
			{
				Debug.LogWarning("[ObstaclesDataManager]: obstacles config not loaded");
			}
		});
	}

	public int GetNextIndex()
	{
		current++;

		if (current == quests.Count)
		{
			current = 0;
		}
		
		return current;
	}

	public void AddActiveQuest()
	{
		var index = GetNextIndex();
		
		ActiveQuests.Add(new Quest(quests[index]));
	}

	public bool RemoveActiveQuest(Quest quest)
	{
		for (var i = ActiveQuests.Count - 1; i >= 0; i--)
		{
			if(ActiveQuests[i] != quest) continue;
            
			ActiveQuests.RemoveAt(i);
			return true;
		}
        
		return false;
	}
}