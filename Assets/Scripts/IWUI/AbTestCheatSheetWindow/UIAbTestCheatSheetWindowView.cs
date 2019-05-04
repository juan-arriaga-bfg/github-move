using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAbTestCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TestsList")] private UIContainerViewController testsList;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIAbTestCheatSheetWindowModel model = Model as UIAbTestCheatSheetWindowModel;
        
        SetTitle(model.Title);

        Fill(CreateList(model), testsList);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIAbTestCheatSheetWindowModel windowModel = Model as UIAbTestCheatSheetWindowModel;
        
    }
    
    private List<IUIContainerElementEntity> CreateList(UIAbTestCheatSheetWindowModel model)
    {
        var tests = model.Items;
        
        var tabViews = new List<IUIContainerElementEntity>(tests.Count);
        for (int i = 0; i < tests.Count; i++)
        {
            var test = tests[i];

            var tabEntity = new UIAbTestCheatSheetElementEntity
            {
                Data = test,
                WindowController = Controller,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            tabViews.Add(tabEntity);
        }

        return tabViews;
    }

}
