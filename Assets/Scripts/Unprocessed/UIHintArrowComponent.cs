using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHintArrowComponent : ECSEntity
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    protected UIBaseWindowView contextView;

    protected Transform contextViewAnchor;

    protected Dictionary<Transform, UIHintArrowViewController> cachedArrowViewControllers = new Dictionary<Transform, UIHintArrowViewController>();

    public virtual ECSEntity SetContextView(UIBaseWindowView contextView, Transform contextViewAnchor)
    {
        this.contextView = contextView;
        this.contextViewAnchor = contextViewAnchor;

        return this;
    }

    public virtual void ClearArrows()
    {
        foreach (var cachedArrowViewControllerPair in cachedArrowViewControllers)
        {
            var cachedArrowViewController = cachedArrowViewControllerPair.Value;

            if (cachedArrowViewController == null) continue;
            
            contextView.UnRegisterWindowViewController(cachedArrowViewController);
            cachedArrowViewController.Hide();
        }
        
        cachedArrowViewControllers.Clear();
    }

    public void HideArrow(Transform anchor)
    {
        if (cachedArrowViewControllers.TryGetValue(anchor, out var cachedArrowViewController) == false) return;
        
        contextView.UnRegisterWindowViewController(cachedArrowViewController);
        cachedArrowViewController.Hide(false);
    }

    public UIHintArrowViewController ShowArrow(Transform anchor, float lifetime = 2.5f, Vector3 offset = default(Vector3))
    {
        return ShowArrow(anchor, offset, anchor.eulerAngles, Vector3.one, lifetime, true);
    }
    
    public UIHintArrowViewController ShowArrow(Transform anchor, Vector3 offset, Vector3 rotation, Vector3 scale, float lifetime = -1, bool animated = true)
    {
        UIHintArrowViewController arrow;
        
        if (!cachedArrowViewControllers.ContainsKey(anchor))
        {
            arrow = UIService.Get.GetCachedObject<UIHintArrowViewController>(R.UIHintArrow);
            arrow.CachedTransform.SetParentAndReset(contextViewAnchor);
            arrow.CachedTransform.SetAsLastSibling();
            arrow.CachedTransform.SetLayerRecursive(contextView.GetCanvas().gameObject.layer);

            arrow.CachedTransform.position = anchor.position;
            arrow.CachedTransform.localPosition = new Vector3(
                arrow.CachedTransform.localPosition.x + offset.x,
                arrow.CachedTransform.localPosition.y + offset.y, 0f);
            arrow.CachedTransform.localRotation = Quaternion.Euler(rotation);
            arrow.CachedTransform.localScale = scale;

            contextView.RegisterWindowViewController(arrow);

            cachedArrowViewControllers.Add(anchor, arrow);
        }
        else
        {
            arrow = cachedArrowViewControllers[anchor];
        }

        arrow.SetLabel("");
        arrow.SetLifeTime(lifetime);
        if (animated)
        {
            arrow.Show();
        }
        
        return arrow;
    }
}