//
//  BfgPlacementDelegateWrapper.mm
//


#import <bfg_iOS_sdk/bfgGameReporting.h>
#import "UnityWrapperUtilities.h"

#define NOTIFICATION_PLACEMENTCONTENT_DIDDISMISS  @"NOTIFICATION_PLACEMENTCONTENT_DIDDISMISS"
#define NOTIFICATION_PLACEMENTCONTENT_DIDOPEN     @"NOTIFICATION_PLACEMENTCONTENT_DIDOPEN"
#define NOTIFICATION_PLACEMENTCONTENT_ERROR       @"NOTIFICATION_PLACEMENTCONTENT_ERROR"
#define NOTIFICATION_PLACEMENTCONTENT_WILLDISMISS @"NOTIFICATION_PLACEMENTCONTENT_WILLDISMISS"
#define NOTIFICATION_PLACEMENTCONTENT_WILLOPEN    @"NOTIFICATION_PLACEMENTCONTENT_WILLOPEN"
#define NOTIFICATION_PLACEMENT_REWARDRECEIVED     @"NOTIFICATION_PLACEMENT_REWARDRECEIVED"
#define NOTIFICATION_PLACEMENT_STARTPURCHASE      @"NOTIFICATION_PLACEMENT_STARTPURCHASE"
#define NOTIFICATION_PLACEMENT_DIDRECEIVEDATASTRING @"NOTIFICATION_PLACEMENT_DIDRECEIVEDATASTRING"

@interface BfgPlacementDelegateWrapper : NSObject <bfgPlacementDelegate>
    +(instancetype) sharedInstance;
@end

@implementation BfgPlacementDelegateWrapper

+(instancetype)sharedInstance
{
    static BfgPlacementDelegateWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgPlacementDelegateWrapper alloc] init]; } );
    return _sharedInstance;
}

- (void) bfgPlacementContentDidDismiss:(NSString *)placementKey
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENTCONTENT_DIDDISMISS object:self userInfo:@{ @"placementKey" : placementKey ? placementKey : @"" }];
}

- (void) bfgPlacementContentDidOpen:(NSString *)placementKey
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENTCONTENT_DIDOPEN object:self userInfo:@{ @"placementKey" : placementKey ? placementKey : @"" }];
}

- (void) bfgPlacementContentError:(NSString *)contentName error:(NSError *)error
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENTCONTENT_ERROR object:self userInfo:@{
                    @"contentName" : contentName ? contentName : @"",
                      @"errorCode" : error ? [NSNumber numberWithLong:error.code] : @"",
                    @"errorDomain" : (error && error.domain) ? error.domain : @""}];
}

- (void) bfgPlacementContentWillDismiss:(NSString *)placementKey
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENTCONTENT_WILLDISMISS object:self userInfo:@{ @"placementKey" : placementKey ? placementKey : @"" }];
}

- (void) bfgPlacementContentWillOpen:(NSString *)placementKey
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENTCONTENT_WILLOPEN object:self userInfo:@{ @"placementKey" : placementKey ? placementKey : @"" }];
}

- (void) bfgPlacementRewardReceived:(NSString *)placementKey
                         rewardName:(NSString *)rewardName
                     rewardQuantity:(NSNumber *)rewardQuantity
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENT_REWARDRECEIVED object:self userInfo:@{
                   @"placementKey" : placementKey ? placementKey : @"",
                     @"rewardName" : rewardName ? rewardName : @"",
                   @"rewardQuantity" : rewardQuantity ? rewardQuantity : @""}];
}

// Handler for NOTIFICATION_PLACEMENT_STARTPURCHASE should start the purchase flow
- (BOOL) bfgPlacementStartPurchase:(NSString *)placementKey
                         productId:(NSString *)productId
                   productQuantity:(NSNumber *)productQuantity
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENT_STARTPURCHASE object:self userInfo:@{
                   @"placementKey" : placementKey ? placementKey : @"",
                      @"productId" : productId ? productId : @"",
                   @"productQuantity" : productQuantity ? productQuantity : @""}];

    // SDK will not start purchase flow
    return YES;
}

// Handler for NOTIFICATION_PLACEMENT_DIDRECEIVEDATASTRING. Allows games to receive data payloads through interstitials.
- (BOOL) bfgPlacementDidReceiveDataString:(NSString *) dataString
                         scenarioId:(NSString *)scenarioId
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PLACEMENT_DIDRECEIVEDATASTRING object:self userInfo:@{
                                                                                                                           @"dataString" : dataString ? dataString : @"",
                                                                                                                           @"scenarioId" : scenarioId ? scenarioId : @""}];
    return YES;
}

@end


extern "C"
{
    void BfgPlacementDelegateWrapper__setPlacementDelegate()
    {
        [bfgGameReporting setPlacementDelegate:[BfgPlacementDelegateWrapper sharedInstance]];
    }
}
