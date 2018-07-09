using System.Collections.Generic;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    private CurrencyPair condition;
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;
    
    public Camera RenderCamera
    {
        get { return thisContext.Context.BoardDef.ViewCamera; }
    }
    
    public List<ResourceCarrier> Carriers { get; private set; }
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var key = new BoardPosition(position.X, position.Y);
        var def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;
        
        viewDef = context.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);

        if (viewDef != null)
        {
            condition = def.Condition;
            storageItem = ProfileService.Current.GetStorageItem(GetResourceId());
            
            ResourcesViewManager.Instance.RegisterView(this);
        }
        
        Mask = BoardPosition.GetRect(BoardPosition.Zero(), def.Size.X, def.Size.Y);
        
        base.OnAddToBoard(position, context);
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        ResourcesViewManager.Instance.UnRegisterView(this);
        
        var key = new BoardPosition(position.X, position.Y);
        var def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;
        
        GameDataService.Current.FogsManager.RemoveFog(key);
        
        if(def.Pieces != null)
        {
            foreach (var piece in def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
                    {
                        At = GetPointInMask(realPosition, pos),
                        PieceTypeId = PieceType.Parse(piece.Key)
                    });
                }
            }
        }

        var weights = def.PieceWeights == null || def.PieceWeights.Count == 0
            ? GameDataService.Current.FogsManager.DefaultPieceWeights
            : def.PieceWeights;
        
        for (int i = 0; i < Mask.Count; i++)
        {
            var point = GetPointInMask(realPosition, Mask[i]);
            var piece = ItemWeight.GetRandomItem(weights).Piece;
            
            if(piece == PieceType.Empty.Id) continue;
            
            context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = piece
            });
        }
    }
    
    public void RegisterCarrier(ResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(ResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
        if(storageItem.Amount < condition.Amount) return;
        
        var view = viewDef.AddView(ViewType.FogState);

        view.Priority = -1;
        view.Change(true);
    }
    
    public string GetResourceId()
    {
        return condition.Currency;
    }
    
    public Vector2 GetScreenPosition()
    {
        return thisContext.Context.BoardDef.GetSectorCenterWorldPosition(
            thisContext.CachedPosition.X,
            thisContext.CachedPosition.Y,
            thisContext.CachedPosition.Z);
    }
}