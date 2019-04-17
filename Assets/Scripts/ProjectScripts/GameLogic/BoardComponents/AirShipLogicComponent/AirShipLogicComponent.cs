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

        Vector3 pos = GetFreePlaceToSpawn();//BoardService.Current.FirstBoard.BoardDef.GetSectorCenterWorldPosition(20, 12, 1);
        view.PlaceTo(pos);
        view.AnimateIdle();

        return view;
    }

    private Vector2 GetFreePlaceToSpawn()
    {
        var boardDef = context.Context.BoardDef;
        
        Vector3 cameraWorldPos = boardDef.ViewCamera.transform.position;
        BoardPosition cellUnderCamera = boardDef.GetSectorPosition(cameraWorldPos);
        Vector3 cellUnderCameraPos = boardDef.GetSectorCenterWorldPosition(cellUnderCamera.X, cellUnderCamera.Y, BoardLayer.Piece.Layer);
        
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

        BoardPosition currentPos = BoardPosition.Zero();

        bool isFound = false;
        for (int distance = 1; distance < boardDef.Width; distance++)
        {
            int len = distance * 2 + 1;
            currentPos = cellUnderCamera.TopLeftAtDistance(distance);
            currentPos.X -= scanDirections[0].X;
            currentPos.Y -= scanDirections[0].Y;

            foreach (var scanDirection in scanDirections)
            {
                for (int i = 0; i < len; i++)
                {
                    currentPos.X += scanDirection.X;
                    currentPos.Y += scanDirection.X;

                    if (!currentPos.IsValid)
                    {
                        continue;
                    }

                    if (IsCellAvailable(currentPos))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (isFound)
                {
                    break;
                }
            }
        }
        
        return boardDef.GetSectorCenterWorldPosition(currentPos.X, currentPos.Y, BoardLayer.Piece.Layer);
    }

    private bool IsCellAvailable(BoardPosition pos)
    {
        BoardController board = context.Context; 
        BoardDefinitionComponent boardDef = board.BoardDef;

        if (!pos.IsValidFor(boardDef.Width, boardDef.Height))
        {
            return false;
        }
        
        // Do not place to Water or Fog
        Piece piece = board.BoardLogic.GetPieceAt(pos);
        if (piece != null && (piece.PieceType == PieceType.Empty.Id 
                           || piece.PieceType == PieceType.Fog.Id))
        {
            return false;
        }
        
        // Do not place at the same place as other AirShip
        foreach (var view in views)
        {
            var viewWorldPos = view.transform.position;
            BoardPosition boardPos = boardDef.GetSectorPosition(viewWorldPos);

            if (pos.Equals(boardPos))
            {
                return false;
            }
        }

        return true;
    }
}