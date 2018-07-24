using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionLogicComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
    private readonly List<ProductionComponent> productions = new List<ProductionComponent>();

    private List<ProductionComponent> select;
    private bool isDrag;
    
    public int Guid
    {
        get { return ComponentGuid; }
    }

    private BoardController context;
    
    public Action OnUpdate;
    
    public List<ProductionComponent> Productions
    {
        get { return select ?? productions; }
    }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Update()
    {
        if(OnUpdate == null) return;

        OnUpdate();
    }

    public void Check(int resource)
    {
        if(isDrag) return;

        isDrag = true;
        select = new List<ProductionComponent>();

        foreach (var production in productions)
        {
            if (production.Check(resource))
            {
                select.Add(production);
                continue;
            }
            
            if(production.IsShow() == false) continue;
            
            production.Hide();
        }

        UIService.Get.GetShowedView<UIProductionWindowView>(UIWindowType.ProductionWindow).Change(select.Count > 0);
    }

    public bool Hide(int resource, BoardPosition position)
    {
        if(!isDrag) return false;
        
        isDrag = false;
        
        foreach (var item in select)
        {
            item.Hide();
        }
        
        select = null;
        
        var piece = context.BoardLogic.GetPieceAt(position);
        
        if (piece == null) return false;
        
        var production = piece.GetComponent<ProductionComponent>(ProductionComponent.ComponentGuid);
        var isAdd = production != null && production.Add(resource);

        if (isAdd)
        {
            var animation = new BouncePieceAnimation {BoardElement = piece.ActorView};
            animation.Animate(context.RendererContext);
        }
        
        return isAdd;
    }

    public void Add(ProductionComponent item)
    {
        productions.Add(item);
    }

    public void Remove(ProductionComponent item)
    {
        productions.Remove(item);
    }
}