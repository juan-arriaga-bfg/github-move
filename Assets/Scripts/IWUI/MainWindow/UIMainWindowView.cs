using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;
    
    private List<UiQuestButton> quests = new List<UiQuestButton>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        GameDataService.Current.QuestsManager.OnUpdateActiveQuests += UpdateQuest;
        
        UpdateQuest();
    }

    public override void OnViewClose()
    {
        GameDataService.Current.QuestsManager.OnUpdateActiveQuests -= UpdateQuest;
        base.OnViewClose();
    }

    public void UpdateQuest()
    {
        var active = GameDataService.Current.QuestsManager.ActiveQuests;
        
        CheckQuestButtons(active);
        
        for (var i = 0; i < active.Count; i++)
        {
            quests[i].Init(active[i]);
        }
        
        InitWindowViewControllers();
    }

    private void CheckQuestButtons(List<Quest> active)
    {
        pattern.SetActive(true);

        if (quests.Count < active.Count)
        {
            for (var i = quests.Count; i < active.Count; i++)
            {
                var item = Instantiate(pattern, pattern.transform.parent);
                quests.Add(item.GetComponent<UiQuestButton>());
            }
        }
        
        pattern.SetActive(false);

        if (quests.Count > active.Count)
        {
            while (quests.Count != active.Count)
            {
                var item = quests[0];
                
                quests.RemoveAt(0);
                Destroy(item.gameObject);
            }
        }
    }

    public void OnClickReset()
    {
        var model= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Reset the progress";
        model.Message = "Do you want to reset the progress and start playing from the beginning?";
        model.AcceptLabel = "<size=30>Reset progress!</size>";
        model.CancelLabel = "No!";
        
        model.OnAccept = () =>
        {
            var profileBuilder = new DefaultProfileBuilder();
            ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
            
            GameDataService.Current.Reload();
        
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
            
            var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
            foreach (var system in ecsSystems)
            {
                ECSService.Current.SystemProcessor.UnRegisterSystem(system);
            }
        };
        
        model.OnCancel = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}
