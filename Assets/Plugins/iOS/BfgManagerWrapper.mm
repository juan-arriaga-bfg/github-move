//
//  BfgManagerWrapper.mm
//  Unity-iPhone
//
//  Created by John Starin on 10/2/13.
//  Copyright 2013 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgManager.h>
#import "UnityWrapperUtilities.h"
#import "BfgPolicyListenerWrapper.h"
#import "bfg_iOS_sdk/FBSDKSettings.h"

extern "C"
{
    CGRect makeCGRect(char * buttonBounds)
    {
        NSDictionary* buttonBoundsDictionary = convertJSONtoDictionary(buttonBounds);
        id x = [buttonBoundsDictionary valueForKey:@"x"];
        id y = [buttonBoundsDictionary valueForKey:@"y"];
        id w = [buttonBoundsDictionary valueForKey:@"width"];
        id h = [buttonBoundsDictionary valueForKey:@"height"];
        
        CGRect buttonRect = CGRectMake((CGFloat)[x floatValue],(CGFloat)[y floatValue],(CGFloat)[w floatValue],(CGFloat)[h floatValue]);
        return buttonRect;
    }
    
    void _setParentViewController()
    {
        if( [bfgManager isInitialized]) {
            [bfgManager setParentViewController:UIApplication.sharedApplication.keyWindow.rootViewController];
        }
        else {
            [bfgManager startWithParentViewController:UIApplication.sharedApplication.keyWindow.rootViewController];
        }
    }
    
    //  Deprecated in  Big Fish iOS SDK 5.10
    /*
     BOOL _coppaOptOut()
     {
     return [bfgManager coppaOptOut];
     }
     
     void _setCoppaOptOut(bool yesOrNo)
     {
     [bfgManager setCoppaOptOut:yesOrNo];
     }
     */
    
    long _userID()
    {
        return [[bfgManager userID] longValue];
    }
    
    void _setUserID(long userID)
    {
        NSNumber *user = [NSNumber numberWithLong:userID];
        [bfgManager setUserID:user];
    }
    
    /*
     BOOL _canShowTellAFriendButton()
     {
     return [bfgManager canShowTellAFriendButton];
     }
     
     BOOL _showTellAFriend()
     {
     return [bfgManager showTellAFriend];
     }
     */
    
    BOOL _launchSDKByURLScheme(const char* urlScheme)
    {
        NSString *url = [NSString stringWithUTF8String:urlScheme];
        return [bfgManager launchSDKByURLScheme:url];
    }
    
    //    int _currentUIType()
    //    {
    //        return [[bfgManager currentUIType] intValue];
    //    }
    
    long _sessionCount()
    {
        return [bfgManager sessionCount];
    }
    
    BOOL _isInitialLaunch()
    {
        return [bfgManager isInitialLaunch];
    }
    
    BOOL _isFirstTime()
    {
        return [bfgManager isFirstTime];
    }
    
    BOOL _isInitialized()
    {
        return [bfgManager isInitialized];
    }
    
    void _showMoreGames()
    {
        [bfgManager showMoreGames];
    }
    
    /* Remove in 5.3
     void _removeMoreGames()
     {
     [bfgManager removeMoreGames];
     }
     */
    void _showSupport()
    {
        [bfgManager showSupport];
    }
    
    void _showPrivacy()
    {
        [bfgManager showPrivacy];
    }
    
    void _showTerms()
    {
        [bfgManager showTerms];
    }
    
    void _showWebBrowser(const char* startPage)
    {
        NSString *url = [NSString stringWithUTF8String:startPage];
        [bfgManager showWebBrowser:url];
    }
    
    void _removeWebBrowser()
    {
        [bfgManager removeWebBrowser];
    }
    
    BOOL _checkForInternetConnection()
    {
        return [bfgManager checkForInternetConnection];
    }
    
    BOOL _checkForInternetConnectionAndAlert(BOOL displayAlert)
    {
        return [bfgManager checkForInternetConnectionAndAlert:displayAlert];
    }
    
    BOOL _startBranding()
    {
        return [bfgManager startBranding];
    }
    
    void _stopBranding()
    {
        [bfgManager stopBranding];
    }
    
    void _addPauseResumeDelegate()
    {
        //    [bfgManager addPauseResumeDelegate];
    }
    
    void _removePauseResumeDelegate()
    {
        //    [bfgManager removePauseResumeDelegate];
    }
    
    BOOL _isPaused()
    {
        return [bfgManager isPaused];
    }
    
    BOOL _createCCSButtonBounds(float widthPercent, char * horizontalAnchor, char * verticalAnchor, char * returnButtonBounds)
    {
        NSString *nsHorizontalAnchor = [NSString stringWithUTF8String:horizontalAnchor].uppercaseString;
        NSString *nsVerticalAnchor = [NSString stringWithUTF8String:verticalAnchor].uppercaseString;
        bfgAnchorLocation horizontalEnum;
        bfgAnchorLocation verticalEnum;
        
        // turn horizontalAnchor and verticalAnchor into enum values of bfgAnchorLocation.
        // I can't find a good way to get an NS_ENUM value from a NSString. I'm going to use a giant ugly if/elif/else for now
        if ([nsHorizontalAnchor isEqualToString:@"TOP"]){
            horizontalEnum = (bfgAnchorLocation)TOP;
        }else if ([nsHorizontalAnchor isEqualToString:@"CENTER"]){
            horizontalEnum = (bfgAnchorLocation)CENTER;
        } else if ([nsHorizontalAnchor isEqualToString:@"BOTTOM"]){
            horizontalEnum = (bfgAnchorLocation)BOTTOM;
        } else if ([nsHorizontalAnchor isEqualToString:@"LEFT"]){
            horizontalEnum = (bfgAnchorLocation)LEFT;
        } else if ([nsHorizontalAnchor isEqualToString:@"RIGHT"]){
            horizontalEnum = (bfgAnchorLocation)RIGHT;
        } else {
            // If there isn't an enum match, return FALSE. The C# code will handle this and print out an Error log.
            return FALSE;
        }
        
        if ([nsVerticalAnchor isEqualToString:@"TOP"]){
            verticalEnum = (bfgAnchorLocation)TOP;
        }else if ([nsVerticalAnchor isEqualToString:@"CENTER"]){
            verticalEnum = (bfgAnchorLocation)CENTER;
        } else if ([nsVerticalAnchor isEqualToString:@"BOTTOM"]){
            verticalEnum = (bfgAnchorLocation)BOTTOM;
        } else if ([nsVerticalAnchor isEqualToString:@"LEFT"]){
            verticalEnum = (bfgAnchorLocation)LEFT;
        } else if ([nsVerticalAnchor isEqualToString:@"RIGHT"]){
            verticalEnum = (bfgAnchorLocation)RIGHT;
        } else {
            // If there isn't an enum match, return FALSE. The C# code will handle this and print out an Error log.
            return FALSE;
        }
        
        // Call createCcsButtonBounds with the NS_ENUM bfgAnchorLocation parameters
        CGRect buttonBounds = [bfgManager createCcsButtonBounds:widthPercent horizontalAnchor:horizontalEnum verticalAnchor:verticalEnum];
        
        // convert the buttonbounds into a dictionary
        int x = (int)buttonBounds.origin.x;
        int y = (int)buttonBounds.origin.y;
        int w = (int)buttonBounds.size.width;
        int h = (int)buttonBounds.size.height;
        NSDictionary * buttonBoundsDictionary = @{
                                                  @"x" : @(x),
                                                  @"y" : @(y),
                                                  @"width" : @(w),
                                                  @"height" : @(h)
                                                  };
        
        // Serialize the dictionary
        NSString* jsonBounds = convertJSONObjectToString(buttonBoundsDictionary);
        
        // pass the dictionary back as a reference, the C# code will deserialize the json buttonbounds dictionary
        copyStringToBuffer(jsonBounds, returnButtonBounds, (int)[jsonBounds length]+1);
        
        return TRUE;
    }
    
    void _hideCCSButton()
    {
        [bfgManager hideCCSButton];
    }
    
    BOOL _isShowingCCSButton()
    {
        return [bfgManager isShowingCCSButton];
    }
    
    void _showCcsButton(char* buttonBounds)
    {
        CGRect buttonRect = makeCGRect(buttonBounds);
        [bfgManager showCCSButtonForFrame:buttonRect];
    }
    
    void _showCcsButtonLocation(char* buttonBounds, char * gameLocation)
    {
        NSString *nsGameLocation = [NSString stringWithUTF8String:gameLocation];
        CGRect buttonRect = makeCGRect(buttonBounds);
        [bfgManager showCCSButtonForFrame:buttonRect gameLocation:nsGameLocation];
    }
    
    void _showCcsButtonLocationWithPercent(float xPercent, float yPercent, float widthPercent, float heightPercent, char * gameLocation)
    {
        NSString *nsGameLocation = [NSString stringWithUTF8String:gameLocation];
        CGFloat width = [UIScreen mainScreen].bounds.size.width;
        CGFloat height = [UIScreen mainScreen].bounds.size.height;
        
        CGRect buttonRect = CGRectMake(width*xPercent, height * yPercent, width * widthPercent, height * heightPercent);
        
        [bfgManager showCCSButtonForFrame:buttonRect gameLocation:nsGameLocation];
    }
    
    void _showCcsButtonLocationWithPixels(int x, int y, int width, int height, char * gameLocation)
    {
        CGFloat nativeScale = [UIScreen mainScreen].nativeScale;
        NSString *nsGameLocation = [NSString stringWithUTF8String:gameLocation];
        x = x / nativeScale;
        y = y / nativeScale;
        width = width / nativeScale;
        height = height / nativeScale;
              
        CGRect buttonRect = CGRectMake(x, y, width, height);
        
        [bfgManager showCCSButtonForFrame:buttonRect gameLocation:nsGameLocation];
    }
    
    
    //  Deprecated in  Big Fish iOS SDK 5.10
    /*
     BOOL _adsRunning()
     {
     return [bfgManager adsRunning];
     }
     
     BOOL _startAds()
     {
     return [bfgManager startAds:BFGADS_ORIGIN_BOTTOM];
     }
     
     void _stopAds()
     {
     [bfgManager stopAds];
     }
     */
    
    // Big Fish iOS SDK 5.7
    
    int _getDebugDictionary( char* jsonDebugDictionary, int size)
    {
        NSMutableDictionary *debugDict = [bfgManager debugDictionary];
        NSString *jsonString = convertJSONObjectToString(debugDict);
        
        NSUInteger jsonStringLength = [jsonString lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        if (size > jsonStringLength)
        {
            copyStringToBuffer(jsonString, jsonDebugDictionary, size);
        }
        return (int) jsonStringLength;
    }
    
    void _setDebugDictionary( const char* jsonDebugDictionary)
    {
        NSDictionary *inDebugDict = convertJSONtoDictionary(jsonDebugDictionary);
        if (!inDebugDict)
            return;
        
        NSMutableDictionary *debugDict = [bfgManager debugDictionary];
        if (!debugDict)
            return;
        
        [debugDict setDictionary:inDebugDict];
    }
    
    void _removePolicyListener()
    {
        [bfgManager removePolicyListener:[UnityWrapper sharedInstance]];
    }
    
    BOOL _didAcceptPolicyControl(char * policyControl)
    {
        NSString *nsPolicyControl = [NSString stringWithUTF8String:policyControl].uppercaseString;
        
        return [bfgManager didAcceptPolicyControl:nsPolicyControl];
    }
    void _setLimitEventAndDataUsage(BOOL limitData)
    {
        [FBSDKSettings setLimitEventAndDataUsage:limitData];
    }
}
