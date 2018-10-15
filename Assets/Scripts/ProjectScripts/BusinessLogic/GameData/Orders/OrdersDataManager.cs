using System.Collections.Generic;
using UnityEngine;

public class OrdersDataManager : IECSComponent, IDataManager, IDataLoader<List<OrderDef>>
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
    private List<int> ids;
    
    private List<int> sequence = new List<int>();
    
    public void Reload()
    {
        Recipes = new List<OrderDef>();
        Orders = new List<Order>();
        LoadData(new ResourceConfigDataMapper<List<OrderDef>>("configs/orders.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<OrderDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                ids = PieceType.GetIdsByFilter(PieceTypeFilter.Character);
                var levels = GameDataService.Current.LevelsManager.Levels;

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
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public void UpdateSequence()
    {
        sequence = ItemWeight.GetRandomSequence(GameDataService.Current.LevelsManager.Recipes, 100);
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

        foreach (var id in ids)
        {
            var cash = logic.PositionsCache.GetPiecePositionsByType(id);
            
            if(cash.Count == 0) continue;
            
            var customer = logic.GetPieceAt(cash[0])?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
            
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