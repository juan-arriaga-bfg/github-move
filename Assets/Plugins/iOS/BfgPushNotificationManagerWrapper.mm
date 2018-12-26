//
//  BfgPushNotificationManagerWrapper.mm
//


#import <bfg_iOS_sdk/bfgPushNotificationManager.h>
#import "UnityWrapperUtilities.h"


#define NOTIFICATION_PUSHNOTIFICATION_DIDRECEIVEPUSHNOTIFICATIONWITHPAYLOAD_WHILEAPPINFOREGROUND  @"NOTIFICATION_PUSHNOTIFICATION_DIDRECEIVEPUSHNOTIFICATIONWITHPAYLOAD_WHILEAPPINFOREGROUND"

@interface BfgPushNotificationDelegateWrapper : NSObject <bfgPushNotificationDelegate>
    +(instancetype) sharedInstance;
@end

@implementation BfgPushNotificationDelegateWrapper

+(instancetype)sharedInstance
{
    static BfgPushNotificationDelegateWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgPushNotificationDelegateWrapper alloc] init]; } );
    return _sharedInstance;
}

- (void) didReceivePushNotificationWithPayload : (NSDictionary *_Nullable) payload
                          whileAppInForeground : (BOOL) appInForeground 
{
    NSString *payloadJson = @"";
    
    if (payload)
    {
        payloadJson = convertJSONObjectToString(payload);
    }
    
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_PUSHNOTIFICATION_DIDRECEIVEPUSHNOTIFICATIONWITHPAYLOAD_WHILEAPPINFOREGROUND object:self userInfo:@{
                              @"payload" : payloadJson,
                      @"appInForeground" : [NSNumber numberWithBool:appInForeground]}];
}

@end

extern "C"
{
    void __bfgPushNotificationManager__registerForPushNotifications()
    {
        [bfgPushNotificationManager registerForPushNotifications];
    }

    void __bfgPushNotificationManager__setIconBadgeNumber(int badgeNumber)
    {
        [bfgPushNotificationManager setIconBadgeNumber:badgeNumber];
    }

    void BfgPushNotificationManager__setPushNotificationDelegate()
    {
        [bfgPushNotificationManager setPushNotificationDelegate:[BfgPushNotificationDelegateWrapper sharedInstance]];
    }
}