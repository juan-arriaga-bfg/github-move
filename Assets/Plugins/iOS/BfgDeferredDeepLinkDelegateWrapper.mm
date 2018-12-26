//
//  BfgDeferredDeepLinkDelegateWrapper.mm
//


#import <bfg_iOS_sdk/bfgGameReporting.h>
#import "UnityWrapperUtilities.h"

#define NOTIFICATION_DEFERREDDEEPLINK_DIDRECEIVEDEFERREDDEEPLINK  @"NOTIFICATION_DEFERREDDEEPLINK_DIDRECEIVEDEFERREDDEEPLINK"

@interface BfgDeferredDeepLinkDelegateWrapper : NSObject <bfgDeferredDeepLinkDelegate>
    +(instancetype) sharedInstance;
@end

@implementation BfgDeferredDeepLinkDelegateWrapper

+(instancetype)sharedInstance
{
    static BfgDeferredDeepLinkDelegateWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgDeferredDeepLinkDelegateWrapper alloc] init]; } );
    return _sharedInstance;
}

- (void)didReceiveDeferredDeepLink:(NSString *)deepLinkString error:(NSError *)error provider:(bfgUATrackerId)provider timeSinceLaunch:(NSTimeInterval)timeSinceLaunch
{
    NSString *providerString = @"Unknown";
    switch(provider)
    {
        case bfgUATrackerIdTune: providerString = @"Tune"; break;       // 1
        case bfgUATrackerIdAll: providerString = @"All"; break;         // 2
        default: break;
    }
    
    NSNotification * notification = [[NSNotification alloc] initWithName:NOTIFICATION_DEFERREDDEEPLINK_DIDRECEIVEDEFERREDDEEPLINK
                                                                  object:self
                                                                userInfo:@{
                                                                           @"deepLinkString" : (deepLinkString != nil) ? deepLinkString : @"",
                                                                           @"errorCode" : (error != nil) ? [NSNumber numberWithLong:error.code] : @"",
                                                                           @"errorDomain" : (error && error.domain) ? error.domain : @"",
                                                                           @"provider" : (providerString != nil) ? providerString : @"Unknown",
                                                                           @"timeSinceLaunch" : timeSinceLaunch ? [NSNumber numberWithDouble:timeSinceLaunch] : @"Unknown"}];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

@end


extern "C"
{
    void BfgDeferredDeepLinkDelegateWrapper__setDeferredDeepLinkDelegate()
    {
        [bfgGameReporting setDeferredDeepLinkDelegate:[BfgDeferredDeepLinkDelegateWrapper sharedInstance]];
    }

}
