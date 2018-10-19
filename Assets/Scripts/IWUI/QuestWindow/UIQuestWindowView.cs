using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        
        var model = Model as UIQuestWindowModel;

        isComplete = false;
        
        SetTitle(model.Title);
        SetMessage(model.Message);

        descriptionLabel.Text = model.Description;
        rewardLabel.Text = model.RewardText;
        amountLabel.Text = model.AmountText;
        buttonLabel.Text = model.ButtonText;

        targetIcon.sprite = model.Icon;

        ShowChainIfPossible(model);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIQuestWindowModel;
    }

    private List<CurrencyPair> RewardsCurrency(List<CurrencyPair> rewards)
    {
        return rewards.FindAll(pair => PieceType.Parse(pair.Currency) == PieceType.None.Id); 
    }
    
    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIQuestWindowModel;
        
        if(isComplete == false)
        {
            windowModel.Quest = null;
            return;
        }
        
        Dictionary<int, int> pieces = windowModel.ConvertRewardsToDict(windowModel.Reward);
        List<CurrencyPair> rewards = RewardsCurrency(windowModel.Reward);
        var board = BoardService.Current.GetBoardById(0);
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        
        windowModel.Quest = null;
        
        board.ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = position,
            Pieces = pieces,
            OnComplete = () =>
            {
                var sequence = DOTween.Sequence();
                
                for (var i = 0; i < rewards.Count; i++)
                {
                    var reward = rewards[i];
                    sequence.InsertCallback(0.5f*i, () => AddResourceView.Show(position, reward));
                }
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
            var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
            Debug.LogError($"{string.Join(",", windowModel.Reward.Select(e => $"{e.Currency} {e.Amount}"))}");
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, windowModel.Reward.Sum(e => e.Amount)))
            {
                UIErrorWindowController.AddError("Need more free cells");
                return;
            }
            
            quest.SetClaimedState();
            GameDataService.Current.QuestsManager.CompleteQuest(quest.Id);
            
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