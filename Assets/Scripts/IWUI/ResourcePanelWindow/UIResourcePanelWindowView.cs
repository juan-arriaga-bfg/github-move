using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResourcePanelWindowView : UIBaseWindowView 
{
    [IWUIBinding("#ResourceContainer")] protected CanvasGroup resourceContainerUp;
    
    [IWUIBinding("#ResourceContainerDown")] protected CanvasGroup resourceContainerDown;

    private Dictionary<string, Transform> cachedResourcePanels = new Dictionary<string, Transform>();
    
    private Dictionary<string, CanvasGroup> cachedResourcePanelContainerUp = new Dictionary<string, CanvasGroup>();
    
    private Dictionary<string, CanvasGroup> cachedResourcePanelContainerDown = new Dictionary<string, CanvasGroup>();
    
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

    public virtual void TogglePanel(string currency, bool isAnimate)
    {
        
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIResourcePanelWindowModel windowModel = Model as UIResourcePanelWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIResourcePanelWindowModel windowModel = Model as UIResourcePanelWindowModel;
        
    }
}
