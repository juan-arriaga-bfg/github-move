using UnityEngine;
using UnityEngine.UI;

public class CastleUpgradeItem : MonoBehaviour, IBoardEventListener
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText progress;
    [SerializeField] private GameObject check;

    private Quest quest;

    public void Init(CurrencyPair price)
    {
        quest = new Quest(new QuestDef{Price = price});
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);
        check.SetActive(false);
        
        var board = BoardService.Current.GetBoardById(0);
        board.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
        Decoration();
    }
    
    private void Decoration()
    {
        if (quest.Check())
        {
            var board = BoardService.Current.GetBoardById(0);
            board.BoardEvents.RemoveListener(this, GameEventsCodes.CreatePiece);
            
            check.SetActive(true);
            progress.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            return;
        }
        
        progress.Text = string.Format("<color=#FE4704><size=40>{0}</size></color>/{1}", quest.CurrentAmount, quest.TargetAmount);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.CreatePiece) return;

        var piece = (int)context;
        
        if(piece != quest.WantedPiece) return;

        quest.CurrentAmount++;
        Decoration();
    }

    public bool IsComplete
    {
        get { return quest.Check(); }
    }
}