//
//  BfgRatingWrapper.mm
//  Unity-iPhone
//
//  Copyright 2013 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgRating.h>

extern "C"
{
    BOOL __bfgRating__canShowMainMenuRateButton()
    {
        return [bfgRating canShowMainMenuRateButton];
    }

	void __bfgRating__mainMenuGiveFeedback()
	{
		[bfgRating mainMenuGiveFeedback];
	}

	void __bfgRating__mainMenuRateApp()
	{
		[bfgRating mainMenuRateApp];
	}

	void __bfgRating__userDidSignificantEvent()
	{
		[bfgRating userDidSignificantEvent];
	}
}
