using DG.Tweening;
using UnityEngine;

public class PartCellView : BoardElementView
{
    [SerializeField] private SpriteRenderer selection;
    [SerializeField] private Material defaultSelectionMaterial;
    
    private readonly Color baseColor = new Color(0.6f, 0.4f, 0.2f);
    
    public void ToggleSelection(bool enabled)
    {
        if (enabled)
        {
            DOTween.Kill(animationUid);

            var sequence = DOTween.Sequence().SetId(animationUid);
            
            sequence.Insert(0f, selection.DOColor(baseColor, 0.2f));
            selection.material = defaultSelectionMaterial;
        }
        else
        {
            DOTween.Kill(animationUid);
            
            selection.color = baseColor;
        }
    }
}