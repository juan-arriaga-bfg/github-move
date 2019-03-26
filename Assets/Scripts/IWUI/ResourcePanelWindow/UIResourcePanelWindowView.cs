using Debug = IW.Logger;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ResourcePanelUtils
{
    public static void ToggleLockFadePanelFor(string currency, bool state, object context)
    {
        var resourcePanelWindow = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        if (resourcePanelWindow == null) return;
        
        resourcePanelWindow.ToggleLockFadePanelFor(currency, state, context);
    }

    public static void ToggleFadePanel(string currency, bool state, bool isCheckLock = false)
    {
        var resourcePanelWindow = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        if (resourcePanelWindow == null) return;
        
        resourcePanelWindow.ToggleFadePanel(currency, state, isCheckLock);
    }

    public static void TogglePanel(string currency, bool state, bool isAnimate = false)
    {
        var resourcePanelWindow = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        if (resourcePanelWindow == null) return;
        
        resourcePanelWindow.TogglePanel(currency, state, isAnimate);
    }

    public static void TogglePanels(bool state, bool isAnimate = false, List<string> ignorePanels = null)
    {
        var resourcePanelWindow = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        if (resourcePanelWindow == null) return;
        
        resourcePanelWindow.TogglePanels(state, isAnimate, ignorePanels);
    }
    
    public static void ToggleFadePanels(bool state, List<string> ignorePanels = null)
    {
        var resourcePanelWindow = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        if (resourcePanelWindow == null) return;
        
        resourcePanelWindow.ToggleFadePanels(state, ignorePanels);
    }
}

public class UIResourcePanelWindowView : UIBaseWindowView 
{
    [IWUIBinding("#ResourceContainer")] protected CanvasGroup resourceContainerUp;
    
    [IWUIBinding("#ResourceContainerDown")] protected CanvasGroup resourceContainerDown;
    
    [IWUIBinding("#HintAnchorEnergyPlusButton")] public Transform HintAnchorEnergyPlusButton;
    
    private Dictionary<string, LockerComponent> panelFadeLockers = new Dictionary<string, LockerComponent>();
    
    private readonly Dictionary<string, Transform> cachedResourcePanels = new Dictionary<string, Transform>();
    
    private readonly Dictionary<string, CanvasGroup> cachedResourcePanelContainerUp = new Dictionary<string, CanvasGroup>();
    
    private readonly Dictionary<string, CanvasGroup> cachedResourcePanelContainerDown = new Dictionary<string, CanvasGroup>();
    
    public UIHintArrowComponent CachedHintArrowComponent { get; private set; }
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        var genericResourcePanelViewControllers = GetComponentsInChildren<UIGenericResourcePanelViewController>();
        
