using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleButton : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        DOTween.Kill(transform);
        transform.DOScale(new Vector3(1.05f, 1.05f, 1f), 0.35f).SetId(transform);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        DOTween.Kill(transform);
        transform.DOScale(new Vector3(1f, 1f, 1f), 0.35f).SetId(transform);
    }
}