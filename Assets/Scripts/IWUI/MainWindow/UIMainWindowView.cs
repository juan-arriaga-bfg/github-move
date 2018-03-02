using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainWindowView : IWUIWindowView, IBoardEventListener
{
    [SerializeField] private NSText settingsLabel;
    [SerializeField] private NSText fightLabel;
    [SerializeField] private NSText fightPriceLabel;
    [SerializeField] private Image fightPriceIcon;
    [SerializeField] private NSText damegeLabel;
    
    [SerializeField] private GameObject robin;
    
    [SerializeField] private List<UIChestSlot> slots;
    
    private BoardPosition spawnAt;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        BoardService.Current.GetBoardById(0).BoardEvents.AddListener(this, GameEventsCodes.EnemyDeath);
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;
        fightLabel.Text = windowModel.FightText;
        

        fightPriceLabel.Text = windowModel.GetPriceLabelForFight();
        fightPriceIcon.gameObject.SetActive(windowModel.GetPriceForFight() > 0);
        
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
        var windowModel = Model as UIMainWindowModel;
        
        if (GameDataService.Current.GetActiveChests().Count >= 4)
        {
            UIMessageWindowController.CreateDefaultMessage("No free slots for chest!");
            return;
        }
        
        var enemy = GameDataService.Current.FightEnemy();

        if (enemy == null)
        {
            UIMessageWindowController.CreateDefaultMessage("Not enough coins to enter the battle!");
            return;
        }
        
        var free = new List<BoardPosition>();
        var board = BoardService.Current.GetBoardById(0);
        
        board.BoardLogic.EmptyCellsFinder.FindAllWithPointInHome(new BoardPosition(8, 8, board.BoardDef.PieceLayer), 15, 15, free);

        if (free.Count == 0)
        {
            UIMessageWindowController.CreateDefaultMessage("No free cells on the field to enter the battle!");
            return;
        }
        
        robin.SetActive(true);
        
        // update price label
        fightPriceLabel.Text = windowModel.GetPriceLabelForFight();
        fightPriceIcon.gameObject.SetActive(windowModel.GetPriceForFight() > 0);

        spawnAt = free[Random.Range(0, free.Count)];
        
        board.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = spawnAt,
            PieceTypeId = PieceType.Parse(enemy.Skin),
            OnFailedAction = (action =>
            {
                robin.SetActive(false);
                
                var fallBackShopItem = new ShopItem
                {
                    Uid = string.Format("purchase.test.enemy.fallback"), 
                    ItemUid = enemy.Price.Currency, 
                    Amount = enemy.Price.Amount,
                    CurrentPrices = new List<Price>
                    {
                        new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
                    }
                };
                ShopService.Current.PurchaseItem(fallBackShopItem);
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

    public void SelectEnemy()
    {
        var board = BoardService.Current.GetBoardById(0);

        var enemmyPiece = board.BoardLogic.GetPieceAt(new BoardPosition(spawnAt.X, spawnAt.Y, board.BoardDef.PieceLayer));

        if (enemmyPiece == null || enemmyPiece.PieceType < PieceType.E1.Id || enemmyPiece.PieceType >= (PieceType.E1.Id + 100))
        {
            robin.SetActive(false);
        }
        
        // move camera
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(spawnAt.X, spawnAt.Y, spawnAt.Z);
        board.Manipulator.CameraManipulator.ZoomTo(0.3f, worldPos);
    }
}
