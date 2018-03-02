using System.Collections.Generic;
using UnityEngine;

public class UIMainWindowView : IWUIWindowView, IBoardEventListener
{
    [SerializeField] private NSText settingsLabel;
    [SerializeField] private NSText fightLabel;
    [SerializeField] private NSText damegeLabel;
    
    [SerializeField] private GameObject robin;
    
    [SerializeField] private List<UIChestSlot> slots;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        BoardService.Current.GetBoardById(0).BoardEvents.AddListener(this, GameEventsCodes.EnemyDeath);
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;
        fightLabel.Text = windowModel.FightText;
        
        robin.SetActive(false);
        
        UpdateSlots();
    }
    
    public override void OnViewClose()
    {
        base.OnViewClose();
        
        BoardService.Current.GetBoardById(0).BoardEvents.RemoveListener(this, GameEventsCodes.EnemyDeath);
        
        UIMainWindowModel windowModel = Model as UIMainWindowModel;
    }

    public override void UpdateView(IWWindowModel model)
    {
        base.UpdateView(model);
        
        var hero = GameDataService.Current.GetHero("Robin");
        var level = GameDataService.Current.HeroLevel;
        var heroDamage = hero.Damages[level];
        
        damegeLabel.Text = heroDamage.ToString();
    }

    public void UpdateSlots()
    {
        var chests = GameDataService.Current.GetActiveChests();
        
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            
            slot.Initialize(i < chests.Count ? chests[i] : null);
        }
    }

    public void ShowSettings()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
    }
    
    public void StartFight()
    {
        var enemy = GameDataService.Current.GetEnemy();
        
        if(enemy == null) return;
        
        var board = BoardService.Current.GetBoardById(0);
        
        var free = new List<BoardPosition>();
        
        board.BoardLogic.EmptyCellsFinder.FindAllWithPointInHome(new BoardPosition(8, 8, board.BoardDef.PieceLayer), 15, 15, free);
        
        if(free.Count == 0) return;
        
        robin.SetActive(true);

        var spawnAt = free[Random.Range(0, free.Count)];
        
        board.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = spawnAt,
            PieceTypeId = PieceType.Parse(enemy.Skin),
            OnFailedAction = (action =>
            {
                robin.SetActive(false);
            }) 

        });
        
        // move camera
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(spawnAt.X, spawnAt.Y, spawnAt.Z);
        board.Manipulator.CameraManipulator.ZoomTo(0.3f, worldPos);
    }

    public void OnBoardEvent(int code, object context)
    {
        GameDataService.Current.AddActiveChest(GameDataService.Current.Chests.Find(def => def.Uid == context.ToString()));
        
        UpdateSlots();
        robin.SetActive(false);
    }
}
