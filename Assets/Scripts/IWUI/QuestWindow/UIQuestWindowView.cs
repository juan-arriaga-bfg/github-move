using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestWindowView : UIGenericPopupWindowView
{
    [SerializeField] private Image targetIcon;
    
    [SerializeField] private NSText descriptionLabel;
    [SerializeField] private NSText rewardLabel;
    [SerializeField] private NSText amountLabel;
    [SerializeField] private NSText buttonLabel;

    private bool isComplete;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIQuestWindowModel;

        isComplete = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        descriptionLabel.Text = windowModel.Description;
        rewardLabel.Text = windowModel.RewardText;
        amountLabel.Text = windowModel.AmountText;
        buttonLabel.Text = windowModel.ButtonText;

        targetIcon.sprite = windowModel.Icon;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIQuestWindowModel;
    }

    public override void OnViewCloseCompleted()
    {
        if(isComplete == false) return;
        
        var windowModel = Model as UIQuestWindowModel;
        
        BoardService.Current.GetBoardById(0).ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = GameDataService.Current.PiecesManager.KingPosition,
            Pieces = windowModel.Quest.Rewards
        });
    }

    public void OnClick()
    {
        var windowModel = Model as UIQuestWindowModel;
        var quest = windowModel.Quest;
        
        if (quest.Check())
        {
            GameDataService.Current.QuestsManager.RemoveActiveQuest(quest);
            
            isComplete = true;
            return;
        }

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
        });
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}