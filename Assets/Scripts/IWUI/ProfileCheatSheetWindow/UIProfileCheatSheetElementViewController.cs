using Debug = IW.Logger;
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
    [IWUIBinding("#BtnSend")]  private UIButtonViewController btnSend;

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
        userProfile = profile?.CurrentProfile;
        
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
        // Disable btn to avoid user errors
        btnDlgSave.gameObject.SetActive(false);
        
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
        
        btnSend.OnClick(() =>
        {
            SendProfile();
        });

        bool active = slotData.SlotPath == ProfileSlots.ActiveSlot;
        btnDel.gameObject.SetActive(!active);
    }

    private void SendProfile()
    {
        var dataMapper = ProfileSlots.GetDataMapper(slotData.SlotPath);
        var text = dataMapper.GetJsonDataAsString();
        
        TextEditor te = new TextEditor();
        te.text = text;
        te.SelectAll();
        te.Copy();
        
        Debug.LogWarning("[UIProfileCheatSheetElementViewController] => SendProfile: Profile text copied to clipboard");
        
#if !UNITY_EDITOR 
        // string fileName = Application.persistentDataPath + "/profile.json";
        NativeShare.Share(text, null, null,  "Share profile json", "text/plain", true);
#endif
    }


    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
    }

    public void UpdateUi()
    {
        GameDataManager dm = null;

        bool isLoadedCorrectly = string.IsNullOrEmpty(slotData.Error) && userProfile != null;

        if (isLoadedCorrectly)
        {
            dm = new GameDataManager();
            try
            {
                dm.SetupComponents(userProfile);
                dm.Reload();
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIProfileCheatSheetElementViewController] => GameDataManager parsing error: {e.Message} - {e.StackTrace}");
                slotData.Error = $"GameDataManager SetupComponents failed: {e.Message}";
                isLoadedCorrectly = false;
            }
        }
        
        if (!isLoadedCorrectly)
        {
            lblRev.Text = $"Can't load '{Colorize(slotData.SlotPath, COLOR_YELLOW)}'";
            lblData.Text = $"{Colorize(slotData.Error, COLOR_WHITE)}";
            lblTimestamp.Text = "";
            ToggleBackColor();
            return;
        }

        lblRev.Text = $"v{userProfile.SystemVersion.ToString()} r{userProfile.Version.ToString()} '{Colorize(slotData.SlotPath, COLOR_YELLOW)}'";
        if (slotData.SlotPath == ProfileSlots.DEFAULT_SLOT_PATH)
        {
            lblRev.Text += $" {Colorize("[Default]", COLOR_WHITE)}";
        }

        string timestamp = ParseTimestamp();
        lblTimestamp.Text = $"{timestamp.Replace("Z", "")}";

        int level = dm.LevelsManager.Level;

        var listResources = new List<CurrencyPair>
        {
            new CurrencyPair {Currency = Currency.Coins.Name, Amount = userProfile.GetStorageItem(Currency.Coins.Name).Amount},
            new CurrencyPair {Currency = Currency.Crystals.Name, Amount = userProfile.GetStorageItem(Currency.Crystals.Name).Amount},
        };
        string resourcesStr = CurrencyHelper.RewardsToString("  ", null, listResources);

        lblData.Text = !string.IsNullOrEmpty(slotData.Error) ? slotData.Error : $"Level: {level}    {resourcesStr}";
        
        ToggleBackColor();
    }

    private string ParseTimestamp()
    {
        string timestamp;
        try
        {
            if (string.IsNullOrEmpty(userProfile.Timestamp))
            {
                timestamp = "No timestamp specified";
            }
            else if (DateTime.TryParse(userProfile.Timestamp, out DateTime parsedDate))
            {
                timestamp = $"{parsedDate.ToLocalTime():u}";
            }
            else if (long.TryParse(userProfile.Timestamp, out long parsedUnixTimestamp))
            {
                var dateTime = UnixTimeHelper.UnixTimestampToDateTime(parsedUnixTimestamp);
                timestamp = $"{dateTime:u}";
            }
            else
            {
                timestamp = "Timestamp parsing error";
            }
        }
        catch (Exception e)
        {
            timestamp = e.GetType().ToString();
        }

        return timestamp;
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