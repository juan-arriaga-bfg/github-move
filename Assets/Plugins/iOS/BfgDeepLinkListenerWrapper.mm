//
//  BfgDeepLinkDelegateWrapper.mm
//


#import <bfg_iOS_sdk/bfgGameReporting.h>
#import "UnityWrapperUtilities.h"

#define NOTIFICATION_DEEPLINK_ONDEEPLINKRECEIVED  @"NOTIFICATION_DEEPLINK_ONDEEPLINKRECEIVED"

@interface BfgDeepLinkListenerWrapper : NSObject <bfgDeepLinkListener>
+(instancetype) sharedInstance;
@end

@implementation BfgDeepLinkListenerWrapper

+(instancetype)sharedInstance
{
    static BfgDeepLinkListenerWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgDeepLinkListenerWrapper alloc] init]; } );
    return _sharedInstance;
}

- (void)onDeepLinkReceived:(NSString * _Nullable)deepLinkString conversionData:(NSDictionary<NSString*,NSString*> * _Nullable)conversionData
                     error:(NSError * _Nullable)error
{
    NSMutableDictionary * deepLinkDictionary;
    [deepLinkDictionary addEntriesFromDictionary:conversionData];
    [deepLinkDictionary setObject:deepLinkString forKey:@"deepLinkString"];
    [deepLinkDictionary setObject:error forKey:@"error"];
    NSNotification * notification = [[NSNotification alloc] initWithName:NOTIFICATION_DEEPLINK_ONDEEPLINKRECEIVED
                                                                  object:self
                                                                userInfo:deepLinkDictionary];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

@end


extern "C"
{
    void BfgDeepLinkDelegateWrapper__setDeepLinkListener()
    {
        [bfgGameReporting setDeepLinkListener:[BfgDeepLinkListenerWrapper sharedInstance]];
    }
    
}

