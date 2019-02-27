using Quests;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestButton : IWUIWindowViewController
{
    [IWUIBinding("#(v)")] private GameObject checkMark;
    [IWUIBinding("#(!)")] private GameObject exclamationMark;
    [IWUIBinding("#Shine")] private GameObject shine;
    [IWUIBinding("#ProgressLabel")] private NSText progressLabel;
    [IWUIBinding("#Progress")] private GameObject progressHost;
    
    private bool inited;

    private DailyQuestEntity dailyQuest => GameDataService.Current.QuestsManager.DailyQuest;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);

        UpdateState();
    }

    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);
        inited = true;
    }

    private void OnEnable()
    {
        if (!inited)
        {
            return;
        }
        
        UpdateState();
        
        ToggleSubscription(true);
    }
    
    private void OnDisable()
    {
        if (!inited)
        {
            return;
        }
        
        ToggleSubscription(false);
    }

    private void ToggleSubscription(bool enabled)
    {
        if (dailyQuest == null)
        {
            return;
        }

        var questManager = GameDataService.Current.QuestsManager;
        
        if (enabled)
        {
            questManager.OnQuestStateChanged += QuestChanged;
        }
        else
        {
            questManager.OnQuestStateChanged -= QuestChanged;
        }
    }

    private void QuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest != dailyQuest)
        {
            return;
        }
        
        UpdateState();
    }

    private void UpdateState()
    {
        if (dailyQuest == null)
        {
            return;
        }
        
        progressLabel.Text = GetQuestProgress(dailyQuest);

        int notClaimedCount = dailyQuest.GetCompletedButNotClaimedTasksCount();
       
        checkMark.SetActive(notClaimedCount > 0);
        shine.SetActive(notClaimedCount > 0);
        
        exclamationMark.SetActive(dailyQuest.State == TaskState.New);
    }
    
    public static string GetQuestProgress(QuestEntity quest, int currentValueFontSize = 33, string currentValueColor = "FE4704")
    {
        bool isCompleted = quest.IsCompletedOrClaimed();
        int  current     = quest.GetCompletedTasksCount();
        int  target      = quest.GetActiveTasksCount();
                
        return $"<color=#{( isCompleted ? "FFFFFF" : currentValueColor)}><size={currentValueFontSize}>{current}</size></color>/{target}";
    }
}