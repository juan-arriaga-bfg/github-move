using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AirShipLogicComponent : ECSEntity, IDraggableFlyingObjectLogic
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private BoardLogicComponent context;

	private readonly Dictionary<int, AirShipDef> defs = new Dictionary<int, AirShipDef>();

    private int idCounter = 0;
    
	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
        InitFromSave();
        UIService.Get.OnCloseWindowEvent += OnCloseWindow;
    }

    private void InitFromSave()
    {
        var save = ProfileService.Current.GetComponent<AirShipSaveComponent>(AirShipSaveComponent.ComponentGuid);
        if (save?.Items == null || save.Items.Count == 0)
        {
            return;
        }

        foreach (var item in save.Items)
        {
            Add(item.Payload, item.Position).AnimateIdle();
        }
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
        UIService.Get.OnCloseWindowEvent -= OnCloseWindow;
    }
    
    private void OnCloseWindow(IWUIWindow window)
    {
        foreach (var showedWindow in UIService.Get.ShowedWindows)
        {
            if (UIWindowType.IsIgnore(showedWindow.WindowName)) continue;
            
            return;
        }
        
        foreach (var ship in defs.Values)
        {
            if(ship.View.isShow) continue;
            
            ship.View.AnimateSpawn();
        }
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
	
    public List<AirShipSaveItem> Save()
    {
        var ret = new List<AirShipSaveItem>();
        foreach (var pair in defs)
        {
            var def = pair.Value;
            AirShipSaveItem item = new AirShipSaveItem
            {
                Position = def.View.transform.position,
                Payload = def.Payload
            };
            ret.Add(item);
        }

        return ret;
    }
    
    public AirShipView Add(Dictionary<int, int> payload, Vector2? position = null)
    {
        Vector3 pos = position ?? GetFreePlaceToSpawn();

        idCounter++;
        
        var view = context.Context.RendererContext.CreateBoardElement<AirShipView>((int) ViewType.AirShip);
        var airShipDef = new AirShipDef {Id = idCounter, View = view, Payload = payload};
        
        defs.Add(idCounter, airShipDef);

        view.Init(context.Context.RendererContext, airShipDef);
        view.PlaceTo(pos);
        
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
        foreach (var def in defs.Values)
        {
            var viewWorldPos = def.View.transform.position;
            viewWorldPos.z = 0;//cam.transform.position.z * -1;
            
            BoardPosition boardPos = boardDef.GetSectorPosition(viewWorldPos);
            
            if (fixedPos.X == boardPos.X && fixedPos.Y == boardPos.Y)
            {
                return false;
            }
        }

        return true;
    }

    public bool DropPayload(int id, out bool partialDrop)
    {
        partialDrop = false;
        
        var airShipDef = defs[id];
        var view = airShipDef.View;
        
        var payload = new Dictionary<int, int>(airShipDef.Payload);
        var amountAll = payload.Sum(pair => pair.Value);
        var pieces = new Dictionary<BoardPosition, int>();
        
        var boardPos = context.Context.BoardDef.GetSectorPosition(view.CachedTransform.position);
        boardPos.Z = BoardLayer.Piece.Layer;
        
        var free = new List<BoardPosition>();
        
        if (context.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(boardPos, free, amountAll, 0.1f) == false) return false;
        
        foreach (var key in airShipDef.SortedPayload)
        {
            if (free.Count == 0) break;
            if (payload.TryGetValue(key, out var amount) == false) continue;

            for (; amount > 0; amount--)
            {
                if (free.Count == 0) break;
                
                pieces.Add(free[0], key);
                free.RemoveAt(0);
            }

            if (amount == 0) payload.Remove(key);
            else payload[key] = amount;
        }
        
        var action = new AirShipEjectionPieceAction
        {
            From = view.CachedTransform.position,
            BoardElement = view,
            Pieces = pieces,
        };
        
        context.Context.ActionExecutor.AddAction(action);
        
        airShipDef.Payload = payload;
        view.UpdatePayload(airShipDef);
        
        if (payload.Count == 0)
        {
            defs.Remove(id);
            view.AnimateDeath();
            return true;
        }
        
        partialDrop = true;
        return false;
    }

    public bool CheckPayload(int piece)
    {
        foreach (var ship in defs.Values)
        {
            foreach (var key in ship.Payload.Keys)
            {
                if(key != piece) continue;

                return true;
            }
        }

        return false;
    }
}