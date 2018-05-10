using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestsDataManager : IDataLoader<List<QuestDef>>
{
	private List<QuestDef> quests;
	
	public List<Quest> ActiveQuests = new List<Quest>();

	private List<Quest> stack = new List<Quest>();
	private readonly Dictionary<int, bool> completed = new Dictionary<int, bool>();

	public Action OnUpdateActiveQuests;
	
	public void LoadData(IDataMapper<List<QuestDef>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				quests = data;
				
				var questSave = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);

				if (questSave != null)
				{
					if (questSave.Completed != null)
					{
						foreach (var uid in questSave.Completed)
						{
							completed.Add(uid, true);
						}
					}
					
					UpdateActiveQuest();

					if (questSave.Active != null)
					{
						foreach (var quest in stack)
						{
							if (questSave.Active.Count == 0) break;
							
							var active = questSave.Active.Find(item => item.Uid == quest.Def.Uid);
							
							if(active == null) continue;
							
							quest.CurrentAmount = active.Progress;
							questSave.Active.Remove(active);
						}
					}
				}
				
				UpdateActiveQuest();
			}
			else
			{
				Debug.LogWarning("[QuestsDataManager]: quests config not loaded");
			}
		});
	}
	
	public void UpdateActiveQuest()
	{
		if (stack.Count == 0)
		{
			foreach (var def in quests)
			{
				stack.Add(new Quest(def));
			}
		}
		
		ActiveQuests = new List<Quest>();

		for (var i = stack.Count - 1; i >= 0; i--)
		{
			var quest = stack[i];
			
			if (IsCompleted(quest.Def.Uid))
			{
				stack.RemoveAt(i);
				continue;
			}
			
			if(quest.IsActive() == false) continue;
			
			ActiveQuests.Add(quest);
		}
		
		if (OnUpdateActiveQuests != null) OnUpdateActiveQuests();
	}

	public bool RemoveActiveQuest(Quest quest)
	{
		for (var i = ActiveQuests.Count - 1; i >= 0; i--)
		{
			if(ActiveQuests[i] != quest) continue;
            
			ActiveQuests.RemoveAt(i);
			completed.Add(quest.Def.Uid, true);
			UpdateActiveQuest();
			return true;
		}
        
		return false;
	}

	public bool IsCompleted(int uid)
	{
		return completed.ContainsKey(uid);
	}

	public List<int> SaveCompleted()
	{
		return completed.Keys.ToList();
	}
}