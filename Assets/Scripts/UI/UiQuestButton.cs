using System;
using System.Collections.Generic;
using DG.Tweening;
using Quests;
using UnityEngine;

public class UiQuestButton : UIGenericResourcePanelViewController
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private UiQuestButtonArrow arrow;
    
    public QuestEntity Quest { get; private set; }
    private bool isUp;
    private bool interactive;
    
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
        
        if (this.Quest != null)
        {
            ResourcesViewManager.Instance.UnRegisterView(this);
            this.Quest.OnChanged -= OnQuestChanged;
            // Debug.Log($"AAAAA FIX UNSUBSCRIBE: {quest.Id}");
        }
        
        this.Quest = quest;

        isUp = false;
        var taskAboutPiece = GetFirstTask() as IHavePieceId;
        if (taskAboutPiece != null)
        {
            int pieceId = taskAboutPiece.PieceId;
            PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);
            itemUid = pieceTypeDef.Id.ToString();
        }
        
        // Debug.Log($"AAAAA SUBSCRIBE: {quest.Id}");

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
                rootCanvasGroup.DOFade(0, 1);
            }
        }
    }

    protected override void OnEnable()
    {
    }

    private void OnDestroy()
    {
        if (Quest != null)
        {
            // Debug.Log($"AAAAA UNSUBSCRIBE: {quest.Id}");
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

    private static Sprite GetIconForFirstTask(QuestEntity quest)
    {
        return GetIcon(quest.Tasks[0]);
    }
    
    public static Sprite GetIcon(TaskEntity task)
    {
        QuestDescriptionComponent cmp = task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid);

        if (cmp?.Ico != null)
        {
            return IconService.Current.GetSpriteById(cmp.Ico);
        }
        
        IHavePieceId taskAboutPiece = task as IHavePieceId;
        try
        {
            if (taskAboutPiece != null && taskAboutPiece.PieceId != PieceType.None.Id && taskAboutPiece.PieceId != PieceType.Empty.Id)
            {
                var pieceTypeDef = PieceType.GetDefById(taskAboutPiece.PieceId);
                return IconService.Current.GetSpriteById(pieceTypeDef.Abbreviations[0]);
            }
        }
        catch (Exception e)
        {
            throw;
        }

        return IconService.Current.GetSpriteById("codexQuestion");
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
                
        return $"<color=#{( isCompleted ? "FFFFFF" : currentValueColor)}><size={currentValueFontSize}>{current}</size></color>/{target}";
    }
    
    private void SetLabelValue(int value)
    {
        if(Quest == null) return;

        // Debug.Log($"AAAAA UPDATE: {quest.Id}");
        
        icon.sprite = GetIconForFirstTask(Quest);

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
        
        ShowQuestWindow();
    }

    private void AddQuestWindowToQueue()
    {
        //Debug.Log("!!! CompleteQuest: AddQuestWindowToQueue: " + Quest.Id);
        
        var action = new QueueActionComponent()
                    .AddCondition(new OpenedWindowsQueueConditionComponent {IgnoredWindows = UIWindowType.IgnoredWindows})
                    //.AddCondition(new NoQueuedActionsConditionComponent {ActionIds = new List<string> {"StartNewQuestsIfAny"}})
                    .SetAction(() =>
                     {
                         // ShowQuestWindow();
                         ShowQuestCompletedWindow();     
                     });

        ProfileService.Current.QueueComponent.AddAction(action, false);
    }

    private void ShowQuestCompletedWindow()
    {
        if (!DevTools.IsQuestDialogsEnabled())
        {
            DevTools.FastCompleteQuest(Quest);
            return;
        }

        var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);
        model.Init(Quest, null, null);
        
        UIService.Get.ShowWindow(UIWindowType.QuestStartWindow);
    }
    
    private void ShowQuestWindow()
    {
        var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        
        model.Quest = Quest;

        UIService.Get.ShowWindow(UIWindowType.QuestWindow);
        
        var board = BoardService.Current.GetBoardById(0);
        
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
}