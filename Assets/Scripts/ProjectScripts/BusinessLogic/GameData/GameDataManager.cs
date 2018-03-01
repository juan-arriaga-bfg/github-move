using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    public List<ChestDef> Chests;
    public List<EnemyDef> Enemies;
    public List<HeroDef> Heroes;
    
    private List<ChestDef> activeChests;
    private int currentEnemy;

    public bool AddActiveChest(ChestDef chest)
    {
        if (activeChests.Count == 4) return false;
        
        activeChests.Add(chest);
        
        return true;
    }
    
    public List<ChestDef> GetActiveChests()
    {
        return activeChests;
    }

    public EnemyDef GetEnemy()
    {
        EnemyDef enemy;
        
        if (currentEnemy > Enemies.Count)
        {
            var last = Enemies[Enemies.Count - 1];
            var factor = currentEnemy - Enemies.Count - 1;
            
            enemy = new EnemyDef
            {
                Skin = string.Format("E{0}", Random.Range(0, 4)),
                HP = last.HP + 50 * factor,
                Price = new CurrencyPair{Currency = last.Price.Currency, Amount = last.Price.Amount + 10 * factor},
                Chest = Chests[Random.Range(0, Chests.Count)].Uid
            };
        }
        else
        {
            enemy = Enemies[currentEnemy];
        }
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Enemy.Name), 
            ItemUid = Currency.Enemy.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = enemy.Price.Currency, DefaultPriceAmount = enemy.Price.Amount}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                currentEnemy++;
            },
            item =>
            {
                // on purchase failed (not enough cash)
                enemy = null;
            }
        );
        
        return enemy;
    }
}