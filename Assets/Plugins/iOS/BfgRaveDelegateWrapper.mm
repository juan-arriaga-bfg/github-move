//
//  BfgRaveDelegateWrapper.mm
//
//  Created by Alex Bowns on 3/15/18.
//

#include "BfgRaveDelegateWrapper.h"


#import <bfg_iOS_sdk/bfg_iOS_sdk.h>
#import "UnityWrapperUtilities.h"

#define BFG_RAVE_PROFILE_FAILED_WITH_ERROR  @"BFG_RAVE_PROFILE_FAILED_WITH_ERROR"
#define BFG_RAVE_PROFILE_SUCCEEDED @"BFG_RAVE_PROFILE_SUCCEEDED"
#define BFG_RAVE_PROFILE_CANCELLED @"BFG_RAVE_PROFILE_CANCELLED"
#define BFG_RAVE_SIGN_IN_COPPA_TRUE @"BFG_RAVE_SIGN_IN_COPPA_TRUE"
#define BFG_RAVE_SIGN_IN_COPPA_FALSE @"BFG_RAVE_SIGN_IN_COPPA_FALSE"
#define BFG_RAVE_SIGN_IN_CANCELLED @"BFG_RAVE_SIGN_IN_CANCELLED"
#define BFG_RAVE_SIGN_IN_SUCCEEDED @"BFG_RAVE_SIGN_IN_SUCCEEDED"
#define BFG_RAVE_SIGN_IN_FAILED_WITH_ERROR @"BFG_RAVE_SIGN_IN_FAILED_WITH_ERROR"
#define BFG_RAVE_USER_DID_LOGIN @"BFG_RAVE_USER_DID_LOGIN"
#define BFG_RAVE_USER_DID_LOGOUT @"BFG_RAVE_USER_DID_LOGOUT"
#define BFG_RAVE_USER_LOGIN_ERROR @"BFG_RAVE_USER_LOGIN_ERROR"
#define BFG_RAVE_CHANGE_DISPLAY_NAME_DID_SUCCEED @"BFG_RAVE_CHANGE_DISPLAY_NAME_DID_SUCCEED"
#define BFG_RAVE_CHANGE_DISPLAY_NAME_DID_FAIL_WITH_ERROR @"BFG_RAVE_CHANGE_DISPLAY_NAME_DID_FAIL_WITH_ERROR"
#define BFG_RAVE_READY @"BFG_RAVE_READY"

#define RAVE_ADK_DELEGATE_ADK_DID_CHANGE @"RAVE_ADK_DELEGATE_ADK_DID_CHANGE"
#define RAVE_ADK_DELEGATE_ADK_UNRESOLVED_KEYS @"RAVE_ADK_DELEGATE_ADK_UNRESOLVED_KEYS"
#define RAVE_ADK_DELEGATE_FETCH_CURRENT_SUCCEEDED @"RAVE_ADK_DELEGATE_FETCH_CURRENT_SUCCEEDED"
#define RAVE_ADK_DELEGATE_FETCH_CURRENT_FAILED @"RAVE_ADK_DELEGATE_FETCH_CURRENT_FAILED"
#define RAVE_ADK_DELEGATE_ADK_SELECT_SUCCEEDED @"RAVE_ADK_DELEGATE_ADK_SELECT_SUCCEEDED"
#define RAVE_ADK_DELEGATE_ADK_SELECT_FAILED @"RAVE_ADK_DELEGATE_ADK_SELECT_FAILED"

@interface BfgRaveDelegateWrapper : NSObject <bfgRaveDelegate, bfgRaveAppDataKeyDelegate>
+(instancetype) sharedInstance;
@end

@implementation BfgRaveDelegateWrapper
+(instancetype)sharedInstance
{
    static BfgRaveDelegateWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgRaveDelegateWrapper alloc] init]; } );
    return _sharedInstance;
}


