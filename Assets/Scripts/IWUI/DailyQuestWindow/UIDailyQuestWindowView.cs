using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Quests;
using UnityEngine.UI;

public class UIDailyQuestWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TaskList")] private UIContainerViewController taskList;
    [IWUIBinding("#MainTimerLabel")] private NSText mainTimerLabel;
    [IWUIBinding("#SecondaryTimerLabel")] private NSText secondaryTimerLabel;
    [IWUIBinding("#ComeBackPanel")] private GameObject comeBackPanel;
    [IWUIBinding("#ComeBackLabel")] private NSText comeBackLabel;
    [IWUIBinding("#MainTimer")] private GameObject mainTimer;
    [IWUIBinding("#SequenceView")] private DailyObjectiveSequenceView sequenceView;

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;

        bool isQuestClaimed = model.Quest.IsClaimed();
        
        SetTitle(model.Title);

        CreateTaskList(model);
        
        model.Timer.OnExecute += OnTimerUpdate;
       
        model.Quest.OnChanged += OnQuestChanged;

        model.Quest.Immortal = !isQuestClaimed;

        // To hide (!) from button at the main window
        if (model.Quest.State == TaskState.New)
        {
            model.Quest.SetInProgressState();
        }
        
        ToggleComebackPanel(isQuestClaimed);

        SetupSequence();
    }

    private void SetupSequence()
    {
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;

        sequenceView.Init();
        
        TaskCompleteDailyTaskEntity clearAllTask = model.Quest.GetTask<TaskCompleteDailyTaskEntity>();
        
        List<CurrencyPair> reward = clearAllTask.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value ?? new List<CurrencyPair>();
        int count = reward.Count;
        
        if (count == 0)
        {
            Debug.LogError("[UIDailyQuestTaskElementViewController] => GetReward: No reward specified for 'Clear all' task!");
            return;
        }

        int globalIndex = GameDataService.Current.QuestsManager.DailyQuestRewardIndex;
        int croppedIndex = globalIndex % count;
        int index = croppedIndex;

        if (globalIndex > 1)
        {
            index -= 2;
        }
        
        List<CurrencyPair> rewardForView = new List<CurrencyPair>();
        for (int i = 0; i < DailyObjectiveSequenceView.ITEMS_COUNT; i++)
        {
            if (index < 0)
            {
                index = 0;
            }

            if (index >= reward.Count)
            {
                index = 0;
            }

            rewardForView.Add(reward[i]);

            index++;
        }

        sequenceView.SetValues(reward, croppedIndex);
    }

    public void ScrollToTop()
    {
        taskList.GetScrollRect().normalizedPosition = new Vector2(0.5f, 1);
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest.IsClaimed())
        {
            ToggleComebackPanel(true); 
        }
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;
        
        model.Timer.OnExecute -= OnTimerUpdate;
        
        model.Quest.OnChanged -= OnQuestChanged;
        
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

            // if (task.IsClaimed())
            // {
            //     continue;
            // }
            
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
            const int CLEAR_ALL_WEIGHT = 5000;
            
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
        
        mainTimerLabel.Text = model.Timer.CompleteTime.GetTimeLeftText(false, null, model.Timer.UseUTC);

        if (comeBackPanel.activeSelf)
        {
            secondaryTimerLabel.Text = model.Timer.CompleteTime.GetTimeLeftText(false, null, model.Timer.UseUTC);
        }
    }

    private void ToggleComebackPanel(bool isQuestCompleted)
    {
        mainTimer.SetActive(!isQuestCompleted);
        
        comeBackPanel.SetActive(isQuestCompleted);
        comeBackLabel.Text = LocalizationService.Get("window.daily.quest.message.all.cleared", "window.daily.quest.message.all.cleared");
        taskList.gameObject.SetActive(!isQuestCompleted);
    }
}