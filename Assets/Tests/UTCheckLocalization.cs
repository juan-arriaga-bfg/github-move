using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace UT
{
    public class UTCheckLocalization
    {

        [Test]
        public void UTCheckLocalizationPass()
        {
            bool state = true;
            
            LocalizationManager localizationManager = new BaseLocalizationManager();
            LocalizationService.Instance.SetManager(localizationManager);
            localizationManager.SupportedLanguages = NSLocalizationSettings.Instance.SupportedLanguages;

            for (int i = 0; i < localizationManager.SupportedLanguages.Count; i++)
            {
                var langName = localizationManager.SupportedLanguages[i];
                localizationManager.SwitchLocalization(langName);

                bool isValid = CheckLocalizationData(localizationManager, langName);

                if (isValid == false)
                {
                    state = false;
                }
                
                Debug.LogWarning($"[Check Localization] => Localization {langName} checked status:{isValid}");
            }
            
            Assert.IsTrue(state);
        }
        
        protected bool CheckLocalizationData(LocalizationManager localizationManager, string currentLocalization)
        {
            bool state = true;
            
            foreach (var localizationData in localizationManager.StorageItems)
            {
                bool isValidData = IsLocalizationDataValid(localizationData, currentLocalization);

                if (isValidData == false)
                {
                    state = false;
                }
            }
            
            return state;
        }

        protected bool IsLocalizationDataValid(LocalizationData localizationData, string currentLocalization)
        {
            bool state = true;

            if (string.IsNullOrEmpty(localizationData.Uid))
            {
                state = false;
                Debug.LogWarning($"[Check Localization] => empty KEY for:{localizationData.Uid} lang:{currentLocalization}");
            }
            else if (string.IsNullOrEmpty(localizationData.Text))
            {
                state = false;
                Debug.LogWarning($"[Check Localization] => empty VALUE for:{localizationData.Uid} lang:{currentLocalization}");
            }
            else if (localizationData.Uid.Contains("\""))
            {
                state = false;
                Debug.LogWarning($"[Check Localization] => KEY has symbol '\' for:{localizationData.Uid} lang:{currentLocalization}");
            }
            else if (localizationData.Uid.Contains("&"))
            {
                // replace to &amp;
                state = false;
                Debug.LogWarning($"[Check Localization] => KEY has symbol '\' for:{localizationData.Uid} lang:{currentLocalization}");
            }
            else if (localizationData.Text.Contains("\""))
            {
                // replace to &quot;
                Debug.LogWarning($"[Check Localization] => VALUE has symbol '\' for:{localizationData.Uid} lang:{currentLocalization}");
            }
            else if (localizationData.Uid.Contains(" "))
            {
                // replace to (remove)
                Debug.LogWarning($"[Check Localization] => KEY has symbol 'space' for:{localizationData.Uid} lang:{currentLocalization}");
            }

            if (currentLocalization.Equals(SystemLanguage.Russian.ToString()) == false)
            {
                Regex wrongSymbols = new Regex(@"S""[\p{IsCyrillic}\p{P}\p{N}\s]*""");
                
                if (wrongSymbols.IsMatch(localizationData.Uid))
                {
                    Debug.LogWarning($"[Check Localization] => KEY has wrong symbol [{wrongSymbols}] for:{localizationData.Uid} lang:{currentLocalization}");
                }
                else if (wrongSymbols.IsMatch(localizationData.Text))
                {
                    Debug.LogWarning($"[Check Localization] => KEY has wrong symbol [{wrongSymbols}] for:{localizationData.Uid} lang:{currentLocalization}");
                }
                else if (localizationData.Text.ToLower().Contains("с"))
                {
                    // replace to C,c (english);
                    Debug.LogWarning($"[Check Localization] => VALUE has symbol 'c or C (ascii)' for:{localizationData.Uid} lang:{currentLocalization}");
                }
                else if (localizationData.Text.ToLower().Contains("х"))
                {
                    // replace to x,X (english);
                    Debug.LogWarning($"[Check Localization] => VALUE has symbol 'х or Х (ascii)' for:{localizationData.Uid} lang:{currentLocalization}");
                }
            }
            
            
            return state;
        }

    }

}