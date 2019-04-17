using System;
using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AirShipLogicComponent : ECSEntity, IDraggableFlyingObjectLogic
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private BoardLogicComponent context;

	private readonly List<AirShipView> views = new List<AirShipView>();
    public List<AirShipView> Views => views;

	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
	}

	public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
	
	public bool OnDragStart(BoardElementView view)
	{
		var airShip = view as AirShipView;

		if (airShip == null) return false;
		
		airShip.OnDragStart();
		return true;
	}

	public bool OnDragEnd(BoardElementView view)
	{
		var airShip = view as AirShipView;

		if (airShip == null) return false;
		
		airShip.OnDragEnd();
		return true;
	}
	
    public bool Check(BoardElementView view)
    {
        return view is AirShipView;
    }

	public bool OnClick(BoardElementView view)
	{
		var airShip = view as AirShipView;
		
		if (airShip == null) return false;

        airShip.OnClick();
        
		return true;
	}

	public void Remove(AirShipView view)
	{
		views.Remove(view);
	}


    public AirShipView Add(Dictionary<int, int> pieces)
    {
        AirShipView view = context.Context.RendererContext.CreateBoardElement<AirShipView>((int) ViewType.AirShip);
        view.Init(context.Context.RendererContext, pieces);
        
        views.Add(view);
        
        var pos = BoardService.Current.FirstBoard.BoardDef.GetSectorCenterWorldPosition(20, 12, 1);
        view.PlaceTo(pos);
        view.AnimateIdle();

        return view;
    }
}