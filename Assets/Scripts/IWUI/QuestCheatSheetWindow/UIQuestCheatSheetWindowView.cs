using System;
using Debug = IW.Logger;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Quests;
using TMPro;
using UnityEngine.UI;

public class UIQuestCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TaskList")] private UIContainerViewController questList;
    [IWUIBinding("#ScrollView")] private ScrollRect scroll;

    [IWUIBinding("#FilterId")] private TMP_InputField inputFilterId;
    [IWUIBinding("#FilterType")] private TMP_InputField inputType;
    
    [IWUIBinding("#TextCounter")] private NSText counterLabel;
    
    [IWUIBinding("#ToggleInactive")] private Toggle toggleInactive;
    [IWUIBinding("#ToggleStarted")] private Toggle toggleStarted;
    [IWUIBinding("#ToggleDone")] private Toggle toggleDone;

    private Regex regexFilterId;
    
    private Regex regexFilterType;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIQuestCheatSheetWindowModel model = Model as UIQuestCheatSheetWindowModel;
        SetTitle(model.Title);

        Fill(CreateList(model), questList);
        
        ApplyFilter();
        
        UIWaitWindowView.Hide(); 
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIQuestCheatSheetWindowModel windowModel = Model as UIQuestCheatSheetWindowModel;
        
    }

    private Regex CreateRegex(string mask)
    {
        if (string.IsNullOrWhiteSpace(mask))
        {
            return null;
        }
        
        StringBuilder sb = new StringBuilder("^");
        foreach (var c in mask)
        {
            if (char.IsDigit(c))
            {
                sb.Append($"[{c}]");
            }
            else if (c == '*' || c == ' ')
            {
                sb.Append(".*");
            }
            else
            {
                sb.Append(c);
            }
        }

        string fixedMask = sb.ToString();
        Regex r = new Regex(fixedMask, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        return r;
    }
    
    private bool CheckTextMatch(Regex regex, string text)
    {
        if (regex == null)
        {
            return true;
            
        }
        
        var ret = regex.IsMatch(text);
        return ret;
    }
    
    private List<IUIContainerElementEntity> CreateList(UIQuestCheatSheetWindowModel model)
    {
        var quests = model.Quests;
        var tasks = model.Tasks;

        // SortTasks(tasks);
        
        if (tasks == null || tasks.Count <= 0)
        {
            Debug.LogError($"[UIQuestCheatSheetWindowView] => CreateList: No tasks defined");
            return null;
        }
        
        if (quests == null || quests.Count <= 0)
        {
            Debug.LogError($"[UIQuestCheatSheetWindowView] => CreateList: No quests defined");
            return null;
        }
        
        if (quests.Count != tasks.Count)
        {
            Debug.LogError($"[UIQuestCheatSheetWindowView] => CreateList: quests.Count != tasks.Count");
            return null;
        }

        var tabViews = new List<IUIContainerElementEntity>(tasks.Count);
        for (int i = 0; i < tasks.Count; i++)
        {
            var quest = quests[i];

            var tabEntity = new UIQuestCheatSheetElementEntity
            {
                MainModel = model,
                QuestId = quest.Id,
                WindowController = Controller,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            tabViews.Add(tabEntity);
        }

        return tabViews;
    }

    public void OnFilterUpdated()
    {
        regexFilterId = CreateRegex(inputFilterId.text);
        regexFilterType = CreateRegex(inputType.text);

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var questManager = GameDataService.Current.QuestsManager;

        int enabledCount = 0;
        int totalCount = 0;
        
        foreach (var item in questList.Tabs)
        {
            totalCount++;
            
            UIQuestCheatSheetElementEntity entity = item.Entity as UIQuestCheatSheetElementEntity;
            string questId = entity.QuestId;
            Type questType = entity.Quest.Tasks[0].GetType();
            
            bool isOk = true;

            do
            {
                if (!CheckTextMatch(regexFilterId, questId))
                {
                    isOk = false;
                    break;
                }

                if (!CheckTextMatch(regexFilterType, questType.ToString().Replace("Task", "").Replace("Entity", "")))
                {
                    isOk = false;
                    break;
                }

                bool isFinished = questManager.FinishedQuests.Contains(questId);
                var activeQuest = questManager.GetActiveQuestById(questId);

                bool isInactive = !isFinished && activeQuest == null;
                bool isStarted = activeQuest != null;
                bool isDone = isFinished;

                if (!toggleInactive.isOn && isInactive)
                {
                    isOk = false;
                    break;
                }
                
                if (!toggleStarted.isOn && isStarted)
                {
                    isOk = false;
                    break;
                }
                
                if (!toggleDone.isOn && isDone)
                {
                    isOk = false;
                    break;
                }
              
            } while (false);

            item.gameObject.SetActive(isOk);

            if (isOk)
            {
                enabledCount++;
            }
        }
        
        counterLabel.Text = $"{enabledCount}/{totalCount}";
    }
}
