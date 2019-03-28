using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UISystemDevParamsElementEntity : IUIContainerElementEntity
{
    public string Uid { get; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public string Name;
    
    public Action<float> OnChanged;

    public float DefaultValue = 0;
    
}

public class UISystemDevParamsWindowWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#ButtonAdd")] private UIButtonViewController btnAdd;
    [IWUIBinding("#ButtonRemove")] private UIButtonViewController btnRemove;

    [IWUIBinding("#TopLabel")] private NSText topLabel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UISystemDevParamsWindowWindowModel windowModel = Model as UISystemDevParamsWindowWindowModel;
        
        var systemParams = new List<UISystemDevParamsElementEntity>
        {
            new UISystemDevParamsElementEntity{Name = "Field Drag Threshold", DefaultValue = BoardManipulatorComponent.DragTreshold, OnChanged = (val) => { BoardManipulatorComponent.DragTreshold = val; }},
            new UISystemDevParamsElementEntity{Name = "UI Drag Threshold", DefaultValue = EventSystem.current.pixelDragThreshold, OnChanged = (val) => { EventSystem.current.pixelDragThreshold = (int)val; }}
        };
        
        Fill(UpdateEntities(systemParams), content);
        
        btnAdd.ToState(GenericButtonState.Active).OnClick(() => { });
        btnRemove.ToState(GenericButtonState.Active).OnClick(() => { });
        
        btnAdd.gameObject.SetActive(false);
        btnRemove.gameObject.SetActive(false);

        topLabel.Text = $"deviceName:{SystemInfo.deviceName}; " +
                        $"processorCount:{SystemInfo.processorCount}; \n" +
                        $"deviceUniqueIdentifier:{SystemInfo.deviceUniqueIdentifier}; \n" +
                        $"maxTextureSize:{SystemInfo.maxTextureSize} \n" +
                        $"dpi:{Screen.dpi} wxh:{Screen.width}x{Screen.height} currentResolution:{Screen.currentResolution.ToString()} " +
                        $"sleepTimeout:{Screen.sleepTimeout} \n" +
                        $"safeArea:{Screen.safeArea}";
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UISystemDevParamsWindowWindowModel windowModel = Model as UISystemDevParamsWindowWindowModel;
        
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<UISystemDevParamsElementEntity> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];

            var entity = def;
            
            views.Add(entity);
        }
        
        return views;
    }
}
