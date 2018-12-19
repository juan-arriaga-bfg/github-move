using System;
using UnityEngine;
using System.Collections;

public class UIOptionsWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#VersionLabel")] private NSText versionLabel;
    [IWUIBinding("#ProfileLabel")] private NSText profileLabel;
    [IWUIBinding("#LanguageLabel")] private NSText languageLabel;
    
    [IWUIBinding("#LoginButtonLabel")] private NSText btnLoginLabel;
    [IWUIBinding("#TermsOfUseButtonLabel")] private NSText btnTermsOfUseLabel;
    [IWUIBinding("#PolicyButtonLabel")] private NSText btnPolicyLabel;
    [IWUIBinding("#RestoreButtonLabel")] private NSText btnRestoreLabel;
    [IWUIBinding("#CreditsButtonLabel")] private NSText btnCreditsLabel;
    [IWUIBinding("#SupportButtonLabel")] private NSText btnSupportLabel;
    [IWUIBinding("#LanguageButtonLabel")] private NSText btnLanguageLabel;
    [IWUIBinding("#SoundButtonLabel")] private NSText btnSoundLabel;
    [IWUIBinding("#MusicButtonLabel")] private NSText btnMusicLabel;
    
    [IWUIBinding("#LoginButton")] private UIButtonViewController btnLogin;
    [IWUIBinding("#TermsOfUseButton")] private UIButtonViewController btnTermsOfUse;
    [IWUIBinding("#PolicyButton")] private UIButtonViewController btnPolicy;
    [IWUIBinding("#RestoreButton")] private UIButtonViewController btnRestore;
    [IWUIBinding("#CreditsButton")] private UIButtonViewController btnCredits;
    [IWUIBinding("#SupportButton")] private UIButtonViewController btnSupport;
    [IWUIBinding("#LanguageButton")] private UIButtonViewController btnLanguage;
    
    [IWUIBinding("#SoundButton")] private UIToggleViewController btnSound;
    [IWUIBinding("#MusicButton")] private UIToggleViewController btnMusic;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        InitBtn(btnLogin, OnLoginClick);
        InitBtn(btnTermsOfUse, OnTermsOfUseClick);
        InitBtn(btnPolicy, OnPolicyClick);
        InitBtn(btnRestore, OnRestoreClick);
        InitBtn(btnCredits, OnCreditsClick);
        InitBtn(btnSupport, OnSupportClick);
        InitBtn(btnLanguage, OnLanguageClick);

        btnSound.OnChange(OnSoundClick);
        btnMusic.OnChange(OnMusicClick);
    }

    private void InitBtn(UIButtonViewController btn, Action onClick)
    {
        btn.ToState(GenericButtonState.Active)
            .OnClick(onClick);
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIOptionsWindowModel windowModel = Model as UIOptionsWindowModel;

        btnSound.ToState(ProfileService.Current.Settings.GetVolume("Sound") > 0.1f ? GenericButtonState.Active : GenericButtonState.UnActive);
        btnMusic.ToState(ProfileService.Current.Settings.GetVolume("Music") > 0.1f ? GenericButtonState.Active : GenericButtonState.UnActive);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIOptionsWindowModel windowModel = Model as UIOptionsWindowModel;
        
    }

    private void OnLoginClick()
    {
        Debug.LogWarning("OnLoginClick");
    }
    
    private void OnTermsOfUseClick()
    {
        Debug.LogWarning("OnTermsOfUseClick");
    }
    
    private void OnPolicyClick()
    {
        Debug.LogWarning("OnPolicyClick");
    }
    
    private void OnRestoreClick()
    {
        Debug.LogWarning("OnRestoreClick");
    }
    
    private void OnCreditsClick()
    {
        Debug.LogWarning("OnCreditsClick");
    }
    
    private void OnSupportClick()
    {
        Debug.LogWarning("OnSupportClick");
    }
    
    private void OnLanguageClick()
    {
        Debug.LogWarning("OnLanguageClick");
    }
    
    private void OnSoundClick(bool isOn)
    {
        var currentVolume = ProfileService.Current.Settings.GetVolume("Sound");
        ProfileService.Current.Settings.SetVolume("Sound", isOn ? 1f : 0f);
    }
    
    private void OnMusicClick(bool isOn)
    {
        var currentVolume = ProfileService.Current.Settings.GetVolume("Music");
        ProfileService.Current.Settings.SetVolume("Music", isOn ? 1f : 0f);
    }
}
