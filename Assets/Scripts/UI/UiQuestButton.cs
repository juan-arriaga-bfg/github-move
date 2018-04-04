using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestButton : MonoBehaviour, IBoardEventListener
{
    [SerializeField] private Image icon;
    
    [SerializeField] private GameObject body;
    
    [SerializeField] private CanvasGroup newHint;
    [SerializeField] private CanvasGroup completeHint;

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