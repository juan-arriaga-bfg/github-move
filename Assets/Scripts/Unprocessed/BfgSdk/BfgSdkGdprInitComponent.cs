using UnityEngine;

public class BfgSdkGdprInitComponent : ThirdPartyInitComponent
{
    private bool isCompleted;
    
    public override bool IsCompleted => isCompleted;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        bfgManager.addPolicyListener(
            willShowPolicies: (policiesToShow) =>
            {
                Debug.Log("BfgPolicyListner: willShowPolicies");
            },
            onPoliciesCompleted: (policiesCompleted) =>
            {
                bool adsConsent = bfgManager.didAcceptPolicyControl(bfgCommon.THIRDPARTYTARGETEDADVERTISING);

                // FB
                bfgManager.setLimitEventAndDataUsage(!adsConsent);

                // // ADS
                // if (AdvertisingService.Current != null)
                // {
                //     AdvertisingService.Current.SetConsent();
                // }
                //
                // // GA
                // GameAnalyticsToggler.UpdateState();

                isCompleted = true;
            });
    }
}