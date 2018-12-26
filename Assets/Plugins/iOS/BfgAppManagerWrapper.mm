//
//  BfgAppManagerWrapper.mm
//  Unity-iPhone
//
//  Created by Anton Rivera on 8/4/14.
//  Copyright 2014 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgAppManager.h>

extern "C"
{
    BOOL __bfgAppManager__launchApp(const char* bundleIdentifier)
    {
        NSString *bundle = [NSString stringWithUTF8String:bundleIdentifier];
        return [bfgAppManager launchApp:bundle];
    }

    BOOL __bfgAppManager__launchAppWithParams(const char* bundleIdentifier, const char* parameterString)
    {
        NSString *bundle = [NSString stringWithUTF8String:bundleIdentifier];
        NSString *parameter = [NSString stringWithUTF8String:parameterString];
		return [bfgAppManager launchApp:bundle withParams:parameter];
	}

    BOOL __bfgAppManager__isAppInstalled(const char* bundleIdentifier)
    {
        NSError *error;
        NSString *bundle = [NSString stringWithUTF8String:bundleIdentifier];
		BOOL result = [bfgAppManager isAppInstalled:bundle error:&error];

        if (!result)
        {
            NSLog(@"isAppInstalled: %@", error);
        }

        return result;
	}

    void __bfgAppManager__launchStoreWithApp(const char* appID)
    {
        NSString *app = [NSString stringWithUTF8String:appID];
		[bfgAppManager launchStoreWithApp:app];
	}

    BOOL __bfgAppManager__isBigFishGamesAppInstalled()
    {
        NSError *error;
		BOOL result = [bfgAppManager isBigFishGamesAppInstalled:&error];

        if (!result)
        {
            NSLog(@"isBigFishGamesAppInstalled: %@", error);
        }

        return result;
	}

    void __bfgAppManager__launchStoreWithBigFishGamesApp()
    {
		[bfgAppManager launchStoreWithBigFishGamesApp];
	}

    BOOL __bfgAppManager__launchOrInstallBigFishGamesApp()
    {
		return [bfgAppManager launchOrInstallBigFishGamesApp];
	}

    BOOL __bfgAppManager__launchBigFishGamesAppStrategyGuideWithWrappingID(const char* wrappingID)
    {
        NSString *wrapping = [NSString stringWithUTF8String:wrappingID];
		return [bfgAppManager launchBigFishGamesAppStrategyGuideWithWrappingID:wrapping];
	}

    BOOL __bfgAppManager__launchBigFishGamesAppStrategyGuideWithWrappingIDChapterIndexPageIndex(const char* wrappingID, NSUInteger chapterIndex, NSUInteger pageIndex)
    {
        NSString *wrapping = [NSString stringWithUTF8String:wrappingID];
		return [bfgAppManager launchBigFishGamesAppStrategyGuideWithWrappingID:wrapping chapterIndex:chapterIndex pageIndex:pageIndex];
	}

    BOOL __bfgAppManager__openReferralURL(const char* url)
    {
        NSString *urlString = [NSString stringWithUTF8String:url];
        NSURL *newURL = [[NSURL alloc] initWithString:urlString];
		return [bfgAppManager openReferralURL:newURL];
	}

    void __bfgAppManager__cancelCurrentReferral()
    {
		return [bfgAppManager cancelCurrentReferral];
	}

    // Big Fish iOS SDK 5.10

    BOOL __bfgAppManager__launchBigFishGamesAppWithForum()
    {
        return [bfgAppManager launchBigFishGamesAppWithForum];
    }

    BOOL __bfgAppManager__launchBigFishGamesAppWithForumId(const char* id)
    {
        NSString *forumId = [NSString stringWithUTF8String:id];
        return [bfgAppManager launchBigFishGamesAppWithForum:forumId];
    }
}
