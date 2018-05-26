using UnityEngine;
using UnityEngine.UI;

public class UiQuestButton : MonoBehaviour, IBoardEventListener
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private GameObject shine;
    
    private Quest quest;
    
    public void Init(Quest quest)
    {
        this.quest = quest;
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);

        var isComplete = quest.Check();
        
        progressLabel.Text = string.Format("<color=#{0}><size=40>{1}</size></color>/{2}", isComplete ? "FFFFFF" : "FE4704", quest.CurrentAmount, quest.TargetAmount);
        shine.SetActive(isComplete);
    }

    private void OnEnable()
    {
        if(BoardService.Current == null) return;
        
        var board = BoardService.Current.GetBoardById(0);
        
        board.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
    }

    private void OnDisable()
    {
        if(BoardService.Current == null) return;
        
        var board = BoardService.Current.GetBoardById(0);
        
        board.BoardEvents.RemoveListener(this, GameEventsCodes.CreatePiece);
    }
    
    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        
        model.Quest = quest;
        
        UIService.Get.ShowWindow(UIWindowType.QuestWindow);
        
        var board = BoardService.Current.GetBoardById(0);
        var kingPos = GameDataService.Current.PiecesManager.KingPosition;
        
        var position = board.BoardDef.GetSectorCenterWorldPosition(kingPos.X, kingPos.Y, kingPos.Z);
        
        board.Manipulator.CameraManipulator.ZoomTo(0f, position);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.CreatePiece) return;

        var piece = (int)context;
        
        if(piece != quest.WantedPiece) return;

        quest.CurrentAmount++;
        Init(quest);
    }
}