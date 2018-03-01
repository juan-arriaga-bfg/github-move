using System.Collections;
using System.Collections.Generic;
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
            -barOffset,
            CachedTransform.localPosition.y,
            CachedTransform.localPosition.z
        );
    }
}
