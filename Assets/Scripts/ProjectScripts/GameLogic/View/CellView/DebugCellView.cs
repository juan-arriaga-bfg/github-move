using UnityEngine;

public class DebugCellView : BoardElementView
{
    [SerializeField] private NSText label;

    public void SetIndex(int x, int y)
    {
        label.Text = $"{x}:{y}";
    }
}