        for (int i = 0; i < genericResourcePanelViewControllers.Length; i++)
        {
            var genericResourcePanelViewController = genericResourcePanelViewControllers[i];
            var itemUid = genericResourcePanelViewController.GetResourceId();

            var resourcePanelContainerUp = GetElement($"#ResourcePanelContainer{itemUid}");
            
            var resourcePanelContainerDown = GetElement($"#ResourcePanelContainerDown{itemUid}");

            if (resourcePanelContainerUp == null || resourcePanelContainerDown == null)
            {
                Debug.LogError($"{resourcePanelContainerUp == null} => {resourcePanelContainerDown == null}");
            }
            
            cachedResourcePanels.Add(itemUid, genericResourcePanelViewController.CachedTransform);
            
            cachedResourcePanelContainerUp.Add(itemUid, resourcePanelContainerUp.GetComponent<CanvasGroup>());
            
            cachedResourcePanelContainerDown.Add(itemUid, resourcePanelContainerDown.GetComponent<CanvasGroup>());
            
            panelFadeLockers.Add(itemUid, new LockerComponent());
            
        }
    }

    public virtual void ToggleLockFadePanelFor(string currency, bool state, object context)
    {
        if (panelFadeLockers.TryGetValue(currency, out var locker))
        {
            if (state)
            {
                locker.Lock(context);
            }
            else
            {
                locker.Unlock(context);
            }
        }
    }
    
    public virtual void ToggleFadePanel(string currency, bool state, bool isCheckLock = false)
    {
        if (isCheckLock && panelFadeLockers.TryGetValue(currency, out var locker))
        {
            if (locker.IsLocked)
            {
                return;
            }
        }
        
        Transform targetPanel;
        CanvasGroup targetContainer;
        if (state)
        {
            if (cachedResourcePanels.TryGetValue(currency, out targetPanel) && cachedResourcePanelContainerUp.TryGetValue(currency, out targetContainer))
            {
                targetPanel.SetParent(targetContainer.transform);
            }
        }
        else
        {
            if (cachedResourcePanels.TryGetValue(currency, out targetPanel) && cachedResourcePanelContainerDown.TryGetValue(currency, out targetContainer))
            {
                targetPanel.SetParent(targetContainer.transform);
            }
        }
    }

    public virtual void TogglePanel(string currency, bool state, bool isAnimate = false)
    {
        var alpha = state ? 1f : 0f;

        if (!cachedResourcePanelContainerDown.TryGetValue(currency, out var targetContainerDown) ||
            !cachedResourcePanelContainerUp.TryGetValue(currency, out var targetContainerUp) ||
            !cachedResourcePanels.TryGetValue(currency, out var targetPanel))
        {
            return;
        }
        
        if (isAnimate)
        {
            DOTween.Kill(targetContainerUp);
            targetContainerUp.DOFade(alpha, 0.35f).SetId(targetContainerUp);

            DOTween.Kill(targetContainerDown);
            targetContainerDown.DOFade(alpha, 0.35f).SetId(targetContainerDown);

            DOTween.Kill(targetPanel);
            
            if (state == false)
            {
                DOTween.Sequence().SetId(targetPanel).AppendInterval(0.35f).OnComplete(() => { targetPanel.gameObject.SetActive(false); });
            }
            else
            {
                targetPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            DOTween.Kill(targetContainerUp);
            DOTween.Kill(targetContainerDown);
            DOTween.Kill(targetPanel);
                
            targetContainerUp.alpha = alpha;
            targetContainerDown.alpha = alpha;
            targetPanel.gameObject.SetActive(state);
        }
    }

    public virtual void TogglePanels(bool state, bool isAnimate = false, List<string> ignorePanels = null)
    {
        foreach (var cachedResourcePanel in cachedResourcePanels)
        {
            if (ignorePanels != null && ignorePanels.Contains(cachedResourcePanel.Key)) continue;

            TogglePanel(cachedResourcePanel.Key, state, isAnimate);
        }
    }
    
    public virtual void ToggleFadePanels(bool state, List<string> ignorePanels = null)
    {
        foreach (var cachedResourcePanel in cachedResourcePanels)
        {
            if (ignorePanels != null && ignorePanels.Contains(cachedResourcePanel.Key)) continue;

            ToggleFadePanel(cachedResourcePanel.Key, state);
        }
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIResourcePanelWindowModel windowModel = Model as UIResourcePanelWindowModel;
        
        CachedHintArrowComponent = new UIHintArrowComponent();
        CachedHintArrowComponent.SetContextView(this, GetCanvas().transform);
        
        Components.RegisterComponent(CachedHintArrowComponent);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIResourcePanelWindowModel windowModel = Model as UIResourcePanelWindowModel;
        
    }

    /// <summary>
    /// Data is calculated for orthoSize == 1. If you need to compare with another camera points, do not forget to convert them too.
    /// </summary>
    public float GetSafeZoneHeightInWorldSpace()
    {
        Vector3[] corners = new Vector3[4];
        resourceContainerDown.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 containerBottomLeft = corners[0];

        Camera camera = Controller.Window.Layers[0].ViewCamera;
        float orthoSize = camera.orthographicSize;
        
        Vector3 viewportTopLeft = camera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));

        var delta = viewportTopLeft.y - containerBottomLeft.y;

        return delta / orthoSize;
    }
}
