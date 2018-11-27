using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIDailyQuestWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TaskList")] private UIContainerViewController taskList;
    [IWUIBinding("#TimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ComeBackPanel")] private GameObject comeBackPanel;
    [IWUIBinding("#ComeBackLabel")] private NSText comeBackLabel;
    [IWUIBinding("#MainTimer")] private GameObject mainTimer;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;

        bool isQuestCompleted = model.Quest.IsCompletedOrClaimed();
        
        SetTitle(model.Title);

        CreateTaskList(model);
        
        model.Timer.OnExecute += OnTimerUpdate;

        model.Quest.Immortal = !isQuestCompleted;

        ToggleComebackPanel(isQuestCompleted);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;
        model.Timer.OnExecute -= OnTimerUpdate;
        
        model.Quest.Immortal = false;

        if (!model.Timer.IsExecuteable())
        {
            GameDataService.Current.QuestsManager.StartNewDailyQuest();
        }
    }
    
    private void CreateTaskList(UIDailyQuestWindowModel model)
    {
        var tasks = model.Tasks;

        SortTasks(tasks);
        
        if (tasks == null || tasks.Count <= 0)
        {
            taskList.Clear();
            return;
        }

        var tabViews = new List<IUIContainerElementEntity>(tasks.Count);
        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];

            if (task.IsClaimed())
            {
                continue;
            }
            
            var tabEntity = new UIDailyQuestTaskElementEntity
            {
                Task = task,
                WindowController = Controller,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            tabViews.Add(tabEntity);
        }
        
        taskList.Create(tabViews);
        taskList.Select(0);
    }

    private void SortTasks(List<TaskEntity> tasks)
    {
        tasks.Sort((item1, item2) =>
        {
            const int CLAIMED_WEIGHT = 10000;
            const int COMPLETED_WEIGHT = -1000;
            const int CLEAR_ALL_WEIGHT = 20000;
            
            int w1 = (int)item1.Group;
            int w2 = (int)item2.Group;

            if (item1.IsClaimed())
            {
                w1 += CLAIMED_WEIGHT;
            }
            else if (item1.IsCompleted())
            {
                w1 += COMPLETED_WEIGHT;
            }

            if (item2.IsClaimed())
            {
                w2 += CLAIMED_WEIGHT;
            }
            else if (item2.IsCompleted())
            {
                w2 += COMPLETED_WEIGHT;
            }

            if (item1 is TaskCompleteDailyTaskEntity)
            {
                w1 += CLEAR_ALL_WEIGHT;
            }
            
            if (item2 is TaskCompleteDailyTaskEntity)
            {
                w2 += CLEAR_ALL_WEIGHT;
            }
            
            return w1 - w2;
        });
    }
    
    private void OnTimerUpdate()
    {
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;
        
        timerLabel.Text = model.Timer.CompleteTime.GetTimeLeftText(false, null, model.Timer.UseUTC);
    }

    private void ToggleComebackPanel(bool isQuestCompleted)
    {
        mainTimer.SetActive(!isQuestCompleted);
        
        comeBackPanel.SetActive(isQuestCompleted);
        comeBackLabel.Text = LocalizationService.Get("window.daily.quest.message.all.cleared", "window.daily.quest.message.all.cleared");
    }
}