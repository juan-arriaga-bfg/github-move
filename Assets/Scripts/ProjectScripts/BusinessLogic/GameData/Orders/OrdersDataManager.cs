using System.Collections.Generic;
using UnityEngine;

public class OrdersDataManager : IECSComponent, IDataManager, IDataLoader<List<OrderDef>>, ISequenceData
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public List<OrderDef> Recipes;
    public List<Order> Orders;
    
    private List<int> sequence;

    private int seed = -1;
    
    public List<RandomSaveItem> GetSaveSequences()
    {
        return new List<RandomSaveItem>{new RandomSaveItem{Uid = GetType().ToString(), Seed = seed, Count = sequence.Count}};
    }
    
    public void Reload()
    {
        Recipes = new List<OrderDef>();
        Orders = new List<Order>();
        sequence = new List<int>();
        LoadData(new ResourceConfigDataMapper<List<OrderDef>>("configs/orders.data", NSConfigsSettings.Instance.IsUseEncryption));
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
                
                InitSequence();
                
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

    private void InitSequence()
    {
        var save = ProfileService.Current.GetComponent<RandomSaveComponent>(RandomSaveComponent.ComponentGuid);
        var item = save?.GetSave(GetType().ToString());

        seed = item?.Seed ?? Random.Range(0, 1000000);
        sequence = ItemWeight.GetRandomSequence(GameDataService.Current.LevelsManager.Recipes, 100, seed);
        
        if(item == null) return;
        
        if (item.Count == 0)
        {
            UpdateSequence();
            return;
        }
        
        sequence.RemoveRange(0, sequence.Count - item.Count);
    }

    public void UpdateSequence()
    {
        seed++;
        sequence = ItemWeight.GetRandomSequence(GameDataService.Current.LevelsManager.Recipes, 100, seed);
    }

    public bool CheckStart()
    {
        return Orders.Count < GameDataService.Current.ConstantsManager.MaxOrders;
    }

    public Order GetOrder(int customer)
    {
        if (Orders.Count >= GameDataService.Current.ConstantsManager.MaxOrders) return null;
        
        if (sequence.Count == 0) UpdateSequence();
        if (sequence.Count == 0) return null;

        var recipe = Recipes[sequence[0]];
        var order = new Order{Customer = customer, Def = recipe};
        
        sequence.RemoveAt(0);
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