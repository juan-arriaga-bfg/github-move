using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestButton : MonoBehaviour, IBoardEventListener
{
    [SerializeField] private Image icon;
    
    [SerializeField] private GameObject body;
    
    [SerializeField] private CanvasGroup newHint;
    [SerializeField] private CanvasGroup completeHint;

    [SerializeField] private Transform sendItemsView;
    
    [SerializeField] private Image iconForSendButton;
    
    [SerializeField] private NSText sendItemsAmountLabel;

    [SerializeField] private UIProgressBarVIewController progressBar;

    private Quest quest;
    
    public void Init(Quest quest)
    {
        this.quest = quest;
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);

        newHint.alpha = 0;
        completeHint.alpha = 0;

        switch (quest.State)
        {
            case QuestState.New:
                Animate(newHint, false);
                break;
            case QuestState.InProgress:
                Animate(newHint, true);
                Animate(completeHint, true);
                break;
            case QuestState.Complete:
                Animate(completeHint, false);
                break;
        }

        int canClaimFull = quest.GetPiecesAmountOnBoard();

        int canClaim = Mathf.Clamp(canClaimFull, 0, quest.TargetAmount - quest.CurrentAmount);
        
        Debug.LogWarning("canClaim: " + canClaim.ToString());
        
        if (quest.CurrentAmount < quest.TargetAmount && canClaim > 0)
        {
            sendItemsView.gameObject.SetActive(true);
        }
        else
        {
            sendItemsView.gameObject.SetActive(false);
        }

        progressBar.SetProgress(quest.CurrentAmount, quest.TargetAmount);
        
        sendItemsAmountLabel.Text = canClaim.ToString();
        
        iconForSendButton.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);
    }

    private void OnEnable()
    {
        if(BoardService.Current == null) return;
        
        var board = BoardService.Current.GetBoardById(0);
        
        board.BoardEvents.AddListener(this, GameEventsCodes.ChangePiecePosition);
    }

    private void OnDisable()
    {
        if(BoardService.Current == null) return;
        
        var board = BoardService.Current.GetBoardById(0);
        
        board.BoardEvents.RemoveListener(this, GameEventsCodes.ChangePiecePosition);
    }

    public void SendItems()
    {
        if (quest == null) return;
        
        int canClaimFull = quest.GetPiecesAmountOnBoard();
        int canClaim = Mathf.Clamp(canClaimFull, 0, quest.TargetAmount - quest.CurrentAmount);
        
        quest.SendToQuest(canClaim);
        
        Init(quest);
    }

    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UISimpleQuestStartWindowModel>(UIWindowType.SimpleQuestStartWindow);
        
        model.Quest = quest;
        
        UIService.Get.ShowWindow(UIWindowType.SimpleQuestStartWindow);

        if (quest.State != QuestState.New) return;
        
        quest.State = QuestState.InProgress;
        Init(quest);
    }

    private void Animate(CanvasGroup group, bool isHide)
    {
        DOTween.Kill(this, true);
        
        if((int)group.alpha == (isHide ? 0 : 1)) return;

        group.DOFade(isHide ? 0 : 1, 0.5f).SetId(this)
            .OnComplete(() => { group.alpha = isHide ? 0 : 1; });
    }

    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ChangePiecePosition) return;

        var piece = (int)context;
        
        if(piece != quest.WantedPiece) return;

        Init(quest);
    }
}