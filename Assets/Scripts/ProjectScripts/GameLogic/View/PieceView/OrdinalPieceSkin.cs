using UnityEngine;

public class OrdinalPieceSkin : PieceSkin
{
    private int index;

    public override void Init(int value)
    {
        index = value - 1;
        base.Init(index);
    }

    public override void UpdateView()
    {
        index = Mathf.Clamp(index - 1, 0, skins.Count - 1);
        UpdateView(index);
    }

    private void OnDisable()
    {
        index = 0;
    }
}