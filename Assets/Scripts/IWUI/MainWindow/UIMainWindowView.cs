using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private NSText settingsLabel;
    [SerializeField] private List<UiChestSlot> slots;
    
    private List<UiQuestButton> quests;
    
    private BoardPosition spawnAt;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;
        
        UpdateSlots();
        UpdateQuest();
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
