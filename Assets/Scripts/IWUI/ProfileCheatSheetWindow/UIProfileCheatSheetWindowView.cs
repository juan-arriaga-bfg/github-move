using System;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

using UnityEngine.UI;

public class UIProfileCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#SlotList")] private UIContainerViewController slotList;
    [IWUIBinding("#ScrollView")] private ScrollRect scroll;
    
    [IWUIBinding("#BtnNewSlot")] private UIButtonViewController btnNewSlot;
    [IWUIBinding("#BtnNewGame")] private UIButtonViewController btnNewGame;
    [IWUIBinding("#BtnDelAll")]  private UIButtonViewController btnDelAll;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        ProfileService.Instance.Manager.UploadCurrentProfile();
        
#if UNITY_EDITOR
        ProfileService.Instance.Manager.SaveLocalProfile();
#endif
        
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
        SetTitle(windowModel.Title);

        Reload();
    }

    public void Reload()
    {
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
        InitList(windowModel);
        
        InitButtons();
    }

    private void InitList(UIProfileCheatSheetWindowModel windowModel)
    {
        CreateList(windowModel, list => { Fill(list, slotList); });
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
                    WindowView = this,
                    OnSelectEvent = null,
                    OnDeselectEvent = null
                };
                tabViews.Add(tabEntity);
            }

            onComplete(tabViews);
        });
    }
    
    private void InitButtons()
    {
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
        btnNewSlot.OnClick(() =>
        {
            CreateNewSlot();
            InitList(windowModel);
        });
        
        btnNewGame.OnClick(() =>
        {
            UIMessageWindowController.CreateMessageWithTwoButtons(
                "Reset the progress",
                "Do you want to reset the progress and start playing from the beginning? 'Default' slot will be cleared and used as active.",
                "<size=30>Reset progress!</size>",
                "No!",
                () =>
                {
                    Controller.CloseCurrentWindow(windowController =>
                    {
                        ProfileSlots.Delete(ProfileSlots.DEFAULT_SLOT_PATH);
                        ProfileSlots.ActiveSlot = ProfileSlots.DEFAULT_SLOT_PATH;
                    
                        DevTools.ReloadScene();
                    });
                },
                () => {});
        });
        
        btnDelAll.OnClick(() =>
        {
            UIMessageWindowController.CreateMessageWithTwoButtons(
                "Delete All",
                "Do you want to delete all saves except the active?",
                "Delete",
                "Cancel",
                () =>
                {
                    DeleteAll(false, Reload);;
                },
                () => {});
        });
    }

    private void DeleteAll(bool includeActive, Action onComplete)
    {
        UIProfileCheatSheetWindowModel model = Model as UIProfileCheatSheetWindowModel;
        
        model.GetExistingProfiles(data =>
        {
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                if (item.SlotPath == ProfileSlots.ActiveSlot && !includeActive)
                {
                    continue;
                }
                
                ProfileSlots.Delete(item.SlotPath);
            }
            
            onComplete.Invoke();
        });
    }
    
    private void CreateNewSlot()
    {
        string path = UIProfileCheatSheetWindowModel.GetFreeSlotPath();
        if (string.IsNullOrEmpty(path))
        {
            UIMessageWindowController.CreateMessage(
                "Error",
                "All available slots are in use. Please remove something before continue!"
            );
            
            return;
        }

        var dataMapper = ProfileSlots.GetDataMapper(path);
        ProfileService.Instance.Manager.UploadCurrentProfile(dataMapper);
       
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}