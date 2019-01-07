using UnityEngine;

public class BfgSdkGdprInitComponent : AsyncInitItemBase
{
    public override void Execute()
    {
#if UNITY_EDITOR
        isCompleted = true;
        OnComplete(this);
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
                OnComplete(this);
                return;
            });
    }
}