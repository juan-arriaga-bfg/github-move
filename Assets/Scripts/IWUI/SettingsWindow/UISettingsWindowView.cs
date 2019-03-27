using Debug = IW.Logger;
using System;
using UnityEngine;

public class UISettingsWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#VersionLabel")] private NSText versionLabel;
    
    [IWUIBinding("#LanguageLabel")] private NSText languageLabel;
    [IWUIBinding("#SoundLabel")] private NSText soundLabel;
    [IWUIBinding("#MusicLabel")] private NSText musicLabel;
    
    [IWUIBinding("#LanguageButtonLabel")] private NSText btnLanguageLabel;
    
    [IWUIBinding("#ProfileLabel")] private NSText profileLabel;
    
    [IWUIBinding("#LoginButtonLabel")] private NSText btnLoginLabel;
    
    [IWUIBinding("#CreditsButtonLabel")] private NSText btnCreditsLabel;
    [IWUIBinding("#SupportButtonLabel")] private NSText btnSupportLabel;
    [IWUIBinding("#RestoreButtonLabel")] private NSText btnRestoreLabel;
    
    [IWUIBinding("#TermsOfUseButtonLabel")] private NSText btnTermsOfUseLabel;
    [IWUIBinding("#PolicyButtonLabel")] private NSText btnPolicyLabel;
    
    [IWUIBinding("#LanguageButton")] private UIButtonViewController btnLanguage;
    
    [IWUIBinding("#SoundButton")] private UIToggleViewController btnSound;
    [IWUIBinding("#MusicButton")] private UIToggleViewController btnMusic;
    
    [IWUIBinding("#VersionButton")] private UIButtonViewController btnVersion;
    
    [IWUIBinding("#LoginButton")] private UIButtonViewController btnLogin;
    
    [IWUIBinding("#CreditsButton")] private UIButtonViewController btnCredits;
    [IWUIBinding("#SupportButton")] private UIButtonViewController btnSupport;
    [IWUIBinding("#RestoreButton")] private UIButtonViewController btnRestore;
    
    [IWUIBinding("#TermsOfUseButton")] private UIButtonViewController btnTermsOfUse;
    [IWUIBinding("#PolicyButton")] private UIButtonViewController btnPolicy;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UISettingsWindowModel;
        
        btnSound.OnChange(OnSoundClick);
        btnMusic.OnChange(OnMusicClick);
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        languageLabel.Text = windowModel.LanguageText;
        soundLabel.Text = windowModel.SoundText;
        musicLabel.Text = windowModel.MusicText;
        
        profileLabel.Text = windowModel.ProfileButtonText;

        btnLanguageLabel.Text = windowModel.LanguageButtonText;
        btnLoginLabel.Text = windowModel.LoginButtonText;
        
        btnCreditsLabel.Text = windowModel.CreditsButtonText;
        btnSupportLabel.Text = windowModel.SupportButtonText;
        btnRestoreLabel.Text = windowModel.RestoreButtonText;
        
        btnTermsOfUseLabel.Text = windowModel.TermsOfUseButtonText;
        btnPolicyLabel.Text = windowModel.PolicyButtonText;
        
#if DEBUG
        versionLabel.Text = $"ver.:{IWProjectVersionSettings.Instance.CurrentVersion}";
#else
        versionLabel.Text = $"ver.:{IWProjectVersionSettings.Instance.ProductionVersion}";
#endif
        
        btnSound.ToState(ProfileService.Current.Settings.GetVolume("Sound") > 0.1f ? GenericButtonState.Active : GenericButtonState.UnActive);
        btnMusic.ToState(ProfileService.Current.Settings.GetVolume("Music") > 0.1f ? GenericButtonState.Active : GenericButtonState.UnActive);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnLanguage, OnLanguageClick);
        
        InitButtonBase(btnLogin, OnLoginClick);
        
        InitButtonBase(btnVersion, OnVersionClick);
        InitButtonBase(btnCredits, OnCreditsClick);
        InitButtonBase(btnSupport, OnSupportClick);
        InitButtonBase(btnRestore, OnRestoreClick);
        
        InitButtonBase(btnTermsOfUse, OnTermsOfUseClick);
        InitButtonBase(btnPolicy, OnPolicyClick);
        
        TackleBoxEvents.SendSettingsOpen();
    }

    
    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UISettingsWindowModel;
        
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        TackleBoxEvents.SendSettingsClosed();
    }

    private bool EditorPlaceholderForBfgServices()
    {
#if UNITY_EDITOR
        UIMessageWindowController.CreateNotImplementedMessage();
        return true;
#endif
        
        return false;
    }
    
    private void OnLoginClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnLoginClick");
    }
    
    private void OnTermsOfUseClick()
    {
        Debug.Log("OnTermsOfUseClick");

        if (NetworkUtils.CheckInternetConnection(true) && !EditorPlaceholderForBfgServices())
        {
            bfgManager.showTerms();
        }
    }
    
    private void OnPolicyClick()
    {
        Debug.Log("OnPolicyClick");

        if (NetworkUtils.CheckInternetConnection(true) && !EditorPlaceholderForBfgServices())
        {
            bfgManager.showPrivacy();
        }
    }
    
    private void OnRestoreClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnRestoreClick");
    }
    
    private void OnVersionClick()
    {
        UIMessageWindowController.CreateVersionMessage();
    }

    
    private void OnCreditsClick()
    {
        UIService.Get.ShowWindow(UIWindowType.CreditsWindow);
        Controller.CloseCurrentWindow();
    }
    
    private void OnSupportClick()
    {
        Debug.Log("OnSupportClick");
        
        if (NetworkUtils.CheckInternetConnection(true) && !EditorPlaceholderForBfgServices())
        {
            bfgManager.showSupport();
        }
    }
    
    private void OnLanguageClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnLanguageClick");
    }
    
    private void OnSoundClick(bool isOn)
    {
        ProfileService.Current.Settings.SetVolume("Sound", isOn ? 1f : 0f);
    }
    
    private void OnMusicClick(bool isOn)
    {
        ProfileService.Current.Settings.SetVolume("Music", isOn ? 1f : 0f);
    }
}
