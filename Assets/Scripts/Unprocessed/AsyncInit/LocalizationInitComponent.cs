using UnityEngine;

public class LocalizationInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        LocalizationManager localizationManager = new BaseLocalizationManager();
        LocalizationService.Instance.SetManager(localizationManager);
        localizationManager.SupportedLanguages = NSLocalizationSettings.Instance.SupportedLanguages;

        if (localizationManager.IsLanguageSupported(ProfileService.Current.Settings.Language))
        {
            localizationManager.SwitchLocalization(ProfileService.Current.Settings.Language);
        }
        else
        {
            ProfileService.Current.Settings.Language = SystemLanguage.English.ToString();
            localizationManager.SwitchLocalization(ProfileService.Current.Settings.Language);
        }

        isCompleted = true;
        OnComplete(this);
    }
}