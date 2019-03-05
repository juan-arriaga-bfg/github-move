using System;
using System.Collections.Generic;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class UIProfileCheatSheetElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#LblRev")] private NSText lblRev;
    [IWUIBinding("#LblTimestamp")] private NSText lblTimestamp;
    [IWUIBinding("#LblData")] private NSText lblData;

    [IWUIBinding("#BackNormal")] private GameObject backNormal;
    [IWUIBinding("#BackActive")] private GameObject backActive;
    [IWUIBinding("#BackError")] private GameObject backError;
    
    [IWUIBinding("#BtnSave")] private UIButtonViewController btnDlgSave;
    [IWUIBinding("#BtnLoad")] private UIButtonViewController btnDlgLoad;
    [IWUIBinding("#BtnDel")]  private UIButtonViewController btnDel;

    private UIProfileCheatSheetElementEntity targetEntity;
    private UIProfileCheatSheetSlotData slotData;
    private ProfileManager<UserProfile> profile;
    private UserProfile userProfile;

    const string COLOR_YELLOW = "#FFFA1F";
    const string COLOR_WHITE  = "#FFFFFF";
    
    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIProfileCheatSheetElementEntity;
        slotData = targetEntity.SlotData;
        profile = slotData.Profile;
        userProfile = profile.CurrentProfile;
        
        InitButtons();
        
        UpdateUi();
    }


    private void RefreshAfterChange()
    {
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        targetEntity.WindowView.Reload();
    }
    
    private void InitButtons()
    {
        btnDlgSave.OnClick(() =>
        {
            var dataMapper = ProfileSlots.GetDataMapper(slotData.SlotPath);
            ProfileService.Instance.Manager.UploadCurrentProfile(dataMapper);

            RefreshAfterChange();
        });
        
        btnDlgLoad.OnClick(() =>
        {   
            targetEntity.WindowController.CloseCurrentWindow(controller =>
            {
                ProfileSlots.ActiveSlot = slotData.SlotPath;
                DevTools.ReloadScene();
            });
        });
        
        btnDel.OnClick(() =>
        {
            ProfileSlots.Delete(slotData.SlotPath);
            RefreshAfterChange();
        });

        bool active = slotData.SlotPath == ProfileSlots.ActiveSlot;
        btnDel.gameObject.SetActive(!active);
    }


    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
    }

    public void UpdateUi()
    {
        ToggleBackColor();
        
        if (!string.IsNullOrEmpty(slotData.Error))
        {
            lblRev.Text = $"Can't load '{Colorize(slotData.SlotPath, COLOR_YELLOW)}'";
            lblData.Text = $"{Colorize(slotData.Error, COLOR_WHITE)}";
            lblTimestamp.Text = "";
            return;
        }
        
        GameDataManager dm = new GameDataManager();
        dm.SetupComponents(userProfile);
        dm.Reload();
        
        lblRev.Text = $"Rev {userProfile.Version.ToString()} '{Colorize(slotData.SlotPath, COLOR_YELLOW)}'";
        if (slotData.SlotPath == ProfileSlots.DEFAULT_SLOT_PATH)
        {
            lblRev.Text += $" {Colorize("[Default]", COLOR_WHITE)}";
        }

        string timestamp = $"{DateTime.Parse(userProfile.Timestamp).ToLocalTime() :u}";
        lblTimestamp.Text = $"{timestamp.Replace("Z", "")}";

        int level = dm.LevelsManager.Level;

        var listResources = new List<CurrencyPair>
        {
            new CurrencyPair {Currency = Currency.Coins.Name, Amount = userProfile.GetStorageItem(Currency.Coins.Name).Amount},
            new CurrencyPair {Currency = Currency.Crystals.Name, Amount = userProfile.GetStorageItem(Currency.Crystals.Name).Amount},
        };
        string resourcesStr = CurrencyHelper.RewardsToString("  ", null, listResources);
        
        lblData.Text = !string.IsNullOrEmpty(slotData.Error) ? slotData.Error : $"Level: {level}    {resourcesStr}";
    }

    private void ToggleBackColor()
    {
        bool active = slotData.SlotPath == ProfileSlots.ActiveSlot;
        bool error = !string.IsNullOrEmpty(slotData.Error);
        backNormal.SetActive(!active && !error);
        backActive.SetActive(active  && !error);
        backError.SetActive(error);
    }

    private string Colorize(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
}