﻿using System;
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

    private bool isClick;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardLogicComponent;
    }

    public void Init()
    {
        localPosition = context.Context.BoardDef.GetPiecePosition(boardPosition.X, boardPosition.Y);

        CreateView(ViewType.Coast, boardPosition);
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
        var isCheck = Check(view);

        if (isCheck && isClick) return isCheck;

        isClick = true;

        UIMessageWindowController.CreateMessage("aaaaaaaaa", "oooooooooooooo", null, () => UpdateView(VIPIslandState.Paid, true));
        
        return isCheck;
    }

    public void UpdateView(VIPIslandState state, bool animation)
    {
        switch (state)
        {
            case VIPIslandState.Fog:
                CreateView(ViewType.FogBridge, boardPosition.Down);
                CreateView(ViewType.IslandFog, boardPosition);
                break;
            case VIPIslandState.Broken:
                RemoveView(ViewType.FogBridge);
                CreateView(ViewType.BrokenBridge, boardPosition);
                CreateView(ViewType.Airbaloon, boardPosition, animation);

                if (animation == false) break;
                
                (views[ViewType.IslandFog] as AnimatedBoardElementView).PlayHide();
                
                break;
            case VIPIslandState.Paid:
                RemoveView(ViewType.BrokenBridge);
                CreateView(ViewType.Bridge, boardPosition, animation);
                
                if (animation == false) break;
                
                (views[ViewType.Airbaloon] as AnimatedBoardElementView).PlayHide(2f);
                
                break;
        }
    }

    private void CreateView(ViewType id, BoardPosition position, bool animation = false)
    {
        var view = context.Context.RendererContext.CreateBoardElement<BoardElementView>((int)id);
        var animView = view as AnimatedBoardElementView;
        
        view.CachedTransform.localPosition = localPosition;
        view.Init(context.Context.RendererContext);
        view.SyncRendererLayers(position);

        if (animation) animView.PlayShow();
        else if (animView != null) animView.PlayIdle();
        
        views.Add(id, view);
    }

    private void RemoveView(ViewType id)
    {
        if (views.TryGetValue(id, out var view) == false) return;

        views.Remove(id);
        context.Context.RendererContext.DestroyElement(view);
    }
}