#pragma mark - bfgRaveDelegate delegates
- (void)bfgRaveProfileFailedWithError:(NSError * _Nullable)error
{
    NSNotification * notification = [[NSNotification alloc] initWithName:BFG_RAVE_PROFILE_FAILED_WITH_ERROR
                                                                  object:self
                                                                userInfo:@{
                                                                           @"errorCode" : (error != nil) ? [NSNumber numberWithLong:error.code] : @"",
                                                                           @"errorDomain" : (error != nil && error.domain) ? error.domain : @"" }];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveProfileSucceeded
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_PROFILE_SUCCEEDED object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveProfileCancelled
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_PROFILE_CANCELLED object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveSignInCOPPAResult:(BOOL)passedCOPPA
{
    NSNotification * notification;
    if (passedCOPPA == true)
    {
        notification = [NSNotification notificationWithName:BFG_RAVE_SIGN_IN_COPPA_TRUE object:self];
    } else {
        notification = [NSNotification notificationWithName:BFG_RAVE_SIGN_IN_COPPA_FALSE object:self];
    }
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveSignInSucceeded
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_SIGN_IN_SUCCEEDED object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveSignInCancelled
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_SIGN_IN_CANCELLED object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveSignInFailedWithError:(NSError * _Nullable)error
{
    NSNotification * notification = [[NSNotification alloc] initWithName:BFG_RAVE_SIGN_IN_FAILED_WITH_ERROR
                                                                  object:self
                                                                userInfo:@{
                                                                           @"errorCode" : (error != nil) ? [NSNumber numberWithLong:error.code] : @"",
                                                                           @"errorDomain" : (error != nil && error.domain) ? error.domain : @"" }];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveUserDidLogin
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_USER_DID_LOGIN object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveUserDidLogout
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_USER_DID_LOGOUT object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveUserLoginError:(NSError * _Nullable)loginError
{
    NSNotification * notification = [[NSNotification alloc] initWithName:BFG_RAVE_USER_LOGIN_ERROR
                                                                  object:self
                                                                userInfo:@{
                                                                           @"errorCode" : (loginError != nil) ? [NSNumber numberWithLong:loginError.code] : @"",
                                                                           @"errorDomain" : (loginError != nil && loginError.domain) ? loginError.domain : @"" }];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveChangeDisplayNameDidSucceed
{
    NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_CHANGE_DISPLAY_NAME_DID_SUCCEED object:self];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

- (void)bfgRaveChangeDisplayNameDidFailWithError:(NSError * _Nullable)error
{
    NSNotification * notification = [[NSNotification alloc] initWithName:BFG_RAVE_CHANGE_DISPLAY_NAME_DID_FAIL_WITH_ERROR
                                                                  object:self
                                                                userInfo:@{
                                                                           @"errorCode" : (error != nil) ? [NSNumber numberWithLong:error.code] : @"",
                                                                           @"errorDomain" : (error != nil && error.domain) ? error.domain : @"" }];
    
    UnityWrapper * unity_wrapper = [UnityWrapper sharedInstance];
    [unity_wrapper handleNotification:notification];
}

#pragma mark - bfgRaveAppDataKeyDelegate delegates
- (void)bfgRaveAppDataKeyDidChange:(NSString *)currentAppDataKey previousKey:(NSString *)previousKey
{
    NSNotification * notification = [[NSNotification alloc] initWithName:RAVE_ADK_DELEGATE_ADK_DID_CHANGE
                                                                  object:self
                                                                userInfo:@{
                                                                           @"currentAppDataKey" : (currentAppDataKey != nil) ? currentAppDataKey : @"",
                                                                           @"previousKey" : (previousKey != nil) ? previousKey : @""
                                                                           }];
    [[UnityWrapper sharedInstance] handleNotification:notification];
}

- (void)bfgRaveAppDataKeyDidReturnUnresolvedKeys:(NSArray<NSString *> *)unresolvedKeys currentAppDataKey:(NSString *)currentAppDataKey
{
    NSNotification * notification = [[NSNotification alloc] initWithName:RAVE_ADK_DELEGATE_ADK_UNRESOLVED_KEYS
                                                                  object:self
                                                                userInfo:@{
                                                                           @"unresolvedKeys" : (unresolvedKeys != nil) ? unresolvedKeys : @[@""],
                                                                           @"currentAppDataKey" : (currentAppDataKey != nil) ? currentAppDataKey : @""
                                                                           }];
    [[UnityWrapper sharedInstance] handleNotification:notification];
}

- (void)bfgRaveSelectAppDataKeyDidFailWithError:(nonnull NSError *)error
{
    NSNotification * notification = [[NSNotification alloc] initWithName:RAVE_ADK_DELEGATE_ADK_SELECT_FAILED
                                                                  object:self
                                                                userInfo:@{
                                                                           @"errorCode" : (error != nil) ? [NSNumber numberWithLong:error.code] : [NSNumber numberWithLong:-1],
                                                                           @"errorDescription" : (error != nil && error.localizedDescription) ? error.localizedDescription : @""
                                                                           }];
    [[UnityWrapper sharedInstance] handleNotification:notification];
}

- (void)bfgRaveSelectAppDataKeyDidSucceed
{
    NSNotification * notification = [[NSNotification alloc] initWithName:RAVE_ADK_DELEGATE_ADK_SELECT_SUCCEEDED
                                                                  object:self
                                                                userInfo:nil
                                     ];
    [[UnityWrapper sharedInstance] handleNotification:notification];
}

- (void)bfgRaveFetchCurrentAppDataKeyDidSucceed:(NSString *)currentAppDataKey
{
    NSNotification * notification = [[NSNotification alloc] initWithName:RAVE_ADK_DELEGATE_FETCH_CURRENT_SUCCEEDED
                                                                  object:self
                                                                userInfo:@{
                                                                           @"currentAppDataKey" : (currentAppDataKey != nil) ? currentAppDataKey : @""
                                                                           }];
    [[UnityWrapper sharedInstance] handleNotification:notification];
}

- (void)bfgRaveFetchCurrentAppDataKeyDidFailWithError:(NSError *)error
{
    NSNotification * notification = [[NSNotification alloc] initWithName:RAVE_ADK_DELEGATE_FETCH_CURRENT_FAILED
                                                                  object:self
                                                                userInfo:@{
                                                                           @"errorCode" : (error != nil) ? [NSNumber numberWithLong:error.code] : [NSNumber numberWithLong:-1],
                                                                           @"errorDescription" : (error != nil && error.localizedDescription) ? error.localizedDescription : @""
                                                                           }];
    [[UnityWrapper sharedInstance] handleNotification:notification];
}

@end

extern "C"
{
    void BfgRaveDelegateWrapper__setRaveDelegate()
    {
        [bfgRave setDelegate:[BfgRaveDelegateWrapper sharedInstance]];
    }
    
    void BfgRaveDelegateWrapper__setupLoginStatusWithExternalCallback()
    {
        [bfgRave setupLoginStatusCallbackWithExternalCallback:^(RaveLoginStatus status, NSError *error) {
            if (status == (RaveLoginStatus)RaveLoggedIn){
                NSNotification * notification = [NSNotification notificationWithName:@"BFG_RAVE_EXTERNALCALLBACK_LOGGEDIN" object:nil userInfo:nil];
                [[UnityWrapper sharedInstance] handleNotification:notification];
            } else if (status == (RaveLoginStatus)RaveLoggedOut) {
                NSNotification * notification = [NSNotification notificationWithName:@"BFG_RAVE_EXTERNALCALLBACK_LOGGEDOUT" object:nil userInfo:nil];
                [[UnityWrapper sharedInstance] handleNotification:notification];
            } else{
                NSNotification * notification = [NSNotification notificationWithName:@"BFG_RAVE_EXTERNALCALLBACK_LOGINERROR" object:error userInfo:nil];
                [[UnityWrapper sharedInstance] handleNotification:notification];
            }
        }];
    }
    
    void BfgRaveDelegateWrapper__setRaveReadyListener()
    {
        [bfgRave listenForRaveReady:^(NSString * _Nullable raveId, NSError * _Nullable error)
         {
             
             id objects[]  = {@"", @""};
             id keys[] = {@"error", @"raveId"};
             NSUInteger count = sizeof(objects)/sizeof(id);
             
             if (error != nil)
             {
                 objects[0] = error;
             }
             if (raveId != nil)
             {
                 objects[1] = raveId;
             }
             NSDictionary * dict = [NSDictionary dictionaryWithObjects: objects
                                                               forKeys: keys
                                                                 count: count];
             
             NSNotification * notification = [NSNotification notificationWithName:BFG_RAVE_READY object:nil userInfo:dict];
             [[UnityWrapper sharedInstance] handleNotification:notification];
         }];
    }
    
    void BfgRaveDelegateWrapper__setRaveAppDataKeyDelegate()
    {
        [bfgRave setRaveAppDataKeyDelegate:[BfgRaveDelegateWrapper sharedInstance]];
    }
}


