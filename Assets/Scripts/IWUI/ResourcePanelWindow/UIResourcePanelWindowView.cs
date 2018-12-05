using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ResourcePanelUtils
{
    public static void ToggleFadePanel(string currency, bool state)
    {
        var resourcePanelWindow = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        if (resourcePanelWindow == null) return;
        
        resourcePanelWindow.ToggleFadePanel(currency, state);
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

    private Dictionary<string, Transform> cachedResourcePanels = new Dictionary<string, Transform>();
    
    private Dictionary<string, CanvasGroup> cachedResourcePanelContainerUp = new Dictionary<string, CanvasGroup>();
    
    private Dictionary<string, CanvasGroup> cachedResourcePanelContainerDown = new Dictionary<string, CanvasGroup>();
    
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
        }
    }
    
    public virtual void ToggleFadePanel(string currency, bool state)
    {
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
        Transform targetPanel;
        CanvasGroup targetContainerUp;
        CanvasGroup targetContainerDown;
        if (cachedResourcePanelContainerDown.TryGetValue(currency, out targetContainerDown) 
            && cachedResourcePanelContainerUp.TryGetValue(currency, out targetContainerUp)
            && cachedResourcePanels.TryGetValue(currency, out targetPanel))
        {
            float alpha = state ? 1f : 0f;
            if (isAnimate)
            {
                DOTween.Kill(targetContainerUp);
                targetContainerUp.DOFade(alpha, 0.35f).SetId(targetContainerUp);

                DOTween.Kill(targetContainerDown);
                targetContainerDown.DOFade(alpha, 0.35f).SetId(targetContainerDown);

                DOTween.Kill(targetPanel);
                if (state == false)
                {
                    DOTween.Sequence().SetId(targetPanel).AppendInterval(0.35f).OnComplete(() => { targetPanel.gameObject.SetActive(state); });
                }
                else
                {
                    targetPanel.gameObject.SetActive(state);
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
}
