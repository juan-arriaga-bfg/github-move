using UnityEngine;

public class DebugCellView : BoardElementView
{
    [SerializeField] private NSText label;

    public void SetIndex(int x, int y)
    {
        label.Text = string.Format("{0}:{1}", x, y);
    }
}
