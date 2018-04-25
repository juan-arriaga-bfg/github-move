using System.Collections.Generic;
using UnityEngine;

public class ViewDefinitionComponent : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    public List<ViewType> ViewIds { get; set; }

    private int shownViewPriority;
    private BoardPosition Position;
    private List<UIBoardView> views = new List<UIBoardView>();

    private Piece context;
    
    private const int Layer = 3;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(context == null || ViewIds == null) return;

        var pos = Position = position;
        
        for (var i = 0; i < ViewIds.Count; i++)
        {
            pos.Z = position.Z + (i + Layer + 1);
            
            var element = context.Context.RendererContext.CreateElementAt((int)ViewIds[i], pos) as UIBoardView;
            element.Init(context);
            views.Add(element);
        }
    }
    
    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
        if(context == null || ViewIds == null) return;
        
        var f = from;
        var t = Position = to;
        
        for (var i = 0; i < ViewIds.Count; i++)
        {
            f.Z = t.Z = from.Z + (i + Layer + 1);
            
            context.Context.RendererContext.MoveElement(f, t);
        }
    }
    
    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(context == null || ViewIds == null) return;
        
        var pos = position;
        
        for (var i = 0; i < ViewIds.Count; i++)
        {
            pos.Z = position.Z + (i + Layer + 1);
            var element = context.Context.RendererContext.RemoveElementAt(pos) as UIBoardView;
            views.Remove(element);
        }
    }

    public bool ShowView(int priority)
    {
        if (priority < shownViewPriority) return false;

        shownViewPriority = priority;

        foreach (var view in views)
        {
            if (view.Priority >= shownViewPriority || !view.IsShow) continue;
            
            view.UpdateVisibility(false);
        }
        
        return true;
    }
    
    public void HideView()
    {
        var shown = views.FindAll(view => view.IsShow);
        
        if(shown.Count == 0) return;
        
        shown.Sort((a, b) => -a.Priority.CompareTo(b.Priority));
        shownViewPriority = shown[0].Priority;
        
        foreach (var view in shown)
        {
            if(view.Priority != shownViewPriority || view.IsShow == false) continue;
            view.UpdateVisibility(true);
        }
    }

    public Vector3 GetViewPositionBottom(int size)
    {
        return context.Context.BoardDef.GetSectorWorldPosition(Position.X + size - 1, Position.Y + (size == 0 ? 2 : 1), Position.Z);
    }
    
    public Vector3 GetViewPositionTop(int size)
    {
        return context.Context.BoardDef.GetSectorWorldPosition(Position.X, Position.Y + size, Position.Z);
    }
}