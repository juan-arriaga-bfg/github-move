using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<ViewType, UIBoardView> views = new Dictionary<ViewType, UIBoardView>();

    private Piece thisContext;
    
    private const int Layer = 10;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
        ViewIds = new List<ViewType>();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if (thisContext == null) return;

        Position = position;

        if (ViewIds == null) return;

        foreach (var id in ViewIds)
        {
            AddView(id);
        }
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
        var f = from;
        var t = Position = to;

        foreach (var view in views.Values)
        {
            f.Z = t.Z += Layer + view.Layer;
            
            thisContext.Context.RendererContext.MoveElement(f, t);
            
            if(view.IsShow == false) continue;
            
            view.SetOfset();
        }
    }
    
    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(context == null) return;

        var keys = views.Keys.ToList();

        foreach (var key in keys)
        {
            var view = views[key];
            var currentPos = Position;
            
            currentPos.Z += Layer + view.Layer;
            thisContext.Context.RendererContext.RemoveElementAt(currentPos);
            views.Remove(key);
        }
    }

    public bool ShowView(int priority)
    {
        if (priority < shownViewPriority) return false;

        shownViewPriority = priority;

        foreach (var view in views.Values)
        {
            if (view.Priority >= shownViewPriority || !view.IsShow) continue;
            
            view.UpdateVisibility(false);
        }
        
        return true;
    }
    
    public UIBoardView AddView(ViewType id)
    {
        UIBoardView view;

        if (views.TryGetValue(id, out view))
        {
            return view;
        }
        
        var pos = Position;
        var currentPos = Position;
        
        pos.Z += Layer*10;
            
        var element = thisContext.Context.RendererContext.CreateElementAt((int)id, pos) as UIBoardView;
        element.Init(thisContext);

        currentPos.Z += Layer + element.Layer;
        
        thisContext.Context.RendererContext.MoveElement(pos, currentPos);
        
        views.Add(id, element);
        
        return element;
    }

    public List<UIBoardView> GetViews()
    {
        return views.Values.ToList();
    }
    
    public void RemoveView(ViewType id)
    {
        UIBoardView view;

        if (views.TryGetValue(id, out view) == false) return;

        if (view.IsShow == false)
        {
            var currentPos = Position;
            currentPos.Z += Layer + view.Layer;
        
            thisContext.Context.RendererContext.RemoveElementAt(currentPos);

            views.Remove(id);
        }
        
        shownViewPriority = -100;

        foreach (var element in views.Values)
        {
            if(element.Priority <= shownViewPriority || element.IsShow == false) continue;

            shownViewPriority = element.Priority;
        }
        
        foreach (var element in views.Values)
        {
            if(element.Priority != shownViewPriority || element.IsShow == false) continue;
            element.UpdateVisibility(true);
        }
    }
    
    public Vector3 GetViewPositionBottom(int size)
    {
        return thisContext.Context.BoardDef.GetSectorCenterWorldPosition(Position.X + (size - 1), Position.Y, Position.Z);
    }
    
    public Vector3 GetViewPositionTop(int size)
    {
        return thisContext.Context.BoardDef.GetSectorCenterWorldPosition(Position.X, Position.Y + (size - 1), Position.Z);
    }
}