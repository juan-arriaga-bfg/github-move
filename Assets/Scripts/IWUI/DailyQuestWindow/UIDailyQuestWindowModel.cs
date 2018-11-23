using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDailyQuestWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.daily.quest.title", "window.daily.quest.title");

    public DailyQuestEntity Quest
    {
        get
        {
            return GameDataService.Current.QuestsManager.DailyQuest;
        }
    }

    public List<TaskEntity> Tasks
    {
        get
        {
            return Quest.Tasks;
        }
    }
}
