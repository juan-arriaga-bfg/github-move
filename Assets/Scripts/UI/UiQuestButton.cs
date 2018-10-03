using UnityEngine;

public class UiQuestButton : UIGenericResourcePanelViewController
{
    [SerializeField] private GameObject shine;
    
    private QuestEntity quest;
    private bool isUp;

    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;
            SetLabelValue(currentValueAnimated);
        }
    }

    public void Init(QuestEntity quest)
    {
        if(this.quest != null) ResourcesViewManager.Instance.UnRegisterView(this);
        
        this.quest = quest;

        isUp = false;
        var taskAboutPiece = quest.ActiveTasks[0] as IHavePieceId;
        if (taskAboutPiece != null)
        {
            int pieceId = taskAboutPiece.PieceId;
            PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);
            itemUid = pieceTypeDef.Id.ToString();
        }
        quest.OnChanged += OnQuestChanged;
        
        ResourcesViewManager.Instance.RegisterView(this);
        UpdateView();
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity task)
    {
        UpdateView();
    }

    protected override void OnEnable()
    {
    }

    private void OnDestroy()
    {
        if (quest != null)
        {
            quest.OnChanged -= OnQuestChanged;
        }
        ResourcesViewManager.Instance.UnRegisterView(this);
    }

    public override void UpdateView()
    {
        if(quest == null) return;
        
        int value = (quest.ActiveTasks[0] as TaskCounterEntity).TargetValue;

        currentValueAnimated = value;
        currentValue = value;

        SetLabelValue(currentValue);
    }

    public static Sprite GetIcon(QuestEntity quest)
    {
        QuestDescriptionComponent cmp = quest.ActiveTasks[0].GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid);

        if (cmp?.Ico != null)
        {
            return IconService.Current.GetSpriteById(cmp.Ico);
        }
        
        IHavePieceId taskAboutPiece = quest.ActiveTasks[0] as IHavePieceId;
        if (taskAboutPiece != null && taskAboutPiece.PieceId != PieceType.None.Id)
        {
            return IconService.Current.GetSpriteById(PieceType.GetDefById(taskAboutPiece.PieceId).Abbreviations[0]); 
        }

        return IconService.Current.GetSpriteById("codexQuestion");
    }
    
    private void SetLabelValue(int value)
    {
        if(quest == null) return;

        icon.sprite = GetIcon(quest);

        var isComplete = quest.IsCompleted();

        if (isComplete && isUp == false)
        {
            isUp = true;
            transform.SetSiblingIndex(0);
            OnClick();
        }

        int targetValue = (quest.ActiveTasks[0] as TaskCounterEntity).TargetValue;
        int curValue = (quest.ActiveTasks[0] as TaskCounterEntity).CurrentValue;
        
        amountLabel.Text = $"<color=#{(isComplete ? "FFFFFF" : "FE4704")}><size=33>{Mathf.Min(value, curValue)}</size></color>/{targetValue}";
        shine.SetActive(isComplete);
    }
    
    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        
        model.Quest = quest;
        
        UIService.Get.ShowWindow(UIWindowType.QuestWindow);
        
        var board = BoardService.Current.GetBoardById(0);
        var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        var position = board.BoardDef.GetSectorCenterWorldPosition(pos.X, pos.Y, pos.Z);
        
        board.Manipulator.CameraManipulator.MoveTo(position);
        board.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
    }
}