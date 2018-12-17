using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UIHintArrowViewController : IWUIWindowViewController
{
    [SerializeField] private CanvasGroup viewAnchor;
    
    [SerializeField] private NSText label;

    protected float lifeTime = -1f;

    protected bool isShowing = false;

    protected Vector3 offset = Vector3.zero;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);

        Reset();
    }

    public virtual UIHintArrowViewController SetLabel(string message)
    {
        if (label != null) label.Text = message;

        return this;
    }

    public virtual UIHintArrowViewController Reset()
    {
        this.lifeTime = -1f;
        this.label.Text = "";
        this.isShowing = false;
        this.offset = Vector3.zero;
        viewAnchor.alpha = 0f;
        DOTween.Kill(this);
        return this;
    }

    public virtual UIHintArrowViewController SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;

        return this;
    }

    public virtual UIHintArrowViewController SetOffset(Vector3 offset)
    {
        this.offset = offset;

        return this;
    }

    public virtual UIHintArrowViewController SetTarget(Transform point, Camera cameraContext)
    {
        SetPosition(point, cameraContext);
        StartCoroutine(CoWaitForPosition(point, cameraContext));
        return this;
    }

    private void SetPosition(Transform point, Camera cameraContext)
    {
        var screenPoint = cameraContext.WorldToScreenPoint(point.position);
        var baseContext = (context as UIBaseWindowView);
        if (baseContext == null) return;

        var targetPosition = baseContext.GetCanvas().worldCamera.ScreenToWorldPoint(screenPoint);
        
        // Find position of objects in grid
        CachedTransform.position = targetPosition;
        CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x + offset.x, CachedTransform.localPosition.y + offset.y, 0f);
    }
    
    IEnumerator CoWaitForPosition(Transform point, Camera cameraContext)
    {
        yield return new WaitForEndOfFrame();

        if (point != null)
        {
            SetPosition(point, cameraContext);
        }
    }

    public virtual UIHintArrowViewController SetDirection(Vector3 direction)
    {
        var targetRotation = Quaternion.Euler(0f, 0f, direction.y < 0 ? 90f : (direction.y > 0 ? -90f : 0f));
        CachedTransform.localRotation = targetRotation;
        return this;
    }

    protected virtual void Return()
    {
        gameObject.SetActive(false);
    }
    
    public virtual void Hide()
    {
        isShowing = false;
        Hide(false);
    }

    public virtual void Hide(bool isReturn)
    {
        if (gameObject.activeSelf == false) return;

        if (isReturn)
        {
            DOTween.Kill(this);
            Return();
            return;
        }
        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        sequence.Append(viewAnchor.DOFade(0f, 0.35f));
        sequence.OnComplete(Return);
    }

    public virtual UIHintArrowViewController Show()
    {
        gameObject.SetActive(true);

        DOTween.Kill(this);

        if (!isShowing)
        {
            viewAnchor.alpha = 0f;
            viewAnchor.DOFade(1f, 0.35f).SetId(this);
        }

        isShowing = true;
        
        if (lifeTime < 0f)
        {
            return this;
        }
        
        var sequence = DOTween.Sequence().SetId(this);
        sequence.AppendInterval(lifeTime);
        sequence.OnComplete(Hide);

        return this;
    }
    
}
