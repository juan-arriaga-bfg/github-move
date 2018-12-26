//
//  BfgLegacyDeviceManagerWrapper.mm
//  Unity-iPhone
//
//  Created by Michael Molien on 01/04/17.
//  Copyright 2017 Big Fish Games, Inc. All rights reserved.
//


#import <bfg_iOS_sdk/bfgRave.h>
#import "UnityWrapperUtilities.h"

#define RAVE_ID_LENGTH 32
#define EMAIL_ADDRESS_LENGTH 320
#define RAVE_NAME_LENGTH 32

extern "C"
{
    void __bfgRave__currentRaveId( char* returnCurrentRaveId)
    {
        NSString *_currentRaveId = [bfgRave currentRaveId];
        
        if (_currentRaveId == nil)
        {
            return;
        }
        
        NSUInteger length = [_currentRaveId lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        if (length <= RAVE_ID_LENGTH)
        {
            copyStringToBuffer( _currentRaveId, returnCurrentRaveId, (int)length+1 );
        }
    }

    BOOL __bfgRave__isCurrentGuest()
    {
        return [bfgRave isCurrentGuest];
    }

    void __bfgRave__lastRaveId( char* returnLastRaveId )
    {
        NSString *_lastRaveId = [bfgRave lastRaveId];
        
        if (_lastRaveId == nil)
        {
            return;
        }
        
        NSUInteger length = [_lastRaveId lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        if (length <= RAVE_ID_LENGTH)
        {
            copyStringToBuffer( _lastRaveId, returnLastRaveId, (int)length+1 );
        }
    }

    BOOL __bfgRave__isLastGuest()
    {
        return [bfgRave isLastGuest];
    }

    void __bfgRave__logoutCurrentUser()
    {
        [bfgRave logoutCurrentUser];
        return;
    }

    void  __bfgRave__presentSignIn()
    {
        [bfgRave presentSignIn];
        return;
    }

    void __bfgRave__presentProfile()
    {
        [bfgRave presentProfile];
        return;
    }

    BOOL __bfgRave__isRaveInitialized()
    {
        return [bfgRave isRaveInitialized];
    }

    BOOL __bfgRave__isAuthenticated()
    {
        return [bfgRave isCurrentAuthenticated];
    }
    
    void __bfgRave__changeRaveDisplayName(const char* raveDisplayName)
    {
        NSString *nsRaveDisplayName = [NSString stringWithUTF8String:raveDisplayName];
        [bfgRave changeRaveDisplayName:nsRaveDisplayName];
        return;
    }
    
    void __bfgRave__currentRaveDisplayName(char* nameString)
    {
        NSString * nameNSString = [bfgRave currentRaveDisplayName];
        
        if (nameNSString == nil)
        {
            return;
        }
        
        NSUInteger length = [nameNSString lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        // 32 characters is the maximum name length
        if (length <= RAVE_NAME_LENGTH)
        {
            copyStringToBuffer(nameNSString, nameString, (int)length+1);
        }
    }

    void __bfgRave__selectRaveAppDataKey(const char* key)
    {
        NSString *nsKey = [NSString stringWithUTF8String:key];
        [bfgRave selectRaveAppDataKey:nsKey];
        return;
    }

    void __bfgRave__fetchCurrentAppDataKey()
    {
        [bfgRave fetchCurrentAppDataKey];
        return;
    }

    BOOL __bfgRave__isCurrentAuthenticated()
    {
        return [bfgRave isCurrentAuthenticated];
    }

    BOOL __bfgRave__isCurrentPersonalized()
    {
        return [bfgRave isCurrentPersonalized];
    }
    
    void __bfgRave__currentRaveEmail(char* emailString)
    {
        NSString * emailNSString = [bfgRave currentRaveEmail];
        
        if (emailNSString == nil)
        {
            return;
        }
        
        NSUInteger length = [emailNSString lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        // 320 characters is the maximum email address length
        if (length <= EMAIL_ADDRESS_LENGTH)
        {
            copyStringToBuffer(emailNSString, emailString, (int)length+1);
        }
    }

    void __bfgRave__fetchCurrentRaveProfilePicture()
    {
      // Not yet implemented.
    }

    void __bfgRave__presentNewsletterSignup()
    {
        [bfgRave presentNewsletterSignup];
    }

    void __bfgRave__presentNewsletterSignupWithOrigin(const char * origin)
    {
        NSString *nsOrigin = [NSString stringWithUTF8String:origin];
        [bfgRave presentNewsletterSignupWithOrigin:nsOrigin];
    }

    void __bfgRave__presentProfileWithOrigin(const char * origin)
    {
        NSString *nsOrigin = [NSString stringWithUTF8String:origin];
        [bfgRave presentProfileWithOrigin:nsOrigin];
    }

    void __bfgRave__presentSignInWithOrigin(const char * origin)
    {
        NSString *nsOrigin = [NSString stringWithUTF8String:origin];
        [bfgRave presentSignInWithOrigin:nsOrigin];
    }
    
    void __bfgRave__setupLoginStatusWithExternalCallback()
    {
        [bfgRave setupLoginStatusCallbackWithExternalCallback:^(RaveLoginStatus status, NSError *error) {
            if (status == (RaveLoginStatus)RaveLoggedIn){
                NSNotification * notification = [NSNotification notificationWithName:@"BFG_RAVE_EXTERNALCALLBACK_LOGGEDIN" object:nil];
                [[UnityWrapper sharedInstance] handleNotification:notification];
            } else if (status == (RaveLoginStatus)RaveLoggedOut) {
                NSNotification * notification = [NSNotification notificationWithName:@"BFG_RAVE_EXTERNALCALLBACK_LOGGEDOUT" object:nil];
                [[UnityWrapper sharedInstance] handleNotification:notification];
            } else{
                NSNotification * notification = [NSNotification notificationWithName:@"BFG_RAVE_EXTERNALCALLBACK_LOGINERROR" object:error];
                [[UnityWrapper sharedInstance] handleNotification:notification];
            }
        }];
    }
}
