public interface ITouchableBoardObjectLogic
{
    bool IsDraggable { get; }
    
    bool OnDragStart(BoardElementView view);

    bool OnDragEnd(BoardElementView view);
    
    bool Check(BoardElementView view);
    
    bool OnClick(BoardElementView view);
}