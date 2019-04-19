using UnityEngine;

public class DebugTextView : BoardElementView
{
    [SerializeField] private NSText label;
    
    private bool markEnable;
    private BoardPosition position;
    
    public BoardPosition Position => position;

    public void SetId(int id)
    {
        label.Text = PieceType.Parse(id);
    }
    
    public void SetIndex(int x, int y)
    {
        position = new BoardPosition(x, y);
        markEnable = false;
        UpdateText();        
    }

    public void Mark(bool enable)
    {
        if (markEnable == enable) return;
        
        markEnable = enable;
        UpdateText();
    }

    private void UpdateText()
    {
        label.Text = $"{position.X}:{position.Y}{(markEnable ? "*" : "")}";
        label.TextLabel.alpha = markEnable ? 0.5f : 0.25f;
    }

    public void SetText(string text)
    {
        label.Text = text;
    }
}