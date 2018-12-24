using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quests;
using UnityEngine.UI;

public class UIDailyQuestWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TaskList")] private UIContainerViewController taskList;
    [IWUIBinding("#ScrollView")] private ScrollRect scroll;
    [IWUIBinding("#MainTimerLabel")] private NSText mainTimerLabel;
    [IWUIBinding("#SequenceHeader")] private NSText sequenceHeaderLabel;
    [IWUIBinding("#ComeBackPanel")] private GameObject comeBackPanel;
    [IWUIBinding("#ComeBackLabel")] private NSText comeBackLabel;
    [IWUIBinding("#MainTimer")] private GameObject mainTimer;
    [IWUIBinding("#MainTimerPlaceholder")] private GameObject mainTimerPlaceholder;
    [IWUIBinding("#SequenceView")] private DailyQuestWindowSequenceView sequenceView;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;

        bool isQuestClaimed = model.Quest.IsClaimed();
        
        SetTitle(model.Title);

        SetSequenceHeader(model);

        Fill(CreateTaskList(model), taskList);
        
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

        ScrollToTop();
    }

    private void SetSequenceHeader(UIDailyQuestWindowModel model)
    {
        string allClearText = model.AllClearTaskName;
        string sequenceHeaderMask = model.SequenceHeaderText;
        string styledAllClearText = $"<color=#FFFFFF><font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SubtitleFinal\">{allClearText}</font></color>";

        sequenceHeaderLabel.Text = string.Format(sequenceHeaderMask, styledAllClearText);
    }

    private void SetupSequence()
    {
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;

        sequenceView.OnViewInit(this);
        
        TaskCompleteDailyTaskEntity clearAllTask = model.Quest.GetTask<TaskCompleteDailyTaskEntity>();
        
        List<CurrencyPair> reward = clearAllTask.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value ?? new List<CurrencyPair>();
        int count = reward.Count;
        
        if (count == 0)
        {
            Debug.LogError("[UIDailyQuestTaskElementViewController] => GetReward: No reward specified for 'Clear all' task!");
            return;
        }

        int globalIndex = GameDataService.Current.QuestsManager.DailyQuestRewardIndex;
        int croppedIndex = globalIndex > count ? globalIndex % count : globalIndex;
        int index = croppedIndex;

        if (globalIndex > 0)
        {
            index -= 1;
        }

        int activeIndex = globalIndex > 0 ? 1 : 0;
        
        List<CurrencyPair> rewardForView = new List<CurrencyPair>();
        for (int i = 0; i < DailyQuestWindowSequenceView.ITEMS_COUNT; i++)
        {
            if (index < 0)
            {
                index = rewardForView.Count - index - 1;
            }

            if (index >= reward.Count)
            {
                index = 0;
            }

            rewardForView.Add(reward[index]);

            index++;
        }

        sequenceView.SetValues(rewardForView, activeIndex);
    }

    public void ScrollToTop()
    {
        taskList.GetScrollRect().normalizedPosition = new Vector2(0.5f, 1);
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest.IsClaimed())
        {
            //ToggleComebackPanel(true); 
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
    
    private List<IUIContainerElementEntity> CreateTaskList(UIDailyQuestWindowModel model)
    {
        var tasks = model.Tasks;

        SortTasks(tasks);
        
        if (tasks == null || tasks.Count <= 0)
        {
            return null;
        }

        var tabViews = new List<IUIContainerElementEntity>(tasks.Count);
        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];

            var tabEntity = new UIDailyQuestTaskElementEntity
            {
                Quest = model.Quest,
                Task = task,
                WindowController = Controller,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            tabViews.Add(tabEntity);
        }

        return tabViews;
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
        
        string time = model.Timer.CompleteTime.GetTimeLeftText(model.Timer.UseUTC, true, null, 2.5f);
        
        if (comeBackPanel.activeSelf)
        {
            string description = model.AllClearedText;
            comeBackLabel.Text = $"{description} {time}";
        }
        else
        {
            string description = model.TimerHeader;
            mainTimerLabel.Text = $"<color=#FFFFFF><font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SubtitleFinal\">{description} </font></color>{time}";
        }
    }

    private void ToggleComebackPanel(bool isQuestCompleted)
    {
        mainTimer.SetActive(!isQuestCompleted);
        mainTimerPlaceholder.SetActive(isQuestCompleted);
        
        comeBackPanel.SetActive(isQuestCompleted);
        taskList.gameObject.SetActive(!isQuestCompleted);
    }

    public void OnInformationButtonClick()
    {
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;
        
        UIMessageWindowController.CreateMessage(model.InformationTitle, model.InformationMessage);
    }
    
    public void RunScrollTween(Transform target, Action onComplete)
    {
        scroll.enabled = false;
        
        DOTween.Sequence()
               .SetId(this)
               .AppendCallback(() =>
                {
                    StartCoroutine(WaitForLayoutAndScroll(target, onComplete));
                })
            ;
    }

    private IEnumerator WaitForLayoutAndScroll(Transform target, Action onComplete)
    {       
        yield return new WaitForEndOfFrame();
        
        // Respect space between top size of the viewport and item
        const float PADDING = 0f;
        const float SCROLL_TIME = 1f;
        
        RectTransform rect = target.GetComponent<RectTransform>();
        float y   = rect.localPosition.y;
        float h   = rect.sizeDelta.y;
        float top = y + h / 2 + PADDING;

        float scrollToY = -top;

        // Do not scroll if already visible
        if (scroll.content.anchoredPosition.y > scrollToY)
        {
            DOTween.Kill(scroll.content);
        
            scroll.content.DOAnchorPosY(scrollToY, SCROLL_TIME)
                  .SetEase(Ease.InOutBack)
                  .SetId(scroll.content)
                  .OnComplete(() =>
                   {
                       scroll.enabled = true;
                       onComplete?.Invoke();
                   });
        }
        else 
        {
            scroll.enabled = true;
            onComplete?.Invoke();
        }
    }

    public void ScrollToFirstNotCompletedOrNotClaimedTask()
    {
        // Allow to highlight completed but not claimed state on the second iteration
        for (int i = 0; i <= 1; i++)
        {
            foreach (var item in taskList.Tabs)
            {
                var view = (UIDailyQuestTaskElementViewController) item;
                var task = view.Task;

                if (task.IsClaimed())
                {
                    continue;
                }
                
                if (i == 0 && task.IsCompleted())
                {
                    continue;
                }

                RunScrollTween(view.transform, () =>
                {
                    view.HighlightForHint();
                });

                return;
            }
        }
    }
}