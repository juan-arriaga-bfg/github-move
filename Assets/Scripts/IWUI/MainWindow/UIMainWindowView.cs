using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private NSText settingsLabel;
    
    [SerializeField] private List<UiChestSlot> slots;
    
    private List<UiQuestButton> quests;
    
    private BoardPosition spawnAt;

    private string chestHintArrowDelayOpen= "chestHintArrowDelayOpen";
    
    private string chestHintArrowDelayFinished = "chestHintArrowDelayFinished";

    private const float delayBeforeChestHint = 15f;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;

        /*for (var i = 0; i < slots.Count; i++)
        {
            var chestView = slots[i];
            chestView.OnChestStateChanged = OnChestStateChanged;
        }

        UpdateSlots();*/
        UpdateQuest();
    }

    private void OnChestStateChanged(UiChestSlot uiChestSlot, ChestState chestState)
    {
        CheckForHighlightChest();
    }

    public void ShowSettings()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
    }

    public void SelectEnemy()
    {
        var board = BoardService.Current.GetBoardById(0);

        var enemmyPiece = board.BoardLogic.GetPieceAt(new BoardPosition(spawnAt.X, spawnAt.Y, board.BoardDef.PieceLayer));

        // move camera
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(spawnAt.X, spawnAt.Y, spawnAt.Z);
        board.Manipulator.CameraManipulator.ZoomTo(0.3f, worldPos);
    }

    public UiChestSlot GetFirstChestWithState(ChestState chestState)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            var chestView = slots[i];

            var chest = chestView.Chest;
            if (chest == null) continue;

            if (chest.State == chestState)
            {
                return chestView;
            }
        }

        return null;
    }
    
    public void UpdateSlots()
    {
        for (var i = 0; i < slots.Count; i++)
        {
            Chest chest = null;
            
            if (i < GameDataService.Current.ChestsManager.ActiveChests.Count)
            {
                chest = GameDataService.Current.ChestsManager.ActiveChests[i];
            }
            
            slots[i].Init(chest);
        }
    }

    public void ClearChestHintArrows()
    {
        for (var i = 0; i < slots.Count; i++)
        {
            var chestView = slots[i];

            chestView.ToggleHintArrow(false);
        }
    }

    public void CheckForHighlightChest()
    {
        int chestsState = -1;

        for (var i = 0; i < slots.Count; i++)
        {
            var chestView = slots[i];

            var chest = chestView.Chest;
            if (chest == null) continue;;

            if (chest.State == ChestState.Open)
            {
                chestsState = 2;
            }
            else if (chest.State == ChestState.InProgress && chestsState == -1)
            {
                chestsState = 1;
            }
            else if (chest.State == ChestState.Close && chestsState == -1)
            {
                chestsState = 0;
            }
        }
        
        switch (chestsState)
        {
            case 2:
            {
                if (DOTween.IsTweening(chestHintArrowDelayOpen))
                {
                    return;
                }

                DOTween.Kill(chestHintArrowDelayFinished);

                ClearChestHintArrows();
                var sequence = DOTween.Sequence().SetId(chestHintArrowDelayOpen);
                sequence.AppendInterval(delayBeforeChestHint);
                sequence.OnComplete(() =>
                {
                    ClearChestHintArrows();
                    var targetChestView = GetFirstChestWithState(ChestState.Open);
                    if (targetChestView != null)
                    {
                        targetChestView.ToggleHintArrow(true);
                    }
                });

                break;
            }
            case 0:
            {
                if (DOTween.IsTweening(chestHintArrowDelayFinished))
                {
                    return;
                }
                
                DOTween.Kill(chestHintArrowDelayOpen);
                
                ClearChestHintArrows();
                var sequence = DOTween.Sequence().SetId(chestHintArrowDelayFinished);
                sequence.AppendInterval(delayBeforeChestHint);
                sequence.OnComplete(() =>
                {
                    ClearChestHintArrows();
                    var targetChestView = GetFirstChestWithState(ChestState.Close);
                    if (targetChestView != null)
                    {
                        targetChestView.ToggleHintArrow(true);
                    }
                });
                
                
                break;
            }
            case 1:
            {
                DOTween.Kill(chestHintArrowDelayOpen);
                
                DOTween.Kill(chestHintArrowDelayFinished);
                
                ClearChestHintArrows();
                break;
            }
        }
        
    }

    public void UpdateQuest()
    {
        var active = GameDataService.Current.QuestsManager.ActiveQuests;
        quests = GetComponentsInChildren<UiQuestButton>().ToList();
        
        for (int i = 0; i < active.Count; i++)
        {
            quests[i].Init(active[i]);
        }
    }
}
