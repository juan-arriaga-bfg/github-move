using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIDailyQuestWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.daily.quest.title", "window.daily.quest.title");
    public string SequenceHeaderText => LocalizationService.Get("window.daily.quest.sequence.header.mask", "window.daily.quest.sequence.header.mask");
    public string AllClearTaskName => LocalizationService.Get("window.daily.quest.all.cleared.task.name", "window.daily.quest.all.cleared.task.name");
    public string TimerHeader => LocalizationService.Get("window.daily.quest.main.timer.title", "window.daily.quest.main.timer.title");
    public string InformationTitle => LocalizationService.Get("window.daily.quest.information.title",     "window.daily.quest.information.title");
    public string InformationMessage => LocalizationService.Get("window.daily.quest.information.message", "window.daily.quest.information.message");
    public string AllClearedText => LocalizationService.Get("window.daily.quest.message.all.cleared",     "window.daily.quest.message.all.cleared");
    
    public DailyQuestEntity Quest => GameDataService.Current.QuestsManager.DailyQuest;

    public List<TaskEntity> Tasks => Quest.ActiveTasks;

    public TimerComponent Timer => GameDataService.Current.QuestsManager.DailyTimer;
    

}
