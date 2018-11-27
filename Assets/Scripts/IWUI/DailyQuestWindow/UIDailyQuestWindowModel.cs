using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDailyQuestWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.daily.quest.title", "window.daily.quest.title");

    public DailyQuestEntity Quest => GameDataService.Current.QuestsManager.DailyQuest;

    public List<TaskEntity> Tasks => Quest.ActiveTasks;

    public TimerComponent Timer => GameDataService.Current.QuestsManager.DailyTimer;
}
