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
        
        InitButtonBase(btnCredits, OnCreditsClick);
        InitButtonBase(btnSupport, OnSupportClick);
        InitButtonBase(btnRestore, OnRestoreClick);
        
        InitButtonBase(btnTermsOfUse, OnTermsOfUseClick);
        InitButtonBase(btnPolicy, OnPolicyClick);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UISettingsWindowModel;
        
    }
    
    private void OnLoginClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnLoginClick");
    }
    
    private void OnTermsOfUseClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnTermsOfUseClick");
    }
    
    private void OnPolicyClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnPolicyClick");
    }
    
    private void OnRestoreClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnRestoreClick");
    }
    
    private void OnCreditsClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnCreditsClick");
    }
    
    private void OnSupportClick()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        Debug.LogWarning("OnSupportClick");
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
