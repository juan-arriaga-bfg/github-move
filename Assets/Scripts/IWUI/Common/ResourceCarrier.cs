using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCarrier : IWUIWindowViewController, IResourceCarrier
{
    [SerializeField] private Image icon;
	
	[SerializeField] private Image iconShadow;

	[SerializeField] private Transform iconShadowTransform;
	
	[SerializeField] private CanvasGroup iconShadowCanvasGroup;

	public Action Callback;

	protected IResourceCarrierView view;

	protected int offset;

	protected Vector3 startPosition;
	
	protected float upScale = 1.6f;
	
	private Vector3 targetScale = Vector3.one;

	protected bool isUseFallback = true;

	public virtual IResourceCarrier SetStartPosition(Vector3 startPosition)
	{
		this.startPosition = startPosition;

		return this;
	}

	public virtual IResourceCarrier IsUseFallback(bool state)
	{
		this.isUseFallback = state;
		return this;
	}
	
	public Vector3 GetTargetScale()
	{
		return targetScale;
	}

	public virtual IResourceCarrier Init(IResourceCarrierView view, int offset)
    {
        this.offset = offset;
        this.view = view;

        icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format("icon_{0}", view.GetResourceId()));
	    CachedRectTransform.sizeDelta = view.GetAnchorRect().sizeDelta;

	    iconShadow.sprite = icon.sprite;

	    CachedTransform.localScale = Vector3.zero;
	    this.targetScale = Vector3.one;
	    this.upScale = 1.6f;
	    this.isUseFallback = true;
	    
		return this;
    }
	
    public IResourceCarrier RefreshIcon(string id)
    {
	    icon.sprite = IconService.Instance.Manager.GetSpriteById(id);
	    iconShadow.sprite = icon.sprite;
	    
	    return this;
    }
	
    public IResourceCarrier SetTargetScale(Vector3 targetScale)
    {
        this.targetScale = targetScale;

	    return this;
    }
	
	public IResourceCarrier SetUpScale(float upScale)
	{
		this.upScale = upScale;

		return this;
	}
	
	public IResourceCarrier SetParent(Transform parentTransform)
	{
		CachedTransform.SetParentAndReset(parentTransform);
		CachedTransform.SetAsLastSibling();
		CachedTransform.SetLayerRecursive(parentTransform.gameObject.layer);

		return this;
	}

    public virtual IResourceCarrier Launch(float delay = 0f, float startOffsetCoef = 0.2f, float offsetCoef = 2f, float backOffsetCoefFrom = -2f, float backOffsetCoefTo = 2f)
    {
	    view.RegisterCarrier(this);

	    iconShadowCanvasGroup.alpha = 0f;
	    iconShadowTransform.localPosition = Vector3.zero;

	    float flydelay = GetFlyDelay();
	    float fallBackDuration = 0.6f + flydelay;
	    float flyDuration = 0.6f * (1f + flydelay);
 
	    var shadowTargetOffset = new Vector3(20f, -20f, 0f);

	    DOTween.Kill(this);
	    
	    Sequence sequence = DOTween.Sequence().SetId(this);
	    sequence.AppendInterval(Time.deltaTime);
	    sequence.AppendInterval(delay);
	    if (isUseFallback)
	    {
		    sequence.AppendCallback(() =>
		    {
			    CachedTransform.position = view.RenderCamera.ScreenToWorldPoint(startPosition);
			    CachedTransform.localPosition =
				    new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, 0f);
			    CachedTransform.localScale = Vector3.one;

			    var targetPositionBack = GetFallBackPosition();

			    DOTween.Sequence().SetId(this)
				    .Insert(0f, CachedTransform.DOMove(targetPositionBack, fallBackDuration).SetEase(Ease.OutBack))
				    .Insert(0f, CachedTransform.DOScale(targetScale * upScale * 0.7f, fallBackDuration));

		    });

		    sequence.AppendInterval(fallBackDuration * 0.5f);
	    }

	    sequence.AppendCallback(() =>
	    {
		    var movePath = GetTargetPath(startOffsetCoef, offsetCoef, backOffsetCoefFrom, backOffsetCoefTo);
		    
		    DOTween.Sequence().SetId(this)
			    .Insert(0f, CachedTransform.DOPath(movePath, flyDuration, PathType.CatmullRom).SetEase(Ease.InSine))
			    .Insert(0f, CachedTransform.DOScale(targetScale * upScale, flyDuration * 0.6f))
			    
			    .Insert(0f, iconShadowCanvasGroup.DOFade(0.2f, flyDuration * 0.6f))
			    .Insert(0f, iconShadowTransform.DOScale(1.2f, flyDuration * 0.6f))
			    .Insert(0f, iconShadowTransform.DOLocalMove(shadowTargetOffset, flyDuration * 0.6f))
			    
			    .Insert(flyDuration * 0.6f, iconShadowCanvasGroup.DOFade(0f, flyDuration * 0.4f))
			    .Insert(flyDuration * 0.6f, iconShadowTransform.DOLocalMove(Vector3.zero, flyDuration * 0.4f))
			    .Insert(flyDuration * 0.6f, iconShadowTransform.DOScale(Vector3.one, flyDuration * 0.4f))
			    
			    .Insert(flyDuration * 0.6f, CachedTransform.DOScale(targetScale, flyDuration * 0.4f));
	    });
	    
	    sequence.AppendInterval(flyDuration);
	    
	    sequence.OnComplete(OnComplete);

	    return this;
    }

	public GameObject Root
	{
		get { return gameObject; }
	}

	protected float GetFlyDelay()
	{
		float flydelay = (view.Carriers.Count - 1) * 0.08f;

		return flydelay;
	}

	protected Vector3 GetTargetPosition()
	{
		var targetPosition = view.GetAcnhorPosition();

		return targetPosition;
	}

	protected Vector3 GetFallBackPosition( float backOffsetCoefFrom = -2f, float backOffsetCoefTo = 2f)
	{
		var targetPosition = GetTargetPosition();
		var currentPosition = CachedTransform.position;
		var targetPositionBack = currentPosition - targetPosition * 0.2f;
		var normalVector = Vector3.Cross(targetPositionBack, targetPosition);

		var rotatedVector = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-20f, 20f)) * normalVector;
		targetPositionBack = targetPositionBack + rotatedVector.normalized * UnityEngine.Random.Range(backOffsetCoefFrom, backOffsetCoefTo);

		return targetPositionBack;
	}

	protected Vector3[] GetTargetPath(float startOffsetCoef = 0.2f, float offsetCoef = 2f, float backOffsetCoefFrom = -2f, float backOffsetCoefTo = 2f)
	{
		var targetPosition = GetTargetPosition();//targetCamera.ScreenToWorldPoint(view.GetScreenPosition());
		var targetPositionBack = GetFallBackPosition( backOffsetCoefFrom, backOffsetCoefTo);

		var crossVector = Vector3.Cross(targetPositionBack, targetPosition).normalized;
		float sign = UnityEngine.Random.Range(0, 2) == 0 
			? -1f 
			: 1f;

		Vector3[] movePath = new Vector3[2];
		movePath[0] = (targetPositionBack + targetPosition) * startOffsetCoef * sign + crossVector * offsetCoef * sign;
		movePath[1] = targetPosition;

		return movePath;
	}
	
	protected virtual void OnComplete()
	{
		Callback?.Invoke();
		Callback = null;

		if (view == null)
	    {
		    UIService.Get.ReturnCachedObject(gameObject);
	        return;
	    }
		
        view.UpdateResource(offset);

        view.UnRegisterCarrier(this);

		UIService.Get.ReturnCachedObject(gameObject);
    }
}
