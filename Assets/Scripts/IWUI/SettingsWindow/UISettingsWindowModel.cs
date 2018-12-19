public class UISettingsWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.settings.title", "window.settings.title");
    public string Message => LocalizationService.Get("window.settings.message", "window.settings.message");
    
    public string LanguageText => LocalizationService.Get("window.settings.language", "window.settings.language");
    public string SoundText => LocalizationService.Get("window.settings.sound", "window.settings.sound");
    public string MusicText => LocalizationService.Get("window.settings.music", "window.settings.music");
    
    public string LanguageButtonText => LocalizationService.Get("window.settings.button.language", "window.settings.button.language");
    
    public string ProfileButtonText => LocalizationService.Get("window.settings.button.profile", "window.settings.button.profile");
    public string LoginButtonText => LocalizationService.Get("window.settings.button.login", "window.settings.button.login");
    
    public string CreditsButtonText => LocalizationService.Get("window.settings.button.credits", "window.settings.button.credits");
    public string SupportButtonText => LocalizationService.Get("window.settings.button.support", "window.settings.button.support");
    public string RestoreButtonText => LocalizationService.Get("window.settings.button.restore", "window.settings.button.restore");
    
    public string TermsOfUseButtonText => LocalizationService.Get("window.settings.button.termsOfUse", "window.settings.button.termsOfUse");
    public string PolicyButtonText => LocalizationService.Get("window.settings.button.policy", "window.settings.button.policy");
}
