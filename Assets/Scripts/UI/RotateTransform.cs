using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotateTransform : MonoBehaviour
{
    [SerializeField] private Vector3 m_angle = new Vector3(0, 0, -360);

    [SerializeField] private float m_time = 1;

    [SerializeField] private RotateMode m_mode = RotateMode.LocalAxisAdd;

    [SerializeField] private int m_loopsCount = -1;
    
    [SerializeField] private float m_delay = -1;

    private Quaternion defaultRotation;

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    private void OnEnable()
    {
        defaultRotation = transform.rotation;
        
        var sequence = DOTween.Sequence();
        
        sequence.SetId(this).SetLoops(m_loopsCount);

        if (m_delay > 0) sequence.AppendInterval(m_delay);

        sequence.Append(transform.DORotate(m_angle, m_time, m_mode).SetEase(Ease.Linear));
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
        transform.rotation = defaultRotation;
    }
}
