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
    private List<KeyValuePair<ViewType, UIBoardView>> views = new List<KeyValuePair<ViewType, UIBoardView>>();

    private Piece thisContext;
    
    private const int Layer = 3;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(thisContext == null) return;
        
        Position = position;
        
        if(ViewIds == null) return;
        
        foreach (var id in ViewIds)
        {
            AddView(id);
        }
    }
    
    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
        if(context == null) return;
        
        var f = from;
        var t = Position = to;
        
        for (var i = 0; i < views.Count; i++)
        {
            f.Z = t.Z = from.Z + (i + Layer + 1);
            
            context.Context.RendererContext.MoveElement(f, t);
        }
    }
    
    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(context == null) return;

        for (var i = views.Count - 1; i >= 0; i--)
        {
            var pair = views[i];
            RemoveView(pair.Key);
        }
    }

    public bool ShowView(int priority)
    {
        if (priority < shownViewPriority) return false;

        shownViewPriority = priority;

        foreach (var view in views)
        {
            if (view.Value.Priority >= shownViewPriority || !view.Value.IsShow) continue;
            
            view.Value.UpdateVisibility(false);
        }
        
        return true;
    }
    
    public void HideView()
    {
        ///TODO: нахера двойная проверка на IsShow?
        
        var shown = views.FindAll(pair => pair.Value.IsShow);
        
        if(shown.Count == 0) return;
        
        shown.Sort((a, b) => -a.Value.Priority.CompareTo(b.Value.Priority));
        shownViewPriority = shown[0].Value.Priority;
        
        foreach (var view in shown)
        {
            if(view.Value.Priority != shownViewPriority || view.Value.IsShow == false) continue;
            view.Value.UpdateVisibility(true);
        }
    }

    public UIBoardView AddView(ViewType id)
    {
        var index = views.FindIndex(pair => pair.Key == id);
        
        if(index != -1) return views[index].Value;
        
        var pos = Position;
        pos.Z += views.Count + Layer + 1;
            
        var element = thisContext.Context.RendererContext.CreateElementAt((int)id, pos) as UIBoardView;
        element.Init(thisContext);
        
        views.Add(new KeyValuePair<ViewType, UIBoardView>(id, element));
        views.Sort((a, b) => -a.Value.Priority.CompareTo(b.Value.Priority));
        
        return element;
    }

    public void RemoveView(ViewType id)
    {
        var index = views.FindIndex(pair => pair.Key == id);
        
        if(index == -1) return;
        
        var pos = Position;
        
        pos.Z += index + Layer + 1;
        thisContext.Context.RendererContext.RemoveElementAt(pos);
        views.RemoveAt(index);
    }

    public Vector3 GetViewPositionBottom(int size)
    {
        return thisContext.Context.BoardDef.GetSectorWorldPosition(Position.X + size - 1, Position.Y + (size == 0 ? 2 : 1), Position.Z);
    }
    
    public Vector3 GetViewPositionTop(int size)
    {
        return thisContext.Context.BoardDef.GetSectorWorldPosition(Position.X, Position.Y + size, Position.Z);
    }
}