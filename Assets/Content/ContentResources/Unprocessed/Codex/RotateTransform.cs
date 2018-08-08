using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotateTransform : MonoBehaviour 
{
    [SerializeField]
    private Vector3 m_angle = new Vector3(0, 0, -360);

    [SerializeField]
    private float m_time = 1;

    [SerializeField]
    private RotateMode m_mode = RotateMode.LocalAxisAdd;

    [SerializeField]
    private int m_loopsCount = -1;

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    private void OnEnable()
    {
        transform.DORotate(m_angle, m_time, m_mode)
            .SetId(this)
            .SetEase(Ease.Linear)
            .SetLoops(m_loopsCount);
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }
}
