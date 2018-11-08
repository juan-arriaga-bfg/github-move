using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;
    [SerializeField] private CodexButton codexButton;
    
    private List<UiQuestButton> questButtons = new List<UiQuestButton>();

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged += OnActiveQuestsListChanged;
        GameDataService.Current.CodexManager.OnNewItemUnlocked += OnNewPieceBuilded;
        
        OnActiveQuestsListChanged();
        UpdateCodexButton();
    }
    
    private void OnDestroy()
    {
        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged -= OnActiveQuestsListChanged;
        GameDataService.Current.CodexManager.OnNewItemUnlocked -= OnNewPieceBuilded;
    }

    public void OnActiveQuestsListChanged()
    {
        var activeQuests = GameDataService.Current.QuestsManager.ActiveQuests;
        
        CheckQuestButtons(activeQuests);

        InitWindowViewControllers();
    }

    private void CheckQuestButtons(List<QuestEntity> active)
    {
        for (int i = questButtons.Count - 1; i >= 0; i--)
        {
            var item = questButtons[i];
            if (active.Any(e => e.Id == item.Quest.Id))
            {
                continue;
            }
            
            questButtons.RemoveAt(i);
            Destroy(item.gameObject);
        }

        if (questButtons.Count == active.Count)
        {
            return;
        }
        
        pattern.SetActive(true);

        foreach (var quest in active)
        {
            if (questButtons.Any(e => e.Quest.Id == quest.Id))
            {
                continue;
            }
            
            var item = Instantiate(pattern, pattern.transform.parent);
            var button = item.GetComponent<UiQuestButton>();
            button.Init(quest, true);
            
            questButtons.Add(button);

            if (quest.IsCompleted())
            {
                button.gameObject.SetActive(false);
            }
        }
        
        pattern.SetActive(false);

    }
    
    private void UpdateCodexButton()
    {
        codexButton.UpdateState();
    }
    
    public void OnClickCodex()
    {
        BoardService.Current.GetBoardById(0)?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        
        var codexManager = GameDataService.Current.CodexManager;
        var content = codexManager.GetCodexContent();
        
        if (codexManager.CodexState == CodexState.NewPiece)
        {
            codexManager.CodexState = content.PendingRewardAmount > 0 ? CodexState.PendingReward : CodexState.Normal;
        }
        
        codexButton.UpdateState();
        
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.CodexWindow);
        model.ActiveTabIndex = -1;
        model.CodexContent = content;
        model.OnClaim = UpdateCodexButton;
        
        UIService.Get.ShowWindow(UIWindowType.CodexWindow);
    }
    
    public void OnClickShop()
    {
        UIService.Get.ShowWindow(UIWindowType.ChestsShopWindow);
    }
    
    public void OnClickOrders()
    {
        var model = UIService.Get.GetCachedModel<UIOrdersWindowModel>(UIWindowType.OrdersWindow);
        if(model.Orders != null && model.Orders.Count > 0) model.Select = model.Orders[0];
        
        UIService.Get.ShowWindow(UIWindowType.OrdersWindow);
    }
    
    private void OnNewPieceBuilded()
    {
        codexButton.UpdateState();
    }
}
