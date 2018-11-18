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
    [SerializeField] private Transform anchor;

    private bool isComplete;

    private const int CHAIN_LENGTH = 5;

    private Transform icon;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var model = Model as UIQuestWindowModel;

        isComplete = false;
        
        SetTitle(model.Title);
        SetMessage(model.Message);
        
        model.InitReward();
        
        descriptionLabel.Text = model.Description;
        rewardLabel.Text = model.RewardText;
        amountLabel.Text = model.AmountText;
        buttonLabel.Text = model.ButtonText;
        
        targetIcon.sprite = model.Icon;

        ShowChainIfPossible(model);
        
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(PieceType.NPC_SleepingBeauty.Abbreviations[0]));
        icon.SetParentAndReset(anchor);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIQuestWindowModel;
    }
    
    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIQuestWindowModel;
        
        windowModel.Quest = null;
        
        UIService.Get.PoolContainer.Return(icon.gameObject);
        
        if(isComplete == false) return;
        
        var board = BoardService.Current.GetBoardById(0);
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
        
        board.ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = position,
            Pieces = windowModel.PiecesReward,
            OnComplete = () =>
            {
                var view = board.RendererContext.GetElementAt(position) as CharacterPieceView;
                
                if(view != null) view.StartRewardAnimation();
                
                AddResourceView.Show(position, windowModel.CurrencysReward);
            }
        });
    }

    public void OnClick()
    {
        var windowModel = Model as UIQuestWindowModel;
        var quest = windowModel.Quest;
        
        var board = BoardService.Current.GetBoardById(0);
        
        if (quest.IsCompleted())
        {
            var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
            
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, windowModel.PiecesReward.Sum(e => e.Value)))
            {
                UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.freeSpace", "Free space not found!"));
                return;
            }
            
            quest.SetClaimedState();
            GameDataService.Current.QuestsManager.FinishQuest(quest.Id);
            
            isComplete = true;
            return;
        }

        quest.Tasks[0].Highlight();
    }

    private void ShowChainIfPossible(UIQuestWindowModel model)
    {
        foreach (Transform child in chain.ItemsHost) 
        {
            Destroy(child.gameObject);
        }
        
        chain.gameObject.SetActive(false);

        var taskAboutPiece = model.Quest.Tasks[0] as IHavePieceId;
        if (taskAboutPiece == null)
        {
            return;
        }
        
        var targetId = taskAboutPiece.PieceId;
        if (targetId == PieceType.None.Id)
        {
            return;
        }
        
        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainAndFocus(targetId, CHAIN_LENGTH);
        if (itemDefs == null)
        {
            return;
        }
       
        chain.gameObject.SetActive(true); 
        
        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        UICodexWindowView.CreateItems(chain, chainDef, codexItemPrefab, CHAIN_LENGTH);
    }
}