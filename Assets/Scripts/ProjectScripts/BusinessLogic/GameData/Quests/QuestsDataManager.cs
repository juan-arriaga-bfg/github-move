using System;
using System.Collections.Generic;
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
}