using UnityEngine;
using UnityEngine.Rendering;

public class LockView : BoardElementView
{
    [SerializeField] private NSText label;
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private Material grayscaleMaterial;
    [SerializeField] private Transform hintArrowTarget;
    
    private Material defaultMaterial;
    
    public string Value {set => label.Text = value;}

    public Transform GetHintTarget()
    {
        return hintArrowTarget;
    }

    public override void Init(BoardRenderer context)
    {
        base.Init(context);
        defaultMaterial = image.material;
        Value = "";
    }
    
    public void SetGrayscale(bool enabled)
    {
        image.material = enabled ? grayscaleMaterial : defaultMaterial;
    }

    public void SetSortingOrder(BoardPosition position)
    {
        var cachedSpriteSortingGroup = GetComponent<SortingGroup>();
        
        if (cachedSpriteSortingGroup == null) cachedSpriteSortingGroup = gameObject.AddComponent<SortingGroup>();
        
        cachedSpriteSortingGroup.sortingOrder = GetLayerIndexBy(new BoardPosition(position.X, position.Y - 1, BoardLayer.Piece.Layer));
    }
}