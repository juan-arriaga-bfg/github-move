using System;
using System.Collections.Generic;
using UnityEngine;

public interface IVIPIslandLogicComponent
{
    VIPIslandLogicComponent VIPIslandLogic { get; }
}

public partial class BoardLogicComponent : IVIPIslandLogicComponent
{
    protected VIPIslandLogicComponent vipIslandLogic;
    public VIPIslandLogicComponent VIPIslandLogic => vipIslandLogic ?? (vipIslandLogic = GetComponent<VIPIslandLogicComponent>(VIPIslandLogicComponent.ComponentGuid));
}

public enum VIPIslandState
{
    Fog,
    Broken,
    Paid
}

public class VIPIslandLogicComponent : ECSEntity, ITouchableBoardObjectLogic
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private BoardLogicComponent context;

    public bool IsDraggable => false;
    
    private readonly Dictionary<ViewType, BoardElementView> views = new Dictionary<ViewType, BoardElementView>();
    
    private readonly BoardPosition boardPosition = new BoardPosition(10, 13, BoardLayer.Piece.Layer);
    private Vector3 localPosition;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardLogicComponent;
        
        localPosition = context.Context.BoardDef.GetPiecePosition(boardPosition.X, boardPosition.Y);

        CreateView(ViewType.Coast, false);
        UpdateView(VIPIslandState.Fog, false);
    }
    
    public bool OnDragStart(BoardElementView view)
    {
        return Check(view);
    }

    public bool OnDragEnd(BoardElementView view)
    {
        return Check(view);
    }

    public bool Check(BoardElementView view)
    {
        foreach (var item in views.Values)
        {
            if (item != view) continue;
        
            return true;
        }
        
        return false;
    }

    public bool OnClick(BoardElementView view)
    {
        return Check(view);
    }

    private void UpdateView(VIPIslandState state, bool animation)
    {
        switch (state)
        {
            case VIPIslandState.Fog:
                CreateView(ViewType.IslandFog, false);
                break;
            case VIPIslandState.Broken:
                CreateView(ViewType.BrokenBridge, animation);
                CreateView(ViewType.Airbaloon, animation);

                if (animation == false) break;
                
                (views[ViewType.IslandFog] as AnimatedBoardElementView).PlayHide();
                
                break;
            case VIPIslandState.Paid:
                CreateView(ViewType.Bridge, animation);
                
                if (animation == false) break;
                
                (views[ViewType.BrokenBridge] as AnimatedBoardElementView).PlayHide();
                (views[ViewType.Airbaloon] as AnimatedBoardElementView).PlayHide();
                
                break;
        }
    }

    private void CreateView(ViewType id, bool animation)
    {
        var view = context.Context.RendererContext.CreateBoardElement<BoardElementView>((int)id);
        var animView = view as AnimatedBoardElementView;
        
        view.Init(context.Context.RendererContext);
        view.CachedTransform.localPosition = localPosition;
        view.SyncRendererLayers(boardPosition);

        if (animation) animView.PlayShow();
        else if (animView != null) animView.PlayIdle();
        
        views.Add(id, view);
    }
}
