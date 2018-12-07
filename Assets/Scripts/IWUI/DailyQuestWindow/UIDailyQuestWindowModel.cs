using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIDailyQuestWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.daily.quest.title", "window.daily.quest.title");

    public string SequenceHeaderText
    {
        get
        {
            string allClearText = LocalizationService.Get("window.daily.quest.all.cleared.task.name", "window.daily.quest.all.cleared.task.name");
            string formattedAllClearText = $"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SubtitleFinal\">{allClearText}</font>";
            string sequenceHeaderMask = LocalizationService.Get("window.daily.quest.sequence.header.mask", "window.daily.quest.sequence.header.mask");

            string ret = string.Format(sequenceHeaderMask, formattedAllClearText);
            return ret;
        }
    }

    public string TimerHeader => LocalizationService.Get("window.daily.quest.main.timer.title", "window.daily.quest.main.timer.title");

    public DailyQuestEntity Quest => GameDataService.Current.QuestsManager.DailyQuest;

    public List<TaskEntity> Tasks => Quest.ActiveTasks;

    public TimerComponent Timer => GameDataService.Current.QuestsManager.DailyTimer;
    
    public string InformationTitle => LocalizationService.Get("window.daily.quest.information.title", "window.daily.quest.information.title");
    public string InformationMessage => LocalizationService.Get("window.daily.quest.information.message", "window.daily.quest.information.message");
}
