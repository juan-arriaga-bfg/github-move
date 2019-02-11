using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UIHintArrowViewController : IWUIWindowViewController, IHintArrow
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
        if (isShowing && this.lifeTime < 0) return this;
        
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
    
    public virtual void Hide()
    {
        if (isShowing == false) return;
        
        isShowing = false;
        
        DOTween.Kill(this);
        
        var sequence = DOTween.Sequence().SetId(this);
        
        sequence.Append(viewAnchor.DOFade(0f, 0.35f * viewAnchor.alpha));
        sequence.OnComplete(() => gameObject.SetActive(false));
    }

    public virtual UIHintArrowViewController Show()
    {
        if (isShowing) return this;
        
        gameObject.SetActive(true);
        
        isShowing = true;
        
        DOTween.Kill(this);
        
        var sequence = DOTween.Sequence().SetId(this);

        sequence.Append(viewAnchor.DOFade(1f, 0.35f * (1 - viewAnchor.alpha)));
        
        if (lifeTime < 0f) return this;
        
        sequence.AppendInterval(lifeTime);
        sequence.OnComplete(Hide);

        return this;
    }

    public void Remove(float delay)
    {
        DOTween.Kill(this);
        
        if (isShowing == false)
        {
            Return();
            return;
        }

        isShowing = false;
        
        var sequence = DOTween.Sequence();
        
        sequence.AppendInterval(delay);
        sequence.Append(viewAnchor.DOFade(0f, 0.35f));
        sequence.OnComplete(Return);
    }

    private void Return()
    {
        if (gameObject == null) return;
        
        UIService.Get.ReturnCachedObject(gameObject);
        gameObject.SetActive(false);
    }
}
