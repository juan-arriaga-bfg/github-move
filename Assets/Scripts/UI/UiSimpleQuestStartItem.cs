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

        var title = "";
        var image = "";
        
        if(piece == PieceType.A1.Id)
        {
            title = "Need wooden pieces?";
            image = "wood_UI";
            position = GameDataService.Current.PiecesManager.SawmillPosition;
        }
        else if(piece == PieceType.B1.Id)
        {
            title = "Need wheat pieces?";
            image = "hay_UI";
        }
        else if(piece == PieceType.C1.Id)
        {
            title = "Need stone pieces?";
            image = "stone_UI";
            position = GameDataService.Current.PiecesManager.MinePosition;
        }
        else if(piece == PieceType.D1.Id)
        {
            title = "Need sheep pieces?";
            image = "sheeps_UI";
            position = GameDataService.Current.PiecesManager.SheepfoldPosition;
        }
        else if(piece == PieceType.E1.Id)
        {
            title = "Need apple pieces?";
            image = "apple_UI";
        }
        
        UIMessageWindowController.CreateImageMessage(title, image, () =>
        {
            if (position == null || position.Value.X == 0 && position.Value.Y == 0) return;
            HintArrowView.Show(position.Value);
            UIService.Get.CloseWindow(UIWindowType.SimpleQuestStartWindow, true);
        });
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}