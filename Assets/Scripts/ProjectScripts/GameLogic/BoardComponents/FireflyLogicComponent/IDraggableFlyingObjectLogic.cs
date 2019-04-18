public interface IDraggableFlyingObjectLogic
{
    bool OnDragStart(BoardElementView view);

    bool OnDragEnd(BoardElementView view);
    
    bool Check(BoardElementView cachedViewForDrag);
    
    bool OnClick(BoardElementView selectedView);
}