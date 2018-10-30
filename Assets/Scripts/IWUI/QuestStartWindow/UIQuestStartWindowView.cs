using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIQuestStartWindowView : IWUIWindowView
{
    [SerializeField] private GameObject questCardPrefab;
    
    private List<UIQuestCard> questCards = new List<UIQuestCard>();
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        WarmUpPool();
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        CreateQuestCards(windowModel.Quests);
    }

    private void CreateQuestCards(List<QuestEntity> quests)
    {
        var pool = UIService.Get.PoolContainer;
        
        // Cleanup
        foreach (var card in questCards)
        {
            pool.Return(card.gameObject);
        }
        questCards.Clear();

        // Create cards
        foreach (var quest in quests)
        {
            UIQuestCard card = pool.Create<UIQuestCard>(questCardPrefab);
            card.gameObject.SetActive(true);
            card.transform.SetParent(questCardPrefab.transform.parent, false);
            card.Init(quest);
            
            questCards.Add(card);
        }
    }

    public override void OnViewClose()
    {
        StartQuests();
        
        base.OnViewClose();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        
    }

    private void StartQuests()
    {
        List<string> ids = (Model as UIQuestStartWindowModel).Quests.Select(e => e.Id).ToList();
        GameDataService.Current.QuestsManager.StartQuests(ids);
    }

    private void WarmUpPool()
    {
        var pool = UIService.Get.PoolContainer;

        for (int i = 1; i <= 3; i++)
        {
            UIQuestCard tabInstance = pool.Create<UIQuestCard>(questCardPrefab);
            pool.Return(tabInstance.gameObject);
        }
        
        questCardPrefab.SetActive(false);
    }
}