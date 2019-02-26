using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMainWindowView : UIBaseWindowView
{
    [SerializeField] private GameObject pattern;
    [SerializeField] private CodexButton codexButton;
    [SerializeField] private CanvasGroup questsCanvasGroup;
    
    [IWUIBinding("#ButtonsRight")] private CanvasGroup rightButtonsCanvasGroups;
    [IWUIBinding("#ButtonsLeft")] private CanvasGroup leftButtonsCanvasGroups;
    
    [SerializeField] private CanvasGroup codexCanvasGroup;
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private CanvasGroup ordersCanvasGroup;
    [SerializeField] private CanvasGroup removeCanvasGroup;
    [SerializeField] private CanvasGroup dailyCanvasGroup;
    
    [IWUIBinding("#QuestsList")] private ScrollRect questListScroll;
    [IWUIBinding("#QuestListViewport")] private RectTransform questListViewport;
    [IWUIBinding("#QuestListContent")] private RectTransform questListContent;
    [IWUIBinding("#QuestListDelimiters")] private GameObject questListDelimiters;
    
    [IWUIBinding("#DebugButtonsAnchor")] private Transform debugButtonsAnchor;

    [Header("Hint anchors")] 
    [SerializeField] private Transform hintAnchorOrdersButton;
    public Transform HintAnchorOrdersButton => hintAnchorOrdersButton;
    
    [SerializeField] private Transform hintAnchorShopButton;
    public Transform HintAnchorShopButton => hintAnchorShopButton;
    
    [SerializeField] private Transform hintAnchorDailyButton;
    public Transform HintAnchorDailyButton => hintAnchorDailyButton;
    
    private readonly List<UiQuestButton> questButtons = new List<UiQuestButton>();

    private int maxCountOfVisibleQuestButtonsCached = -1;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
#if DEBUG
        
        var dev = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName("DevTools"));
        dev.SetParent(debugButtonsAnchor, false);
        
#endif
        
    }
    
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
                ResourcePanelUtils.TogglePanel(Currency.Worker.Name, !isLock, isAnimate);
                return;
                break;
            case UiLockTutorialItem.Energy:
                ResourcePanelUtils.TogglePanel(Currency.Energy.Name, !isLock, isAnimate);
                return;
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
            case UiLockTutorialItem.Remove:
                target = removeCanvasGroup;
                break;
            case UiLockTutorialItem.Daily:
                target = dailyCanvasGroup;
                break;
            default:
                return;
        }
        
        DOTween.Kill(target);
        target.blocksRaycasts = !isLock;

        if (isAnimate == false)
        {
            target.alpha = isLock ? 0 : 1;
            target.gameObject.SetActive(!isLock);
            return;
        }
        
        target.DOFade(isLock ? 0 : 1, 0.4f)
            .OnStart(() =>
            {
                if(isLock) return;
                target.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                if(isLock == false) return;
                target.gameObject.SetActive(false);
            });
    }

    public void OnActiveQuestsListChanged()
    {
        var activeQuests = GameDataService.Current.QuestsManager.ActiveStoryQuests;
        
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

            if (quest.IsCompletedOrClaimed())
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                listChanged = true;
            }
        }
        
        pattern.SetActive(false);

        // Scroll list to top
        if (listChanged)
        {
            StartCoroutine(UpdateQuestListDelimiters());
            questListScroll.verticalNormalizedPosition  = 1f;
        }
    }

    private IEnumerator UpdateQuestListDelimiters()
    {
        yield return new WaitForEndOfFrame();
        
        float contentH = questListContent.rect.height;
        float viewportH = questListViewport.rect.height;
        
        questListDelimiters.SetActive(contentH > viewportH);
    }

    private void UpdateCodexButton()
    {
        codexButton.UpdateState();
    }
    
    public void OnClickCodex()
    {
        if ((Model as UIMainWindowModel).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        BoardService.Current.FirstBoard?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        
        var codexManager = GameDataService.Current.CodexManager;
        var content = codexManager.GetCodexContent();

        codexButton.UpdateState();
        
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.CodexWindow);
        model.ActiveTabIndex = -1;
        model.CodexContent = content;
        model.OnClose = UpdateCodexButton;
        
        UIService.Get.ShowWindow(UIWindowType.CodexWindow);
    }
    
    public void OnClickShop()
    {
        if ((Model as UIMainWindowModel).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        UIService.Get.ShowWindow(UIWindowType.MarketWindow);
        
        CachedHintArrowComponent.HideArrow(HintAnchorShopButton);
    }
    
    public void OnClickOrders()
    {
        if ((Model as UIMainWindowModel).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        var model = UIService.Get.GetCachedModel<UIOrdersWindowModel>(UIWindowType.OrdersWindow);
        if(model.Orders != null && model.Orders.Count > 0) model.Select = model.Orders[0];
        
        UIService.Get.ShowWindow(UIWindowType.OrdersWindow);
        
        CachedHintArrowComponent.HideArrow(HintAnchorOrdersButton);
    }
    
    public void OnClickDailyQuest()
    {
        if ((Model as UIMainWindowModel).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        UIService.Get.ShowWindow(UIWindowType.DailyQuestWindow);
        
        CachedHintArrowComponent.HideArrow(HintAnchorDailyButton);
    }
    
    public void OnClickOptions()
    {
        if ((Model as UIMainWindowModel).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        if (UIService.Get.GetShowedWindowsCount(UIWindowType.IgnoredWindows) > 0)
        {
            return;
        }
        
        UIService.Get.ShowWindow(UIWindowType.SettingsWindow);
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
        leftButtonsCanvasGroups.DOFade(visible ? 1 : 0, time);

        foreach (var image in questListDelimiters.transform.GetComponentsInChildren<Image>())
        {
            image.DOFade(visible ? 1 : 0, time);
        }
    }
}
