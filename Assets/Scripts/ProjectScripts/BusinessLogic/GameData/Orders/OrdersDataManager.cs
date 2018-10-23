﻿using System.Collections.Generic;
using UnityEngine;

public class OrdersDataManager : ECSEntity, IDataManager, IDataLoader<List<OrderDef>>, ISequenceData
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private SequenceComponent sequenceData;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public List<OrderDef> Recipes;
    public List<Order> Orders;
    
    public void Reload()
    {
        Recipes = new List<OrderDef>();
        Orders = new List<Order>();

        ReloadSequences();
        
        LoadData(new ResourceConfigDataMapper<List<OrderDef>>("configs/orders.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public List<RandomSaveItem> GetSaveSequences()
    {
        return new List<RandomSaveItem>{sequenceData.Save()};
    }

    public void ReloadSequences()
    {
        if(sequenceData == null) return;
        
        UnRegisterComponent(sequenceData);
    }

    public void LoadData(IDataMapper<List<OrderDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                var levels = GameDataService.Current.LevelsManager.Levels;
                var save = ProfileService.Current.GetComponent<OrdersSaveComponent>(OrdersSaveComponent.ComponentGuid);

                foreach (var level in levels)
                {
                    foreach (var weight in level.OrdersWeights)
                    {
                        var find = data.Find(def => def.Uid == weight.Uid);
                        
                        if(find == null || find.Level > 0) continue;

                        find.Level = level.Index;
                        
                        data.Remove(find);
                        Recipes.Add(find);
                    }
                }
                
                Recipes.Sort((a, b) => a.Level.CompareTo(b.Level));
                
                sequenceData = new SequenceComponent{Key = "Orders"};
                sequenceData.Init(GameDataService.Current.LevelsManager.Recipes);
                
                RegisterComponent(sequenceData);
                
                if(save?.Orders == null) return;

                foreach (var order in save.Orders)
                {
                    Orders.Add(new Order{Customer = order.Customer, Def = Recipes[order.Id]});
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public void UpdateSequence()
    {
        sequenceData.Reinit(GameDataService.Current.LevelsManager.Recipes);
    }

    public bool CheckStart()
    {
        return Orders.Count < GameDataService.Current.ConstantsManager.MaxOrders;
    }

    public Order GetOrder(int customer)
    {
        if (Orders.Count >= GameDataService.Current.ConstantsManager.MaxOrders) return null;

        var item = sequenceData.GetNext();
        
        if (item == null) return null;

        var recipe = Recipes.Find(def => def.Uid == item.Uid);
        var order = new Order{Customer = customer, Def = recipe};
        
        Orders.Add(order);
        
        return order;
    }

    public void RemoveOrder(Order order, BoardLogicComponent logic)
    {
        Orders.Remove(order);

        var customers = new List<CustomerComponent>();
        
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Character);

        foreach (var position in positions)
        {
            var customer = logic.GetPieceAt(position)?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
            
            if(customer == null || customer.Order != null || customer.Cooldown.IsExecuteable()) continue;
            
            customers.Add(customer);
        }
        
        if(customers.Count == 0) return;
        
        var amount = Mathf.Min(customers.Count, GameDataService.Current.ConstantsManager.MaxOrders - Orders.Count);
        
        for (var i = 0; i < amount; i++)
        {
            var customer = customers[Random.Range(0, customers.Count)];

            customers.Remove(customer);
            customer.Cooldown.Start();
        }
    }

    public void UpdateOrders()
    {
        foreach (var order in Orders)
        {
            order.Check();
        }
    }
}