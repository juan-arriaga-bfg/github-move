using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestWindowView : UIGenericPopupWindowView
{
    [SerializeField] private Image targetIcon;
    
    [SerializeField] private NSText descriptionLabel;
    [SerializeField] private NSText rewardLabel;
    [SerializeField] private NSText amountLabel;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private CodexChain chain;
    [SerializeField] private GameObject codexItemPrefab;

    private bool isComplete;

    private const int CHAIN_LENGTH = 5;
    
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

        CreateChain(windowModel);
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
        
        var board = BoardService.Current.GetBoardById(0);
        if (quest.Check())
        {
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(GameDataService.Current.PiecesManager.KingPosition, quest.Rewards.Values.Sum()))
            {
                UIErrorWindowController.AddError("Need more free cells");
                return;
            }
            
            GameDataService.Current.QuestsManager.RemoveActiveQuest(quest);
            
            isComplete = true;
            return;
        }
        
        var piece = board.BoardLogic.MatchDefinition.GetFirst(quest.WantedPiece);
        
        if(piece == PieceType.None.Id) return;
        
        BoardPosition? position = null;

        var title = "";
        var image = "";

        var positions = new List<BoardPosition>();
        
        if(piece == PieceType.A1.Id)
        {
            title = "Need wooden pieces?";
            image = "wood_UI";
            positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceTypeFilter.Obstacle, 1);
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
            
            positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.MineC.Id, 1);
        }
        else if(piece == PieceType.D1.Id)
        {
            title = "Need sheep pieces?";
            image = "sheeps_UI";
        }
        else if(piece == PieceType.E1.Id)
        {
            title = "Need apple pieces?";
            image = "apple_UI";
        }
        
        if (positions.Count != 0) position = positions[0];
        
        UIMessageWindowController.CreateImageMessage(title, image, () =>
        {
            if (position == null || position.Value.X == 0 && position.Value.Y == 0)
            {
                UIService.Get.ShowWindow(UIWindowType.CastleWindow);
                return;
            }
            
            board.HintCooldown.Step(position.Value);
        });
    }

    private void CreateChain(UIQuestWindowModel model)
    {
        foreach (Transform child in chain.ItemsHost) 
        {
            Destroy(child.gameObject);
        }
        
        var targetId = model.Quest.WantedPiece;
        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainAndFocus(targetId, CHAIN_LENGTH);
        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        UICodexWindowView.CreateItems(chain, chainDef, codexItemPrefab, CHAIN_LENGTH);
    }
}