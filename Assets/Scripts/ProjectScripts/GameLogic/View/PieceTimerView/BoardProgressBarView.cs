using UnityEngine;

public class BoardProgressBarView : IWBaseMonoBehaviour
{
    [SerializeField] private SpriteRenderer barView;
    
    [SerializeField] private float barWidth;

    public virtual void SetProgress(float progress)
    {
        float targetWidth = Mathf.Lerp(0f, barWidth, progress);
        barView.size = new Vector2(targetWidth, barView.size.y);
        float barOffset = Mathf.Lerp(barWidth * 0.5f, 0f, progress);
        
        barView.transform.localPosition = new Vector3
        (
            float.IsNaN(barOffset) ? 0 : -barOffset,
            barView.transform.localPosition.y,
            barView.transform.localPosition.z
        );
    }
}
