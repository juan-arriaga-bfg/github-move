using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

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

            if (state)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
        
        protected bool CheckLocalizationData(LocalizationManager localizationManager, string currentLocalization)
        {
            bool state = true;
            
            foreach (var localizationData in localizationManager.StorageItems)
            {
                bool isValidData = LocalizationEditorUtils.IsLocalizationDataValid(localizationData, currentLocalization);

                if (isValidData == false)
                {
                    state = false;
                }
            }
            
            return state;
        }

        

    }

}