using TMPro;
using UnityEngine;

public class DebugCellView : BoardElementView
{
    [SerializeField] private NSText label;
    [SerializeField] private TextMeshPro textMesh;
    
    private bool markEnable;
    private BoardPosition position;
    
    public BoardPosition Position => position;
    
    public void SetIndex(int x, int y)
    {
        position = new BoardPosition(x, y);
        markEnable = false;
        UpdateText();        
    }

    public void Mark(bool enable)
    {
        if (markEnable == enable)
            return;
        markEnable = enable;
        UpdateText();
    }

    private void UpdateText()
    {
        label.Text = $"{position.X}:{position.Y}";
        if (markEnable)
        {
            label.Text = $"{label.Text}*";
            var color = textMesh.color;
            color.a = 0.5f;
            textMesh.color = color;
        }
        else
        {
            var color = textMesh.color;
            color.a = 0.25f;
            textMesh.color = color;
        }
            
    }

    public void SetText(string text)
    {
        label.Text = text;
    }
}
