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
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Check(int resource)
    {
        if(isDrag) return;

        isDrag = true;
        select = productions.FindAll(production => production.Check(resource));
    }

    public bool Hide(int resource, BoardPosition position)
    {
        if(!isDrag) return false;
        
        isDrag = false;
        
        foreach (var item in select)
        {
            item.Hide();
        }

        var piece = context.BoardLogic.GetPieceAt(position);

        if (piece == null) return false;
        
        var production = piece.GetComponent<ProductionComponent>(ProductionComponent.ComponentGuid);
        var isAdd = production != null && production.Add(resource);

        if (isAdd)
        {
            context.RendererContext.AddAnimationToQueue(new BouncePieceAnimation
            {
                BoardElement = piece.ActorView
            });
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