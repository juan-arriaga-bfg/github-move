using UnityEngine;

public class BfgSdkGdprInitComponent : AsyncInitComponent
{
    private bool isCompleted;
    
    public override bool IsCompleted => isCompleted;

    public override void OnRegisterEntity(ECSEntity entity)
    {
#if UNITY_EDITOR
        isCompleted = true;
        return;
#endif
        
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