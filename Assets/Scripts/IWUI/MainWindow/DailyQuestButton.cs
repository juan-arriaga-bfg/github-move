using UnityEngine;
using UnityEngine.UI;

public class DailyQuestButton : IWUIWindowViewController
{
    [IWUIBinding("#(!)")] private Image markIcon;
    [IWUIBinding("#(!)")] private Transform exclamationMark;
    [IWUIBinding("#Shine")] private Transform shine;
    [IWUIBinding("#ProgressLabel")] private NSText progressLabel;
    
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
        
        ToggleSubscription(true);
        UpdateState();
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
        if (enabled)
        {
            dailyQuest.OnChanged += QuestChanged;
        }
        else
        {
            dailyQuest.OnChanged -= QuestChanged;
        }
    }

    private void QuestChanged(QuestEntity quest, TaskEntity task)
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (dailyQuest == null)
        {
            return;
        }
        
        progressLabel.Text = GetQuestProgress(dailyQuest);

        int notClaimedCount = dailyQuest.CompletedButNotClaimedTasksCount();
        exclamationMark.gameObject.SetActive(notClaimedCount > 0);
    }
    
    public static string GetQuestProgress(QuestEntity quest, int currentValueFontSize = 33, string currentValueColor = "FE4704")
    {
        bool isCompleted = quest.IsCompletedOrClaimed();
        int  current     = quest.CompletedTasksCount();
        int  target      = quest.ActiveTasksCount();
                
        return $"<color=#{( isCompleted ? "FFFFFF" : currentValueColor)}><size={currentValueFontSize}>{current}</size></color>/{target}";
    }
}