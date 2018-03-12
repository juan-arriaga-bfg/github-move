using System.Collections.Generic;
using UnityEngine;

public class EnemiesDataManager : IDataLoader<List<EnemyDef>>
{
    public List<EnemyDef> Enemies;
    
    public void LoadData(IDataMapper<List<EnemyDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                Enemies = data;
            }
            else
            {
                Debug.LogWarning("[EnemiesDataManager]: enemies config not loaded");
            }
        });
    }
    
    public int EnemyIndex { get; set; }
    
    public EnemyDef GetEnemy(int index)
    {
        EnemyDef enemy;
        var max = Enemies.Count - 1;
        
        if (index > max)
        {
            var last = Enemies[max];
            var factor = index - max;
            
            enemy = new EnemyDef
            {
                Skin = string.Format("E{0}", Random.Range(0, 4)),
                HP = last.HP + 30 * factor,
                Price = new CurrencyPair{Currency = last.Price.Currency, Amount = last.Price.Amount + 10 * factor},
//                Chest = Chests[Random.Range(0, Chests.Count)].Uid
            };
        }
        else
        {
            enemy = Enemies[index];
        }
        
        return enemy;
    }

    public EnemyDef GetCurrentEnemy()
    {
        var enemy = GetEnemy(EnemyIndex);

        return enemy;
    }

    public bool IsCanEnterBattle()
    {
        var enemy = GetEnemy(EnemyIndex);

        bool isCanPurchase = ShopService.Current.IsCanPurchase(new List<Price>
        {
            new Price {Currency = enemy.Price.Currency, DefaultPriceAmount = enemy.Price.Amount}
        });

        return isCanPurchase;
    }

    public EnemyDef FightEnemy()
    {
        var enemy = GetEnemy(EnemyIndex);
        
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
                EnemyIndex++;
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