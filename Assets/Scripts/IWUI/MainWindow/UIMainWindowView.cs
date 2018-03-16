using UnityEngine;

public class UIMainWindowView : IWUIWindowView, IBoardEventListener
{
    [SerializeField] private NSText settingsLabel;
    
    private BoardPosition spawnAt;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        BoardService.Current.GetBoardById(0).BoardEvents.AddListener(this, GameEventsCodes.EnemyDeath);
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;
        
        UpdateSlots();
    }
    
    public override void OnViewClose()
    {
        base.OnViewClose();
        
        BoardService.Current.GetBoardById(0).BoardEvents.RemoveListener(this, GameEventsCodes.EnemyDeath);
        
        UIMainWindowModel windowModel = Model as UIMainWindowModel;
    }
    
    public void UpdateSlots()
    {
        var chests = GameDataService.Current.ChestsManager.GetActiveChests();
    }

    public void ShowSettings()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
    }

    public void OnBoardEvent(int code, object context)
    {
        GameDataService.Current.ChestsManager.AddActiveChest(GameDataService.Current.ChestsManager.Chests.Find(def => def.Uid == context.ToString()));
        
        UpdateSlots();
    }

    public void SelectEnemy()
    {
        var board = BoardService.Current.GetBoardById(0);

        var enemmyPiece = board.BoardLogic.GetPieceAt(new BoardPosition(spawnAt.X, spawnAt.Y, board.BoardDef.PieceLayer));

        // move camera
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(spawnAt.X, spawnAt.Y, spawnAt.Z);
        board.Manipulator.CameraManipulator.ZoomTo(0.3f, worldPos);
    }
}
