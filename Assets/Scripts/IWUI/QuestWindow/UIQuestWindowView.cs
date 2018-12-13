using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#TaskIcon")] private Image targetIcon;
    [IWUIBinding("#MessageLabel")] private NSText descriptionLabel;
    [IWUIBinding("#RewardLabel")] private NSText rewardLabel;
    [IWUIBinding("#TaskProgress")] private NSText amountLabel;
    [IWUIBinding("#FindButtomLabel")] private NSText buttonLabel;
    [IWUIBinding("#HintLabel")] private NSText hintLabel;
    [IWUIBinding("#Chain")] private CodexChain chain;

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
        
        UpdateUi();

        targetIcon.sprite = model.Icon;

        if (!ShowChain(model))
        {
            hintLabel.gameObject.SetActive(false);
        }

        model.Quest.OnChanged += OnQuestChanged;
    }
    
    public override void OnViewClose()
    {
        var model = Model as UIQuestWindowModel;
        model.Quest.OnChanged -= OnQuestChanged;
        
        base.OnViewClose();
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest.IsCompleted())
        {
            Controller.CloseCurrentWindow();
        }
        else
        {
            UpdateUi();
        }
    }

    private void UpdateUi()
    {
        var model = Model as UIQuestWindowModel;
        
        descriptionLabel.Text = model.Description;
        rewardLabel.Text = model.RewardText;
        amountLabel.Text = model.AmountText;
        buttonLabel.Text = model.ButtonText;
        buttonLabel.gameObject.SetActive(model.Quest.IsInProgress());
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
            GetFrom = () => position,
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

        if (quest.IsCompletedOrClaimed())
        {
            return;
        }

        quest.Tasks[0].Highlight();
    }

    private bool ShowChain(UIQuestWindowModel model)
    {
        foreach (Transform child in chain.ItemsHost) 
        {
            Destroy(child.gameObject);
        }
        
        chain.gameObject.SetActive(false);

        var taskAboutPiece = model.Quest.Tasks[0] as IHavePieceId;
        if (taskAboutPiece == null)
        {
            return false;
        }
        
        var targetId = taskAboutPiece.PieceId;
        if (targetId == PieceType.None.Id)
        {
            return false;
        }
        
        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainAndFocus(targetId, CHAIN_LENGTH);
        if (itemDefs == null)
        {
            return false;
        }
       
        chain.gameObject.SetActive(true); 
        
        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        UICodexWindowView.CreateItems(chain, chainDef, CHAIN_LENGTH);

        hintLabel.gameObject.SetActive(false);
        
        return true;
    }
}