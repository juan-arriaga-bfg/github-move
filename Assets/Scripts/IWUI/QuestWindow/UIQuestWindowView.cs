using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#TaskIcon")] private Image targetIcon;
    [IWUIBinding("#RewardLabel")] private NSText rewardLabel;
    [IWUIBinding("#TaskProgress")] private NSText progressLabel;
    [IWUIBinding("#FindButtonLabel")] private NSText buttonLabel;
    [IWUIBinding("#HintLabel")] private NSText hintLabel;
    [IWUIBinding("#Chain")] private CodexChain chain;
    [IWUIBinding("#FindButton")] private UIButtonViewController findButton;

    private const int CHAIN_LENGTH = 5;

    private TaskEntity firstTask;

    public override void OnViewShow()
    {
        base.OnViewShow();

        var model = Model as UIQuestWindowModel;

        Debug.Log($"[UIQuestWindowView] => OnViewShow: Quest: {model.Quest.Id}");
        
        firstTask = model.Quest.Tasks[0];

        SetTitle(LocalizationService.Get("window.quest.title", "window.quest.title"));

        UpdateUi();

        targetIcon.sprite = UiQuestButton.GetIcon(firstTask);

        if (!ShowChain(model))
        {
            hintLabel.gameObject.SetActive(true);
        }
        
        model.Quest.OnChanged += OnQuestChanged;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(findButton, OnClick);
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

        message.Text = GetMessageText();
        hintLabel.Text    = GetHintText();
        rewardLabel.Text  = GetRewardText();
        progressLabel.Text  = GetProgressText();
        buttonLabel.Text  = GetButtonText();
        
        buttonLabel.gameObject.SetActive(model.Quest.IsInProgress());
    }

    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIQuestWindowModel;

        windowModel.Quest = null;

        var action = windowModel.OnClosed;
        windowModel.OnClosed = null;
        action?.Invoke();
    }

    public void OnClick()
    {
        var windowModel = Model as UIQuestWindowModel;
        var quest       = windowModel.Quest;

        if (quest.IsCompletedOrClaimed())
        {
            return;
        }

        quest.Tasks[0].Highlight();
        
        Controller.CloseCurrentWindow();
    }

    private bool ShowChain(UIQuestWindowModel model)
    {
        chain.ReturnContentToPool();

        var taskAboutPiece = model.Quest.Tasks[0] as IHavePieceId;
        if (taskAboutPiece == null)
        {
            return false;
        }

        var targetId = taskAboutPiece.PieceId;
        if (targetId == PieceType.None.Id || targetId == PieceType.Empty.Id)
        {
            return false;
        }

        var hintText = GetHintText();
        if (!string.IsNullOrEmpty(hintText))
        {
            Debug.Log($"[UIQuestWindowView] => ShowChain: has piece id but cancelled by hint: {hintText}");
            return false;
        }
        
        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainAndFocus(targetId, CHAIN_LENGTH, true);
        if (itemDefs == null)
        {
            return false;
        }
        
        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        
        UICodexWindowView.CreateItems(chain, chainDef, CHAIN_LENGTH);

        hintLabel.gameObject.SetActive(false);

        return true;
    }

    private string GetRewardText()
    {
        var windowModel = Model as UIQuestWindowModel;

        var reward = windowModel.Quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        List<CurrencyPair> currencysReward;
        var piecesReward = CurrencyHellper.FiltrationRewards(reward, out currencysReward);

        var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
        var strBuilder = new StringBuilder($"{str}");
        strBuilder.Append(CurrencyHellper.RewardsToString("  ", piecesReward, currencysReward));

        return strBuilder.ToString();
    }

    private string GetProgressText()
    {
        return UiQuestButton.GetTaskProgress(firstTask, 36);
    }
    
    private string GetMessageText()
    {
        return firstTask.GetLocalizedMessage();
    }

    private string GetHintText()
    {
        return firstTask.GetLocalizedHint();
    }

    private string GetButtonText()
    {
        return LocalizationService.Get("common.button.find", "common.button.find");
    }
}