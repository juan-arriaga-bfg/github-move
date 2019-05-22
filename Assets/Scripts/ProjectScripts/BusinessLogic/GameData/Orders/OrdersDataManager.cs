using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class OrdersDataManager : ECSEntity, IDataManager, IDataLoader<List<OrderDef>>, ILockerComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private LockerComponent locker;
    public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
    
    private ECSEntity context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        locker = new LockerComponent();
        RegisterComponent(locker);
        
        Reload();
    }
    
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        
        context = null;
    }

    public List<OrderDef> Recipes;
    public List<Order> Orders;
    
    public void Reload()
    {
        Recipes = new List<OrderDef>();
        Orders = new List<Order>();
        LoadData(new HybridConfigDataMapper<List<OrderDef>>("configs/orders.data", NSConfigsSettings.Instance.IsUseEncryption));
        
        Locker.Lock(this);
    }

    public void LoadData(IDataMapper<List<OrderDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                var dataManager = (GameDataManager) context;
                
                var levels = dataManager.LevelsManager.Levels;
                var save = dataManager.UserProfile.GetComponent<OrdersSaveComponent>(OrdersSaveComponent.ComponentGuid);

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

                if (save?.Orders == null) return;

                foreach (var order in save.Orders)
                {
                    if (GetOrderDef(order.Id, out var orderDef))
                    {
                        Orders.Add(new Order{Customer = order.Customer, Def = orderDef});
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
    
    public bool CheckStart()
    {
        return Locker.IsLocked == false && Orders.Count < GameDataService.Current.ConstantsManager.MaxOrders;
    }

    public bool GetOrderDef(string recipeId, out OrderDef recipe)
    {
        recipe = null;
        
        recipe = Recipes.Find(def => def.Uid == recipeId);

        if (recipe == null) return false;

        return true;
    }

    public bool GetOrder(int customer, out Order order)
    {
        order = null;
        
        if (Orders.Count >= GameDataService.Current.ConstantsManager.MaxOrders) return false;

        ItemWeight item;
        OrderDef recipe = null;
        
        for (var i = 0; i < 5; i++)
        {
            item = GameDataService.Current.LevelsManager.GetSequence(Currency.Order.Name)?.GetNext();
            
            if (item == null) continue;
            
            recipe = Recipes.Find(def => def.Uid == item.Uid);

            if (recipe.IsUnlocked() == false)
            {
                recipe = null;
                continue;
            }
            
            break;
        }

        if (recipe == null) return true;
        
        order = new Order{Customer = customer, Def = recipe};
        
        Orders.Add(order);
        
        return false;
    }

    public void RemoveOrder(Order order, BoardLogicComponent logic)
    {
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.OrderCompleted, order);
        
        Orders.Remove(order);

        var customers = new List<CustomerComponent>();
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Character);
        
        foreach (var position in positions)
        {
            var customer = logic.GetPieceAt(position)?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
            
            if(customer == null || customer.Cooldown.IsExecuteable() || (customer.Order != null && customer.Order.State != OrderState.Reward)) continue;
            
            customers.Add(customer);
        }

        if (customers.Count == 0) return;
        
        var amount = Mathf.Min(customers.Count, GameDataService.Current.ConstantsManager.MaxOrders - Orders.Count);
        
        for (var i = 0; i < amount; i++)
        {
            var customer = customers[Random.Range(0, customers.Count)];

            customers.Remove(customer);
            customer.RestartCooldown();
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