using Debug = IW.Logger;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuestCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TaskList")] private UIContainerViewController questList;
    [IWUIBinding("#ScrollView")] private ScrollRect scroll;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIQuestCheatSheetWindowModel model = Model as UIQuestCheatSheetWindowModel;
        SetTitle(model.Title);

        Fill(CreateList(model), questList);
        
        UIWaitWindowView.Hide();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIQuestCheatSheetWindowModel windowModel = Model as UIQuestCheatSheetWindowModel;
        
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
}
