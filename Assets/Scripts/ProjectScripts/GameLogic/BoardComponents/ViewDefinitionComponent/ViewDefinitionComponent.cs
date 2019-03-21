using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ViewDefinitionComponent : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public List<ViewType> ViewIds { get; set; }

    private int shownViewPriority;
    private BoardPosition Position;
    private readonly Dictionary<ViewType, UIBoardView> views = new Dictionary<ViewType, UIBoardView>();

    private Piece thisContext;
    
    private bool isDrag;
    private bool isSwap;

    private UIBoardView container;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
        ViewIds = new List<ViewType>();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
        OnRemoveFromBoard(Position, thisContext);
    }

    public bool Visible
    {
        get => container != null && container.GetCanvasGroup().alpha > 0;
        set
        {
            if (container != null) container.GetCanvasGroup().alpha = value ? 1 : 0;
        }
    }

    public void OnSwap(bool isEnd)
    {
        if(isSwap == !isEnd) return;

        isSwap = !isEnd;
        isDrag = false;
        
        foreach (var view in views.Values)
        {
            view.OnSwap(isEnd);
        }
    }

    public void OnDrag(bool isEnd)
    {
        if (isSwap) return;
        
        if(isDrag == !isEnd) return;

        isDrag = !isEnd;
        
        foreach (var view in views.Values)
        {
            view.OnDrag(isEnd);
        }
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        DOTween.Sequence()
            .SetId(this)
            .SetLoops(10)
            .AppendCallback(() =>
            {
                if (thisContext?.ActorView == null) return;
                
                DOTween.Kill(this);
                
                foreach (var pair in views)
                {
                    pair.Value.SetOffset(GetViewPosition(pair.Value.IsTop) + thisContext.ActorView.GetUIPosition(pair.Key));
                }
            })
            .AppendInterval(0.1f);
        
        if (thisContext == null) return;

        Position = thisContext.Multicellular is FogObserver observer ? observer.Def.GetCenter() : position;

        if (ViewIds == null) return;

        foreach (var id in ViewIds)
        {
            AddView(id);
        }
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        Position = to;
    }

    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        var f = from;
        var t = to;
                
        if (container != null)
        {
            f.Z = t.Z = BoardLayer.UI.Layer;
            thisContext.Context.RendererContext.MoveElement(f, t);
            container.CachedTransform.localPosition = thisContext.Context.BoardDef.GetPiecePosition(Position.X, Position.Y);
        }
        
        thisContext.Context.ActionExecutor.AddAction(new CallbackAction{Callback = controller =>
        {
            OnDrag(true);
        }});
    }
    
    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(context == null) return;

        var keys = views.Keys.ToList();
        
        foreach (var key in keys)
        {
            var view = views[key];
            
            thisContext.Context.RendererContext.DestroyElement(view);
            views.Remove(key);
        }

        if (container != null)
        {
            thisContext.Context.RendererContext.RemoveElement(container);
            container = null;
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
        if (views.TryGetValue(id, out var view))
        {
            return view;
        }
        
        var pos = Position;

        pos.Z = BoardLayer.UI.Layer;

        if (container == null)
        {
            container = thisContext.Context.RendererContext.CreateElementAt((int) ViewType.UIContainer, pos) as UIBoardView;
            container.GetCanvasGroup().alpha = 1f;
        }

        var element = thisContext.Context.RendererContext.CreateBoardElement<UIBoardView>((int)id);
        
        element.Init(thisContext);
        element.Init(thisContext.Context.RendererContext);
        element.CachedTransform.SetParentAndReset(container.CachedTransform);
        element.SyncRendererLayers(pos);
        element.SetOffset(thisContext?.ActorView != null ? GetViewPosition(element.IsTop) + thisContext.ActorView.GetUIPosition(id) : Vector2.zero);
        
        views.Add(id, element);
        
        return element;
    }
    
    public UIBoardView GetView(ViewType viewType)
    {
        return views.TryGetValue(viewType, out var targetView) ? targetView : null;
    }

    public List<UIBoardView> GetViews()
    {
        return views.Values.ToList();
    }
    
    public void RemoveView(ViewType id)
    {
        if (views.TryGetValue(id, out var view) == false) return;

        if (view.IsShow == false)
        {
            thisContext.Context.RendererContext.DestroyElement(view);
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

    private Vector2 GetViewPosition(bool isTop)
    {
        var position = BoardPosition.Zero();

        if (thisContext.Multicellular == null || thisContext.Multicellular is FogObserver)
            return thisContext.Context.BoardDef.GetSectorWorldPosition(position.X, position.Y, position.Z);
        
        var size = (int) Mathf.Sqrt(thisContext.Multicellular.Mask.Count + 1) - 1;
        
        if (isTop) position.Y += size;
        else position.X += size;
        
        return thisContext.Context.BoardDef.GetSectorWorldPosition(position.X, position.Y, position.Z);
    }
}