using Debug = IW.Logger;
using DG.Tweening;
using Quests;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestButton : UIGenericResourcePanelViewController
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private UiQuestButtonArrow arrow;
    [SerializeField] private Button button;
    [SerializeField] private Transform iconAnchor;
    [SerializeField] private Transform iconAnchorHero;
    
    public QuestEntity Quest { get; private set; }
    private bool isUp;
    private bool interactive;
    
    private Transform content;
    
    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;
            SetLabelValue(currentValueAnimated);
        }
    }

    private TaskEntity GetFirstTask()
    {
        return Quest.Tasks[0];
    }
    
    public void Init(QuestEntity quest, bool interactive)
    {
        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.alpha = 1;
        }

        this.interactive = interactive;

        if (checkmark != null)
        {
            checkmark.SetActive(false);
        }
        
        if (Quest != null)
        {
            ResourcesViewManager.Instance.UnRegisterView(this);
            Quest.OnChanged -= OnQuestChanged;
        }
        
        Quest = quest;

        isUp = false;
        var taskAboutPiece = GetFirstTask() as IHavePieceId;
        if (taskAboutPiece != null)
        {
            int pieceId = taskAboutPiece.PieceId;
            PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);
            itemUid = pieceTypeDef.Id.ToString();
        }

        if (interactive)
        {
            quest.OnChanged += OnQuestChanged;
        }
        
        ResourcesViewManager.Instance.RegisterView(this);
        UpdateView();

        if (arrow != null)
        {
            arrow.SetQuest(interactive ? quest : null);
        }

        ToggleButton();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
    }

    private void ToggleButton()
    {
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest.Id != Quest.Id)
        {
            return;
        }
        
        UpdateView();
        
        if (Quest.IsCompletedOrClaimed())
        {
            interactive = false;

            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.DOFade(0, 1)
                               .SetId(this);
            }
        }
    }

    protected override void OnEnable()
    {
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
        
        if (Quest != null)
        {
            Quest.OnChanged -= OnQuestChanged;
        }
        ResourcesViewManager.Instance.UnRegisterView(this);
    }

    public override void UpdateView()
    {
        if(Quest == null) return;
        
        int value = (GetFirstTask() as TaskCounterEntity).TargetValue;

        currentValueAnimated = value;
        currentValue = value;

        SetLabelValue(currentValue);
    }
    
    public static Transform GetIcon(TaskEntity task, Transform anchor, Transform anchorHero, Image icon = null)
    {
        var id = task.GetIco();

        if (string.IsNullOrEmpty(id))
        {
            if (task is TaskCurrencyEntity taskAboutCurrency && !string.IsNullOrEmpty(taskAboutCurrency.CurrencyName))
            {
                var pair = new CurrencyPair {Currency = taskAboutCurrency.CurrencyName};
                id = pair.GetIcon();
            }
        }

        if (string.IsNullOrEmpty(id))
        {
            if (task is IHavePieceId taskAboutPiece && taskAboutPiece.PieceId != PieceType.None.Id && taskAboutPiece.PieceId != PieceType.Empty.Id)
            {
                id = PieceType.Parse(taskAboutPiece.PieceId);
            }
        }

        if (string.IsNullOrEmpty(id))
        {
            id = "codexQuestion";
            IW.Logger.LogWarning($"[QuestButton] => icon is Null or Empty");
        }

        if (ContentService.Current.IsObjectRegistered(id) == false)
        {
            if (icon != null) icon.sprite = IconService.Current.GetSpriteById(id);
            return null;
        }
        
        var obj = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        obj.SetParentAndReset(id.Contains("NPC_") && anchorHero != null ? anchorHero : anchor);
        
        return obj;
    }

    public static string GetTaskProgress(TaskEntity task, int currentValueFontSize = 55, string currentValueColor = "FE4704")
    {
        TaskCounterEntity counterTask = task as TaskCounterEntity;
        if (counterTask == null)
        {
            return string.Empty;
        }
            
        bool isCompleted = task.IsCompletedOrClaimed();
        int  target      = counterTask.TargetValue;
        int  current     = Mathf.Min(counterTask.CurrentValue, target);
        
        if (isCompleted && current != target)
        {
            current = target;
            Debug.LogWarning($"[UiQuestButton] => GetTaskProgress: task '{task.Id} current value is hacked {counterTask.CurrentValue} => {target} due to 'completed' state of quest.\nIf it happens not after migration, consider to take a look.");
        }
                
        return $"<color=#{( isCompleted ? "FFFFFF" : currentValueColor)}><size={currentValueFontSize}>{current}</size></color>/{target}";
    }
    
    private void SetLabelValue(int value)
    {
        if(Quest == null) return;

        // Debug.Log($"AAAAA UPDATE: {quest.Id}");
        
        if (content != null)
        {
            UIService.Get.PoolContainer.Return(content.gameObject);
            content = null;
        }
        
        content = GetIcon(Quest.Tasks[0], iconAnchor, iconAnchorHero, icon);
        icon.gameObject.SetActive(content == null);

        var isComplete = Quest.IsCompletedOrClaimed();

        if (isComplete && isUp == false && interactive)
        {
            isUp = true;
            transform.SetSiblingIndex(0);

            if (Quest.State == TaskState.Completed)
            {
                AddQuestWindowToQueue();
            }
        }

        if (amountLabel != null)
        {

            int targetValue = (GetFirstTask() as TaskCounterEntity).TargetValue;
            if (interactive)
            {
                int curValue = (GetFirstTask() as TaskCounterEntity).CurrentValue;
                amountLabel.Text = $"<color=#{(isComplete ? "FFFFFF" : "FE4704")}><size=33>{Mathf.Min(value, curValue)}</size></color>/{targetValue}";
            }
            else
            {
                amountLabel.Text = $"x{targetValue}";
            }
        }

        if (shine != null)
        {
            shine.SetActive(isComplete || !interactive);
        }
    }

    private void AnimateButton()
    {
        var viewAnchor = button.transform;
        DOTween.Kill(viewAnchor);
        if (viewAnchor != null)
        {
            var sequence = DOTween.Sequence().SetId(viewAnchor);
            sequence.Append(viewAnchor.DOScale(new Vector3(1.05f, 1.05f, 1f), 0.35f));
            sequence.Append(viewAnchor.DOScale(new Vector3(1f, 1f, 1f), 0.35f));
        }
    }
    
    public void OnClick()
    {
        if (!interactive)
        {
            return;
        }

        if (Quest == null || Quest.IsCompletedOrClaimed())
        {
            return;
        }
        
        AnimateButton();
        
        if (UIService.Get.GetCachedModel<UIMainWindowModel>(UIWindowType.MainWindow).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        if (IsClickDisabledByTutorial())
        {
            UIMessageWindowController.CreateMessage(
                LocalizationService.Get("common.title.forbidden", "common.title.forbidden"),
                LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            
            return;
        }
        
        ShowQuestWindow();
    }

    private void AddQuestWindowToQueue()
    {
        string id = "ShowQuestCompletedWindow_" + Quest.Id;
        DefaultSafeQueueBuilder.BuildAndRun(id, true, ShowQuestCompletedWindow);
    }

    private void ShowQuestCompletedWindow()
    {
        if (!DevTools.IsQuestDialogsEnabled())
        {
            Debug.Log($"Quest completed in FAST mode: '{Quest.Id}'. Consider to enable dialogs in the Dev Tools to see a conversation");
            DevTools.FastCompleteQuest(Quest);
            return;
        }

        if (!ShowCharUnlockedWindow(Quest))
        {
            var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);
            model.Init(Quest, null, null);
        
            UIService.Get.ShowWindow(UIWindowType.QuestStartWindow);
        }
    }

    public static bool ShowCharUnlockedWindow(QuestEntity quest)
    {
        var task = quest.Tasks[0];
        var createPieceTask = task as IHavePieceId;
        if (createPieceTask == null)
        {
            return false;
        }

        var pieceId = createPieceTask.PieceId;
        if (!PieceType.GetDefById(pieceId).Filter.Has(PieceTypeFilter.Character))
        {
            return false;
        }
        
        var model = UIService.Get.GetCachedModel<UICharacterUnlockedWindowModel>(UIWindowType.CharacterUnlockedWindow);
        model.Quest = quest;
        model.TestMode = false;
        UIService.Get.ShowWindow(UIWindowType.CharacterUnlockedWindow);

        return true;
    }
    
    private void ShowQuestWindow()
    {
        var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        
        model.Quest = Quest;

        UIService.Get.ShowWindow(UIWindowType.QuestWindow);
        
        var board = BoardService.Current.FirstBoard;
        
        // Focus to Char (working code - uncomment if needed)
        // var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        // var position = board.BoardDef.GetSectorCenterWorldPosition(pos.X, pos.Y, pos.Z);
        // board.Manipulator.CameraManipulator.MoveTo(position);
        
        board.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
    }

    public void ToggleCheckMark(bool enabled)
    {
        if (checkmark == null)
        {
            return;
        }

        checkmark.SetActive(enabled);
    }

    // Do not allow to open details until the end of tutorials https://app.assembla.com/spaces/sherwood/tickets/realtime_cardwall?ticket=1482
    public bool IsClickDisabledByTutorial()
    {
        return GameDataService.Current.QuestsManager.GetActiveQuestById("1_CreatePiece_PR_C4") != null;
    }
}