using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIProfileCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#SlotList")] private UIContainerViewController slotList;
    [IWUIBinding("#ScrollView")] private ScrollRect scroll;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
        SetTitle(windowModel.Title);

        CreateList(windowModel, list =>
        {
            Fill(list, slotList);
        });
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
    }

    private void CreateList(UIProfileCheatSheetWindowModel model, Action<List<IUIContainerElementEntity>> onComplete)
    {
        model.GetExistingProfiles(data =>
        {
            var tabViews = new List<IUIContainerElementEntity>(data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                var slotData = data[i];
                var tabEntity = new UIProfileCheatSheetElementEntity
                {
                    SlotData = slotData,
                    WindowController = Controller,
                    OnSelectEvent = null,
                    OnDeselectEvent = null
                };
                tabViews.Add(tabEntity);
            }

            onComplete(tabViews);
        });
    }
}