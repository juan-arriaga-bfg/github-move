//
//  BfgPolicyListenerWrapper.mm
//  Unity-iPhone
//
//  Created by Alex Bowns on 5/11/18.
//
#import <bfg_iOS_sdk/bfgManager.h>
#import "UnityWrapperUtilities.h"

#define BFG_POLICY_LISTENER_WILLSHOWPOLICIES  @"BFG_POLICY_LISTENER_WILLSHOWPOLICIES"
#define BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED  @"BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED"

@interface BfgPolicyListenerWrapper : NSObject
+(instancetype) sharedInstance;
@end

@implementation BfgPolicyListenerWrapper
+(instancetype)sharedInstance
{
    static BfgPolicyListenerWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgPolicyListenerWrapper alloc] init]; } );
    return _sharedInstance;
}

-(void) addPolicyListener
{
    void (^willShowPoliciesBlock)(void)= ^{
        NSNotification * notification = [NSNotification notificationWithName:@"BFG_POLICY_LISTENER_WILLSHOWPOLICIES" object:nil];
        [[UnityWrapper sharedInstance] handleNotification:notification];
    };

    void (^onPoliciesCompletedBlock)(void)= ^{
        NSNotification * notification = [NSNotification notificationWithName:@"BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED" object:nil];
        [[UnityWrapper sharedInstance] handleNotification:notification];
    };

    [bfgManager addPolicyListener:self willShowPolicies:willShowPoliciesBlock onPoliciesCompleted:onPoliciesCompletedBlock];
}

-(void) removePolicyListener
{
    [bfgManager removePolicyListener:self];
}

@end
extern "C"
{
    void BfgPolicyListenerWrapper__setBfgPolicyListener()
    {
        [[BfgPolicyListenerWrapper sharedInstance] addPolicyListener];
    }
    
    void BfgPolicyListenerWrapper__removeBfgPolicyListener()
    {
        [[BfgPolicyListenerWrapper sharedInstance] removePolicyListener];
    }
}
