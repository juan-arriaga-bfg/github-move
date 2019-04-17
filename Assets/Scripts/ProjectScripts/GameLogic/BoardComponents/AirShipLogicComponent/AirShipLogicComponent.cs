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


    public AirShipView Add(Dictionary<int, int> pieces, bool animated)
    {
        Vector3 pos = GetFreePlaceToSpawn();
        
        AirShipView view = context.Context.RendererContext.CreateBoardElement<AirShipView>((int) ViewType.AirShip);
        view.Init(context.Context.RendererContext, pieces);
        
        views.Add(view);

        view.PlaceTo(pos);

        if (animated)
        {            
            view.AnimateSpawn(); 
        }
        else
        {
            view.AnimateIdle();
        }

        return view;
    }

    private Vector2 GetFreePlaceToSpawn()
    {
        var boardDef = context.Context.BoardDef;

        var cam = context.Context.BoardDef.ViewCamera;
        
        Vector3 screenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, cam.transform.position.z * -1);
        var targetPosition = cam.ScreenToWorldPoint(screenPosition);

        BoardPosition cellUnderCamera = boardDef.GetSectorPosition(targetPosition);
        Vector3 cellUnderCameraPos = boardDef.GetSectorCenterWorldPosition(cellUnderCamera.X, cellUnderCamera.Y, 0);
        
        // var cell1 = context.Context.RendererContext.CreateBoardElementAt<DebugCellView>(R.DebugCell, new BoardPosition(cellUnderCamera.X, cellUnderCamera.Y, BoardLayer.MAX.Layer));
        // cell1.SetText("(!)");
        
        if (IsCellAvailable(cellUnderCamera))
        {
            return cellUnderCameraPos;
        }

        BoardPosition[] scanDirections = {
            new BoardPosition(1,  0),
            new BoardPosition(0,  -1),
            new BoardPosition(-1, 0),
            new BoardPosition(0,  1),
        };

        BoardPosition currentPos = new BoardPosition(0, 0, 0);

        for (int distance = 1; distance <= boardDef.Width; distance++)
        {
            int len = distance * 2 + 1;
            currentPos = cellUnderCamera.TopLeftAtDistance(distance);

            int counter = 0;
            foreach (var scanDirection in scanDirections)
            {
                for (int i = 0; i < len - 1; i++)
                {
                    currentPos.X += scanDirection.X;
                    currentPos.Y += scanDirection.Y;

                    // var cell = context.Context.RendererContext.CreateBoardElementAt<DebugCellView>(R.DebugCell, new BoardPosition(currentPos.X, currentPos.Y, BoardLayer.MAX.Layer));
                    // cell.SetText($"{distance}:{counter++}");

                    if (IsCellAvailable(currentPos))
                    {
                        return boardDef.GetSectorCenterWorldPosition(currentPos.X, currentPos.Y, 0);
                    }
                }
            }
        }
        
        // fallback
        return cellUnderCameraPos;
    }

    private bool IsCellAvailable(BoardPosition pos)
    {
        int pieceLayer = BoardLayer.Piece.Layer;
        var fixedPos = pos;
        fixedPos.Z = pieceLayer;
        // var cam = context.Context.BoardDef.ViewCamera;
        
        BoardController board = context.Context; 
        BoardDefinitionComponent boardDef = board.BoardDef;

        if (!fixedPos.IsValidFor(boardDef.Width, boardDef.Height))
        {
            return false;
        }
        
        // Do not place to Water or Fog
        Piece piece = board.BoardLogic.GetPieceAt(fixedPos);
        if (piece != null && (piece.PieceType == PieceType.Empty.Id 
                           || piece.PieceType == PieceType.Fog.Id))
        {
            return false;
        }

        // Do not place at the same place as other AirShip
        foreach (var view in views)
        {
            var viewWorldPos = view.transform.position;
            viewWorldPos.z = 0;//cam.transform.position.z * -1;
            
            BoardPosition boardPos = boardDef.GetSectorPosition(viewWorldPos);
            
            if (fixedPos.X == boardPos.X && fixedPos.Y == boardPos.Y)
            {
                return false;
            }
        }

        return true;
    }
}