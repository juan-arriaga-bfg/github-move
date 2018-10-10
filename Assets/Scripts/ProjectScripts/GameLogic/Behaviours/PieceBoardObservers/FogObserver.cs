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
    private UIBoardView view;
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
        
        AddResourceView.Show(def.GetCenter(thisContext.Context), def.Reward);
        GameDataService.Current.FogsManager.RemoveFog(key);
        
        thisContext.Context.HintCooldown.RemoweView(view);

        List<CreatePieceAtAction> actions = new List<CreatePieceAtAction>();
        List<BoardPosition> addedPieces = new List<BoardPosition>();
        
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
                    
                    addedPieces.Add(new BoardPosition(pos.X, pos.Y, thisContext.Layer.Index));
                    var act = new CreatePieceAtAction
                    {
                        At = pos,
                        PieceTypeId = pieceId
                    };
                    thisContext.Context.ActionExecutor.AddAction(act);
                    actions.Add(act);
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
            
            addedPieces.Add(new BoardPosition(point.X, point.Y, thisContext.Layer.Index));
            var act = new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = piece
            };
            thisContext.Context.ActionExecutor.AddAction(act);
            actions.Add(act);
        }
        
        GeneratePathfindRecalc(actions);
    }

    private void GeneratePathfindRecalc(List<CreatePieceAtAction> actions)
    {
        actions.First().OnComplete = () =>
        {
            //thisContext.Context.PathfindLocker.RecalcAll(thisContext.Context.AreaAccessController.AvailiablePositions);
            foreach (var act in actions)
            {
                
                var pos = act.At;
                var piece = thisContext.Context.BoardLogic.GetPieceAt(pos);
                
                piece.PathfindLockObserver.OnAddToBoard(pos);
            }

            Debug.LogError("Callback Execute");
        };
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
        
        view = viewDef.AddView(ViewType.FogState);
        
        if(view.IsShow) return;
        
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
}