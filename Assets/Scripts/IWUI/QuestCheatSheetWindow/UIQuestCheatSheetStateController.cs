using System;
using System.Collections.Generic;
using DG.Tweening;
using Quests;
using UnityEngine;

public class UIQuestCheatSheetStateController : IWUIWindowViewController
{
    [IWUIBinding("#StateTextInactive")] private NSText lblInactive;
    [IWUIBinding("#StateTextStarted")] private NSText lblStarted;
    [IWUIBinding("#StateTextDone")] private NSText lblDone;
    [IWUIBinding("#MarkInactive")] private GameObject markInactive;
    [IWUIBinding("#MarkStarted")] private GameObject markStarted;
    [IWUIBinding("#MarkDone")] private GameObject markDone;

    private string questId;
    private QuestsDataManager questManager;

    public Action OnApplyChanges;
    
    public void Init(string id)
    {
        CacheResources();
        
        questManager = GameDataService.Current.QuestsManager;
        questId = id;

        UpdateUI();
    }

    private bool IsQuestCompleted()
    {
        return questManager.FinishedQuests.Contains(questId);
    }

    private bool IsQuestInProgress()
    {
        return questManager.GetActiveQuestById(questId) != null;
    }

    private bool IsQuestNotStarted()
    {
        return !IsQuestCompleted() && !IsQuestInProgress();
    }
    
    public void UpdateUI()
    {
        markInactive.SetActive(IsQuestNotStarted());
        markStarted.SetActive(IsQuestInProgress());
        markDone.SetActive(IsQuestCompleted());

        const float FADE = 0.3f;
        ColorUtility.TryParseHtmlString("#D9D9D9", out Color colorInactive);
        ColorUtility.TryParseHtmlString("#FFFA1F", out Color colorStarted);
        ColorUtility.TryParseHtmlString("#4CE717", out Color colorDone);
        var colorWhite = new Color(1, 1, 1, FADE);

        colorInactive = IsQuestNotStarted() ? colorInactive : colorWhite; 
        colorStarted = IsQuestInProgress() ? colorStarted : colorWhite;
        colorDone = IsQuestCompleted() ? colorDone : colorWhite;

        lblInactive.TextLabel.color = colorInactive;
        lblStarted.TextLabel.color = colorStarted;
        lblDone.TextLabel.color = colorDone;
    }

    public void OnStateInactiveClick()
    {
        SetInactive();
    }

    public void SetInactive()
    {
        if (IsQuestNotStarted())
        {
            return;
        }

        if (IsQuestInProgress())
        {
            var quest = questManager.GetActiveQuestById(questId);
            quest.ForceComplete();
            questManager.FinishQuest(questId);
        }

        if (IsQuestCompleted())
        {
            questManager.FinishedQuests.Remove(questId);
        }

        ApplyChanges();
    }

    public void OnStateStartedClick()
    {
        SetStarted();
    }

    public void SetStarted()
    {
        if (IsQuestInProgress())
        {
            return;
        }

        if (IsQuestCompleted())
        {
            questManager.FinishedQuests.Remove(questId);
        }

        DevTools.FastStartQuest(new List<string> {questId});

        ApplyChanges();
    }

    public void OnStateDoneClick()
    {
        SetDone();
    }

    public void SetDone()
    {
        if (IsQuestCompleted())
        {
            return;
        }

        if (IsQuestNotStarted())
        {
            DevTools.FastStartQuest(new List<string> {questId});
        }

        var quest = questManager.GetActiveQuestById(questId);
        quest.ForceComplete();
        questManager.FinishQuest(questId);

        ApplyChanges();
    }

    private void ApplyChanges()
    {
        var questsToStart = questManager.CheckConditions(out _);
        if (questsToStart.Count > 0)
        {
            DevTools.FastStartQuest(questsToStart);
        }

        ProfileService.Current.QueueComponent.RemoveAction("StartNewQuestsIfAny");
        ProfileService.Current.QueueComponent.RemoveAction("ShowQuestCompletedWindow_" + questId);
        
        var model = UIService.Get.GetCachedModel<UIQuestCheatSheetWindowModel>(UIWindowType.QuestCheatSheetWindow);
        model.Refresh();
        
        OnApplyChanges?.Invoke();
    }
}