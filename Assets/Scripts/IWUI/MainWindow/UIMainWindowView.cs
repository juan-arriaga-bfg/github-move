using System.Collections.Generic;
using System.Linq;

public class UIMainWindowView : IWUIWindowView
{
    private List<UiQuestButton> quests;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;
        
        UpdateQuest();
    }
    
    public void UpdateQuest()
    {
        var active = GameDataService.Current.QuestsManager.ActiveQuests;
        quests = GetComponentsInChildren<UiQuestButton>().ToList();
        
        for (int i = 0; i < active.Count; i++)
        {
            quests[i].Init(active[i]);
        }
    }
}
