using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    private int level;
    private FogDef def;
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;
    private LockView lockView;
    private BubbleView view;
    private BoardPosition key;
    
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
        key = new BoardPosition(position.X, position.Y);
        
        def = GameDataService.Current.FogsManager.GetDef(key);
        
        if(def == null) return;
        
        Mask = def.Positions;
        viewDef = thisContext.ViewDefinition;

        if (viewDef != null)
        {
            viewDef.OnAddToBoard(position, context);
            level = def.Level;
            storageItem = ProfileService.Current.GetStorageItem(GetResourceId());
            ResourcesViewManager.Instance.RegisterView(this);
        }
        
        base.OnAddToBoard(position, context);
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        ResourcesViewManager.Instance.UnRegisterView(this);
        
        var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

        if (views == null) return;
        
        foreach (var view in views)
        {
            view.UpdateResource(0);
        }
    }

    public override BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
    {
        return new BoardPosition(mask.X, mask.Y, position.Z);
    }

    public void Clear()
    {
        if(def == null) return;
        
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ClearFog, null);
        
        AddResourceView.Show(def.GetCenter(thisContext.Context), def.Reward);
        GameDataService.Current.FogsManager.RemoveFog(key);
        
        thisContext.Context.HintCooldown.RemoweView(view);

        List<CreatePieceAtAction> actions = new List<CreatePieceAtAction>();
        var addedPieces = new Dictionary<BoardPosition, int>();
        
        if(def.Pieces != null)
        {
            foreach (var piece in def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    var pieceId = GameDataService.Current.MinesManager.GetMineTypeById(piece.Key);

                    if (pieceId == PieceType.None.Id)
                    {
                        pieceId = PieceType.Parse(piece.Key);
                    }
                    
                    addedPieces.Add(new BoardPosition(pos.X, pos.Y, thisContext.Layer.Index), pieceId);
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
            var position = new BoardPosition(point.X, point.Y, thisContext.Layer.Index);
            if(addedPieces.ContainsKey(position) == false)
                addedPieces.Add(position, piece);
        }
        
        thisContext.Context.ActionExecutor.AddAction(new CreateGroupPieces()
        {
            Pieces = addedPieces,
            OnSuccessEvent = () =>
            {
                var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

                if (views == null) return;
        
                foreach (var view in views)
                {
                    view.UpdateResource(0);
                }
            }
        });

    }

    public void RegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
//      var canPath = thisContext.Context.Pathfinder.CanPathToCastle(thisContext);
        var canPath = thisContext.Context.PathfindLocker.HasPath(thisContext);
        
        
        var levelAccess = storageItem.Amount >= level;

        if ((canPath ^ levelAccess) && lockView == null)
        {
            lockView = viewDef.AddView(ViewType.Lock) as LockView;
            lockView.Value = level.ToString();
            lockView.transform.position = def.GetCenter(thisContext.Context);
        }

        lockView?.SetGrayscale(!canPath);
        
        
        if(canPath == false || storageItem.Amount < level) return;

        if (lockView != null)
        {
            lockView.Change(false);
            viewDef.RemoveView(ViewType.Lock);
            lockView = null;
        }
        
        view = viewDef.AddView(ViewType.Bubble) as BubbleView;
        
        if(view.IsShow) return;
        
        view.SetData("Clear fog", def.Condition.ToStringIcon(), OnClick);
        view.SetOfset(def.GetCenter(thisContext.Context) + new Vector3(0, 0.1f));
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

    public bool CanBeCleared()
    {
        var pathExists = thisContext.Context.Pathfinder.CanPathToCastle(thisContext);
        var resourcesEnought = storageItem.Amount >= level;

        return pathExists && resourcesEnought;
    }
    
    private void OnClick(Piece piece)
    {
        if(CurrencyHellper.IsCanPurchase(def.Condition, true, () => OnClick(piece)) == false) return;
        
        CurrencyHellper.Purchase(Currency.Fog.Name, 1, def.Condition, success =>
        {
            if (success == false) return;
			
            piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = piece.CachedPosition,
                Positions = new List<BoardPosition>{piece.CachedPosition}
            });
        });
    }
}