using UnityEngine;

public class UiQuestButton : UIGenericResourcePanelViewController
{
    [SerializeField] private GameObject shine;
    
    private Quest quest;
    private bool isUp;

    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;
            SetLabelValue(currentValueAnimated);
        }
    }

    public void Init(Quest quest)
    {
        this.quest = quest;

        isUp = false;
        itemUid = PieceType.Parse(quest.WantedPiece);
        ResourcesViewManager.Instance.RegisterView(this);
        UpdateView();
    }

    protected override void OnEnable()
    {
    }

    private void OnDestroy()
    {
        ResourcesViewManager.Instance.UnRegisterView(this);
    }

    public override void UpdateView()
    {
        if(quest == null) return;
        
        currentValueAnimated = currentValue = quest.CurrentAmount;

        SetLabelValue(currentValue);
    }

    public override void UpdateResource(int offset)
    {
        base.UpdateResource(offset);
        quest.CurrentAmount += offset;
    }

    private void SetLabelValue(int value)
    {
        if(quest == null) return;
        
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);

        var isComplete = quest.Check();

        if (isComplete && isUp == false)
        {
            isUp = true;
            transform.SetSiblingIndex(0);
        }
        
        amountLabel.Text = $"<color=#{(isComplete ? "FFFFFF" : "FE4704")}><size=40>{Mathf.Min(value, quest.TargetAmount)}</size></color>/{quest.TargetAmount}";
        shine.SetActive(isComplete);
    }
    
    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        
        model.Quest = quest;
        
        UIService.Get.ShowWindow(UIWindowType.QuestWindow);
        
        var board = BoardService.Current.GetBoardById(0);
        var kingPos = GameDataService.Current.PiecesManager.KingPosition;
        
        var position = board.BoardDef.GetSectorCenterWorldPosition(kingPos.X, kingPos.Y, kingPos.Z);
        
        board.Manipulator.CameraManipulator.MoveTo(position);
    }
}