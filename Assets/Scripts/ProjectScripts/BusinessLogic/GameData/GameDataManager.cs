﻿using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    public List<ChestDef> Chests;
    public List<EnemyDef> Enemies;
    public List<HeroDef> Heroes;
    public Dictionary<int, PieceDef> Pieces = new Dictionary<int, PieceDef>();
    
    private readonly List<ChestDef> activeChests = new List<ChestDef>();
    
    public int EnemyIndex { get; set; }
    
    public int HeroLevel
    {
        get
        {
            var hero = GetHero("Robin");
            var level = Mathf.Clamp(ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount, 0, hero.Damages.Count - 1);
            
            return level;
        }
    }
    
    public bool AddActiveChest(ChestDef chest)
    {
        if (activeChests.Count == 4) return false;

        chest.State = ChestState.Lock;
        activeChests.Add(chest);
        
        return true;
    }

    public void RemoveActiveChest(ChestDef chest)
    {
        var index = activeChests.IndexOf(chest);
        
        if(index == -1) return;
        
        activeChests.RemoveAt(index);
    }
    
    public List<ChestDef> GetActiveChests()
    {
        return activeChests;
    }

    public HeroDef GetHero(string uid)
    {
        return Heroes.Find(def => def.Uid == uid);
    }

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
                HP = last.HP + 50 * factor,
                Price = new CurrencyPair{Currency = last.Price.Currency, Amount = last.Price.Amount + 10 * factor},
                Chest = Chests[Random.Range(0, Chests.Count)].Uid
            };
        }
        else
        {
            enemy = Enemies[index];
        }
        
        return enemy;
    }
    
    public EnemyDef GetEnemy()
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