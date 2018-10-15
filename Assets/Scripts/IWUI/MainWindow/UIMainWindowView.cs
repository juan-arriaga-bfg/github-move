using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;
    [SerializeField] private CodexButton codexButton;
    
    private List<UiQuestButton> quests = new List<UiQuestButton>();

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
        var active = GameDataService.Current.QuestsManager.ActiveQuests;
        
        CheckQuestButtons(active);
        
        for (var i = 0; i < active.Count; i++)
        {
            quests[i].Init(active[i]);
        }
        
        InitWindowViewControllers();
    }

    private void CheckQuestButtons(List<QuestEntity> active)
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

        if (quests.Count <= active.Count) return;
        
        while (quests.Count != active.Count)
        {
            var item = quests[0];
                
            quests.RemoveAt(0);
            Destroy(item.gameObject);
        }
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
