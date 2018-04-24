using UnityEngine;
using UnityEngine.UI;

public class UiQuestButton : MonoBehaviour, IBoardEventListener
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText progressLabel;
    
    private Quest quest;
    
    public void Init(Quest quest)
    {
        this.quest = quest;
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);
        
        progressLabel.Text = string.Format("<color=#{0}><size=45>{1}</size></color>/{2}", quest.Check() ? "FFFFFF" : "FE4704", quest.CurrentAmount, quest.TargetAmount);
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