using System.Collections.Generic;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    private int level;
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;
    private UIBoardView view;

    private PathfinderComponent pathfinder;
    
    public RectTransform GetAnchorRect()
    {
        throw new System.NotImplementedException();
    }
    
    public Vector3 GetAcnhorPosition()
    {
        throw new System.NotImplementedException();
    }

    public Camera RenderCamera => thisContext.Context.BoardDef.ViewCamera;

    public List<IResourceCarrier> Carriers { get; private set; }
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var key = new BoardPosition(position.X, position.Y);
        var def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;
        
        viewDef = thisContext.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);

        if (viewDef != null)
        {
            level = def.Level;
            storageItem = ProfileService.Current.GetStorageItem(GetResourceId());
            
            ResourcesViewManager.Instance.RegisterView(this);
        }
        
        pathfinder = thisContext.GetComponent<PathfinderComponent>(PathfinderComponent.ComponentGuid);
        Mask = def.Positions;
        
        base.OnAddToBoard(position, context);
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        ResourcesViewManager.Instance.UnRegisterView(this);
    }

    public override BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
    {
        return new BoardPosition(mask.X, mask.Y, position.Z);
    }

    public void Clear()
    {
        var key = new BoardPosition(realPosition.X, realPosition.Y);
        var def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;
        
        AddResourceView.Show(def.GetCenter(thisContext.Context), def.Reward);
        GameDataService.Current.FogsManager.RemoveFog(key);
        
        thisContext.Context.HintCooldown.RemoweView(view);
        
        if(def.Pieces != null)
        {
            foreach (var piece in def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    var pieceId = GameDataService.Current.MinesManager.GetMineById(piece.Key);

                    if (pieceId == PieceType.None.Id)
                    {
                        pieceId = PieceType.Parse(piece.Key);
                    }
                    
                    thisContext.Context.ActionExecutor.AddAction(new CreatePieceAtAction
                    {
                        At = pos,
                        PieceTypeId = pieceId
                    });
                }
            }
        }
        
        var weights = def.PieceWeights == null || def.PieceWeights.Count == 0
            ? GameDataService.Current.FogsManager.DefaultPieceWeights
            : def.PieceWeights;
        
        foreach (var point in Mask)
        {
            var piece = ItemWeight.GetRandomItem(weights).Piece;
            
            if(piece == PieceType.Empty.Id) continue;
            
            thisContext.Context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = piece
            });
        }
    }
    
    public void RegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
        if(/*pathfinder.CanPathToCastle(thisContext) == false || */storageItem.Amount < level) return;
        
        view = viewDef.AddView(ViewType.FogState);
        
        view.Priority = -1;
        view.Change(true);
        thisContext.Context.HintCooldown.AddView(view);
    }
    
    public string GetResourceId()
    {
        return Currency.Level.Name;
    }
    
    public Vector2 GetScreenPosition()
    {
        return thisContext.Context.BoardDef.GetSectorCenterWorldPosition(
            thisContext.CachedPosition.X,
            thisContext.CachedPosition.Y,
            thisContext.CachedPosition.Z);
    }
}