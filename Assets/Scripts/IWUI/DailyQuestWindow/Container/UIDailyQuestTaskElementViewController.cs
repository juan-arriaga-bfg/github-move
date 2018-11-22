using System;
using System.Collections.Generic;
using DG.Tweening;
using Quests;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyQuestTaskElementViewController : UITabElementViewController
{
    [IWUIBinding("#TaskDescription")] private NSText lblDescription;
    [IWUIBinding("#TaskProgress")] private NSText lblProgress;
    [IWUIBinding("#TaskIcon")] private Image taskIcon;
    [IWUIBinding("#TaskButton")] private Button taskButton;

    private TaskEntity task;
    
    public override void Init()
    {
        base.Init();
        
        var targetEntity = entity as UIDailyQuestTaskElementEntity;

        Init(targetEntity.Task);
    }
    
    public void Init(TaskEntity task)
    {
        this.task = task;

        taskButton.interactable = !task.IsClaimed();
        // this.pieceId = pieceId;
        //
        // if (pieceId <= 0)
        // {
        //     lblName.gameObject.SetActive(false);
        //     ico.gameObject.SetActive(false);
        //     return;
        // }
        //
        // // var pieceManager = GameDataService.Current.PiecesManager;
        // PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);
        //
        // lblName.Text = pieceTypeDef.Abbreviations[0];
        // lblId.Text = "id " + pieceId.ToString();
        //
        // var spr = GetPieecSprite();
        // if (spr != null)
        // {
        //     ico.sprite = spr;
        // }
    }

    public void OnClick()
    {
        if (task.IsClaimed())
        {
            return;
        }
        
        if (task.IsCompleted())
        {
            ProvideReward();
            return;
        }
        
        task.Highlight();
    }

    private void ProvideReward(Action onComplete = null)
    {
        taskButton.interactable = false;
        
        task.SetClaimedState();

        List<CurrencyPair> reward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        CurrencyHellper.Purchase(reward, success =>
        {
            onComplete?.Invoke();
        },
        taskIcon.transform.position);
    }
}