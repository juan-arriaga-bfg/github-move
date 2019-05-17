using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleButton : Button
{
    private Vector3? baseScale;
    private static readonly Vector3 scaleModifier = new Vector3(0.05f, 0.05f, 0f);
    public void Start()
    {
        if (baseScale == null)
        {
            baseScale = transform.localScale;
        }
        baseScale = transform.localScale;
    }

    public void OnEnable()
    {
        if (baseScale == null)
        {
            baseScale = transform.localScale;
        }
    }
    
    public void OnDisable()
    {
        if (baseScale == null)
        {
            return;
        }
        DOTween.Kill(transform);
        transform.localScale = baseScale.Value;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (baseScale == null)
        {
            return;
        }
        base.OnPointerDown(eventData);
        DOTween.Kill(transform);
        transform.DOScale(baseScale.Value + scaleModifier, 0.35f).SetId(transform);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (baseScale == null)
        {
            return;
        }
        base.OnPointerUp(eventData);
        DOTween.Kill(transform);
        transform.DOScale(baseScale.Value, 0.35f).SetId(transform);
    }
}