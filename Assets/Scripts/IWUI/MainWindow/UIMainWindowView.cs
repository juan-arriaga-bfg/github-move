using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;
    [SerializeField] private CodexButton codexButton;
    [SerializeField] private CanvasGroup questsCanvasGroup;
    [SerializeField] private CanvasGroup rightButtonsCanvasGroups;
    
    [SerializeField] private CanvasGroup workerCanvasGroup;
    [SerializeField] private CanvasGroup energyCanvasGroup;
    [SerializeField] private CanvasGroup codexCanvasGroup;
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private CanvasGroup ordersCanvasGroup;
    
    [SerializeField] private GameObject delimiters;
    
    [SerializeField] private ScrollRect questsScroll;

    private List<UiQuestButton> questButtons = new List<UiQuestButton>();

    private int maxCountOfVisibleQuestButtonsCached = -1;
    
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

    public void ChangeVisibility(UiLockTutorialItem item, bool isLock, bool isAnimate)
    {
        CanvasGroup target = null;
        
        switch (item)
        {
            case UiLockTutorialItem.Worker:
                target = workerCanvasGroup;
                break;
            case UiLockTutorialItem.Energy:
                target = energyCanvasGroup;
                break;
            case UiLockTutorialItem.Codex:
                target = codexCanvasGroup;
                break;
            case UiLockTutorialItem.Shop:
                target = shopCanvasGroup;
                break;
            case UiLockTutorialItem.Orders:
                target = ordersCanvasGroup;
                break;
            default:
                return;
        }
        
        DOTween.Kill(target);
        target.blocksRaycasts = !isLock;

        if (isAnimate == false)
        {
            target.alpha = isLock ? 0 : 1;
            return;
        }

        target.DOFade(isLock ? 0 : 1, 0.2f);
    }

    public void OnActiveQuestsListChanged()
    {
        var activeQuests = GameDataService.Current.QuestsManager.ActiveQuests;
        
        CheckQuestButtons(activeQuests);

        InitWindowViewControllers();
    }

    private void CheckQuestButtons(List<QuestEntity> active)
    {
        bool listChanged = false;
        
        for (int i = questButtons.Count - 1; i >= 0; i--)
        {
            var item = questButtons[i];
            if (active.Any(e => e.Id == item.Quest.Id))
            {
                continue;
            }
            
            questButtons.RemoveAt(i);
            Destroy(item.gameObject);

            listChanged = true;
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
            
            GameObject item = Instantiate(pattern, pattern.transform.parent);
            item.transform.SetSiblingIndex(0);
            var button = item.GetComponent<UiQuestButton>();
            button.Init(quest, true);
            
            questButtons.Add(button);

            if (quest.IsCompleted())
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                listChanged = true;
            }
        }
        
        pattern.SetActive(false);

        delimiters.SetActive(GetMaxCountOfVisibleQuestButtons() < questButtons.Count);
        
        // Scroll list to top
        if (listChanged)
        {
            questsScroll.verticalNormalizedPosition  = 1f;
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

    //todo: remove this hack
    public void FadeAuxButtons(bool visible)
    {
        var time = 0.5f;

        questsCanvasGroup.DOFade(visible ? 1 : 0, time);
        rightButtonsCanvasGroups.DOFade(visible ? 1 : 0, time);

        foreach (var image in delimiters.transform.GetComponentsInChildren<Image>())
        {
            image.DOFade(visible ? 1 : 0, time);
        }
    }

    private int GetMaxCountOfVisibleQuestButtons()
    {
        if (maxCountOfVisibleQuestButtonsCached > 0)
        {
            return maxCountOfVisibleQuestButtonsCached;
        }

        LayoutElement layoutElement = pattern.GetComponent<LayoutElement>();
        VerticalLayoutGroup layoutGroup = pattern.transform.parent.GetComponent<VerticalLayoutGroup>();
        RectTransform container = layoutGroup.transform.parent.GetComponent<RectTransform>();

        float itemH = layoutElement.minHeight;
        float spacing = layoutGroup.spacing;
        float topPadding = layoutGroup.padding.top;
        float bottomPadding = layoutGroup.padding.bottom;
        float containerH = container.rect.height;

        maxCountOfVisibleQuestButtonsCached = (int) ((containerH - topPadding - bottomPadding + spacing) / (itemH + spacing));
        return maxCountOfVisibleQuestButtonsCached;
    }
}
