using UnityEngine;
using UnityEngine.UI;

public class UiSimpleQuestStartItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    
    [SerializeField] private NSText progressLabel;
    
    [SerializeField] private RectTransform progress;
    
    [SerializeField] private GameObject btn;
    [SerializeField] private GameObject check;

    private Quest quest;
    
    private bool isComplete;
    
    
    public void Init(Quest quest)
    {
        this.quest = quest;
        
        var current = quest.CurrentAmount;
        var target = quest.TargetAmount;
        
        isComplete = quest.Check();
        
        background.sprite = IconService.Current.GetSpriteById(string.Format("back_is_{0}", isComplete.ToString().ToLower()));
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);
        
        progressLabel.Text = string.Format("{0}/{1}", current, target);
        progress.sizeDelta = new Vector2(Mathf.Clamp(145 * current / (float) target, 0, 145), progress.sizeDelta.y);
        
        btn.SetActive(!isComplete);
        check.SetActive(isComplete);
    }

    public void OnClick()
    {
        if(isComplete) return;

        var piece = BoardService.Current.GetBoardById(0).BoardLogic.MatchDefinition.GetFirst(quest.WantedPiece);
        
        if(piece == PieceType.None.Id) return;
        
        BoardPosition? position = null;
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Message = null;
        model.OnCancel = null;
        model.AcceptLabel = "Ok";
        
        if(piece == PieceType.A1.Id)
        {
            model.Title = "Need wooden pieces?";
            model.Image = "wood_UI";
            position = GameDataService.Current.PiecesManager.SawmillPosition;
        }
        else if(piece == PieceType.B1.Id)
        {
            model.Title = "Need wheat pieces?";
            model.Image = "hay_UI";
        }
        else if(piece == PieceType.C1.Id)
        {
            model.Title = "Need stone pieces?";
            model.Image = "stone_UI";
            position = GameDataService.Current.PiecesManager.MinePosition;
        }
        else if(piece == PieceType.D1.Id)
        {
            model.Title = "Need sheep pieces?";
            model.Image = "sheeps_UI";
            position = GameDataService.Current.PiecesManager.SheepfoldPosition;
        }
        else if(piece == PieceType.E1.Id)
        {
            model.Title = "Need apple pieces?";
            model.Image = "apple_UI";
        }
        
        model.OnAccept = () =>
        {
            if (position == null || position.Value.X == 0 && position.Value.Y == 0) return;
            HintArrowView.Show(position.Value);
        };
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}