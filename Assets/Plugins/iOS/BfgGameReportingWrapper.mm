//
//  BfgGameReportingWrapper.mm
//  Unity-iPhone
//
//  Created by John Starin on 12/17/13.
//  Copyright 2013 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgGameReporting.h>
#import "BfgError.h"

@interface NSJSONSerialization (Expose)
+ (id)safeJSONObjectWithData:(NSData *)data options:(NSJSONReadingOptions)opt error:(NSError **)error;
@end

extern "C"
{
    void __bfgGameReporting__logPurchaseMainMenuClosed()
    {
        //Unused, depricated since SDK6.4 BFPremium
        //[bfgGameReporting logPurchaseMainMenuClosed];
    }

    void __bfgGameReporting__logPurchasePayWallClosed(const char* paywallID)
    {
        //Unused, depricated since SDK6.4 BFPremium
        //NSString *paywall = [NSString stringWithUTF8String:paywallID];
        //[bfgGameReporting logPurchasePayWallClosed:paywall];
    }

    void __bfgGameReporting__logGameHintRequested()
    {
        [bfgGameReporting logGameHintRequested];
    }

    void __bfgGameReporting__logCustomPlacement(const char* placementName)
    {
        NSString *placement = [NSString stringWithUTF8String:placementName];
        [bfgGameReporting logCustomPlacement:placement];
    }

    void __bfgGameReporting__preloadCustomPlacement(const char* placementName)
    {
        //Unused, depricated since SDK6.8
        //NSString *placement = [NSString stringWithUTF8String:placementName];
        //[bfgGameReporting preloadCustomPlacement:placement];
    }

    void __bfgGameReporting__logStrategyGuideRequested()
    {
        // Removed in 6.0
        //[bfgGameReporting logStrategyGuideRequested];
    }

    void __bfgGameReporting__logContentGateShown()
    {
        [bfgGameReporting logContentGateShown];
    }

    void __bfgGameReporting__logMainMenuShown()
    {
        [bfgGameReporting logMainMenuShown];
    }

    void __bfgGameReporting__logRateMenuCanceled()
    {
        [bfgGameReporting logRateMainMenuCanceled];
    }

    void __bfgGameReporting__logOptionsShown()
    {
        [bfgGameReporting logOptionsShown];
    }

    void __bfgGameReporting__logPurchaseMainMenuShown()
    {
        //Unused, depricated since SDK6.4 BFPremium
        //[bfgGameReporting logPurchaseMainMenuShown];
    }

    void __bfgGameReporting__logPurchasePayWallShown(const char* paywallID)
    {
        //Unused, depricated since SDK6.4 BFPremium
        //NSString *paywall = [NSString stringWithUTF8String:paywallID];
        //[bfgGameReporting logPurchasePayWallShown:paywall];
    }

    void __bfgGameReporting__logLevelStart(const char* levelID)
    {
        NSString *level = [NSString stringWithUTF8String:levelID];
        [bfgGameReporting logLevelStart:level];
    }

    void __bfgGameReporting__logLevelFinished(const char* levelID)
    {
        NSString *level = [NSString stringWithUTF8String:levelID];
        [bfgGameReporting logLevelFinished:level];
    }

    void __bfgGameReporting__logMiniGameStart(const char* miniGameID)
    {
        NSString *miniGame = [NSString stringWithUTF8String:miniGameID];
        [bfgGameReporting logMiniGameStart:miniGame];
    }

    void __bfgGameReporting__logMiniGameSkipped(const char* miniGameID)
    {
        NSString *miniGame = [NSString stringWithUTF8String:miniGameID];
        [bfgGameReporting logMiniGameSkipped:miniGame];
    }

    void __bfgGameReporting__logMiniGameFinished(const char* miniGameID)
    {
        NSString *miniGame = [NSString stringWithUTF8String:miniGameID];
        [bfgGameReporting logMiniGameFinished:miniGame];
    }

    void __bfgGameReporting__logAchievementEarned(const char* achievementID)
    {
        NSString *achievement = [NSString stringWithUTF8String:achievementID];
        [bfgGameReporting logAchievementEarned:achievement];
    }

    void __bfgGameReporting__logGameCompleted()
    {
        [bfgGameReporting logGameCompleted];
    }

    void __bfgGameReporting__logRateMainMenuCanceled()
    {
        [bfgGameReporting logRateMainMenuCanceled];
    }

    BOOL __bfgGameReporting__logCustomEvent(const char* name,
                                            const int value,
                                            const int level,
                                            const char* details1,
                                            const char* details2,
                                            const char* details3,
                                            const char* const* additionalDetailsKeys,
                                            const char* const* additionalDetailsValues,
                                            const int additionalDetailsCount)
    {
        NSString *ns_name = (name == nil || strlen(name) == 0) ? @"" : [NSString stringWithUTF8String:name];
        NSInteger ns_value = value;
        NSInteger ns_level = level;
        NSString *ns_details1 = (details1 == nil || strlen(details1) == 0) ? @"" : [NSString stringWithUTF8String:details1];
        NSString *ns_details2 = (details2 == nil || strlen(details2) == 0) ? @"" : [NSString stringWithUTF8String:details2];
        NSString *ns_details3 = (details3 == nil || strlen(details3) == 0) ? @"" : [NSString stringWithUTF8String:details3];

        NSMutableDictionary *ns_additionalDetails =[[NSMutableDictionary alloc] init];

        if (additionalDetailsKeys && additionalDetailsValues)
        {
            for (int i=0; i < additionalDetailsCount; ++i)
            {
                [ns_additionalDetails setObject:[NSString stringWithUTF8String:additionalDetailsValues[i]]
                                         forKey:[NSString stringWithUTF8String:additionalDetailsKeys[i]]];
            }
        }
        
        // Convert NSDictionary of additionalDetails to NSData, then perform safeJSON check on that NSData
        NSError *error;
        NSData * nsdata_additionalDetails = [NSJSONSerialization  dataWithJSONObject:ns_additionalDetails options:0 error:&error];
        NSDictionary * nsdictionary_nsdata = [NSJSONSerialization safeJSONObjectWithData:nsdata_additionalDetails options:0 error:&error];
        
        if (!nsdictionary_nsdata)
        {
            //NSString * nsExceptionWithName = @"INVALID JSON DETECTED IN: bfgGameReporting.cs - logCustomEvent()";
            //NSString * nsReason =  [NSString stringWithFormat:@"\nThe additionalDetails parameter:\n\"%@\" \nis an invalid JSON string.", ns_additionalDetails];
            //const char* exceptionWithName = [nsExceptionWithName UTF8String];
            //const char* reason = [nsReason UTF8String];
            
            [BfgError logErrorToCrashlytics:error];
        }

        return [bfgGameReporting logCustomEvent:ns_name
                                   value:ns_value
                                   level:ns_level
                                details1:ns_details1
                                details2:ns_details2
                                details3:ns_details3
                       additionalDetails:ns_additionalDetails];
    }
    
    
    // Logging a Custom Event from Serialized JSON additionalDetails
    BOOL __bfgGameReporting__logCustomEvent2(const char* name,
                                            const int value,
                                            const int level,
                                            const char* details1,
                                            const char* details2,
                                            const char* details3,
                                            const char* additionalDetails)
    {
        NSString *ns_name = (name == nil || strlen(name) == 0) ? @"" : [NSString stringWithUTF8String:name];
        NSInteger ns_value = value;
        NSInteger ns_level = level;
        NSString *ns_details1 = (details1 == nil || strlen(details1) == 0) ? @"" : [NSString stringWithUTF8String:details1];
        NSString *ns_details2 = (details2 == nil || strlen(details2) == 0) ? @"" : [NSString stringWithUTF8String:details2];
        NSString *ns_details3 = (details3 == nil || strlen(details3) == 0) ? @"" : [NSString stringWithUTF8String:details3];
        
        // Convert C string to NSString using UTF8 encoding
        NSString *nsstring_additionalDetails = (additionalDetails == nil || strlen(additionalDetails) == 0) ? @"" : [NSString stringWithUTF8String:additionalDetails];
        
        // Convert NSString to NSData, using UTF8 encoding
        NSData *nsdata_additionalDetails = [nsstring_additionalDetails dataUsingEncoding:NSUTF8StringEncoding];
        
        // Convert NSData to NSDictionary if the NSData is safe JSON object
        NSError *error;
        NSDictionary *nsdictionary_additionalDetails;
        nsdictionary_additionalDetails = [NSJSONSerialization safeJSONObjectWithData:nsdata_additionalDetails options:0 error:&error];
        if (!nsdictionary_additionalDetails)
        {
            //NSString * nsExceptionWithName = @"INVALID JSON DETECTED IN: bfgGameReporting.cs - logCustomEventSerialized()";
            //NSString * nsReason =  [NSString stringWithFormat:@"\nThe additionalDetails parameter:\n\"%@\" \nis an invalid JSON string.", nsstring_additionalDetails];
            //const char* exceptionWithName = [nsExceptionWithName UTF8String];
            //const char* reason = [nsReason UTF8String];
            
            [BfgError logErrorToCrashlytics:error];
        }
        
        return [bfgGameReporting logCustomEvent:ns_name
                                         value:ns_value
                                         level:ns_level
                                      details1:ns_details1
                                      details2:ns_details2
                                      details3:ns_details3
                             additionalDetails:nsdictionary_additionalDetails];
    }

    // Big Fish iOS SDK 5.7

    void __bfgGameReporting__logCustomEventImmediately(const char* name,
                                            const int value,
                                            const int level,
                                            const char* details1,
                                            const char* details2,
                                            const char* details3,
                                            const char* const* additionalDetailsKeys,
                                            const char* const* additionalDetailsValues,
                                            const int additionalDetailsCount)
    {
        NSString *ns_name = (name == nil || strlen(name) == 0) ? @"" : [NSString stringWithUTF8String:name];
        NSInteger ns_value = value;
        NSInteger ns_level = level;
        NSString *ns_details1 = (details1 == nil || strlen(details1) == 0) ? @"" : [NSString stringWithUTF8String:details1];
        NSString *ns_details2 = (details2 == nil || strlen(details2) == 0) ? @"" : [NSString stringWithUTF8String:details2];
        NSString *ns_details3 = (details3 == nil || strlen(details3) == 0) ? @"" : [NSString stringWithUTF8String:details3];

        NSMutableDictionary *ns_additionalDetails =[[NSMutableDictionary alloc] init];

        if (additionalDetailsKeys && additionalDetailsValues)
        {
            for (int i=0; i < additionalDetailsCount; ++i)
            {
                [ns_additionalDetails setObject:[NSString stringWithUTF8String:additionalDetailsValues[i]]
                                         forKey:[NSString stringWithUTF8String:additionalDetailsKeys[i]]];
            }
        }

        [bfgGameReporting logCustomEventImmediately:ns_name
                                   value:ns_value
                                   level:ns_level
                                details1:ns_details1
                                details2:ns_details2
                                details3:ns_details3
                       additionalDetails:ns_additionalDetails];
    }

    void __bfgGameReporting__logMobileAppTrackingCustomEvent(const char* name)
    {
        NSString *nsName = [NSString stringWithUTF8String:name];
        [bfgGameReporting logMobileAppTrackingCustomEvent:nsName];
    }

    void __bfgGameReporting__logSurveyEvent()
    {
        [bfgGameReporting logSurveyEvent];
    }

    // Big Fish iOS SDK 5.10

    void __bfgGameReporting__dismissVisiblePlacement()
    {
        [bfgGameReporting dismissVisiblePlacement];
    }

    void __bfgGameReporting__setSuppressPlacement(BOOL suppressPlacements)
    {
        [bfgGameReporting setSuppressPlacement:suppressPlacements];
    }
    
    void __bfgGameReporting__setLastLevelPlayed(const char * lastLevel)
    {
        NSString *nsLastLevel = [NSString stringWithUTF8String:lastLevel];
        [bfgGameReporting setLastLevelPlayed:nsLastLevel];
    }
    
    void __bfgGameReporting__setPlayerSpend(float playerSpend)
    {
        [bfgGameReporting setPlayerSpend:playerSpend];
    }
    
    void __bfgGameReporting__logRewardedVideoSeenWithProviderVideoLocation(const char * provider, const char* videoLocation)
    {
        NSString *nsProvider = [NSString stringWithUTF8String:provider];
        NSString *nsVideoLocation = [NSString stringWithUTF8String:videoLocation];
        [bfgGameReporting logRewardedVideoSeenWithProvider:nsProvider
                                             videoLocation:nsVideoLocation];
        
    }
    
    void __bfgGameReporting__logRewardedVideoSeenWithProvider(const char* provider)
    {
        NSString *nsProvider = [NSString stringWithUTF8String:provider];
        [bfgGameReporting logRewardedVideoSeenWithProvider:nsProvider];
    }
}
