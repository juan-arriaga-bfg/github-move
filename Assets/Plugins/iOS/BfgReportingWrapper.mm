//
//  BfgReportingWrapper.mm
//  Unity-iPhone
//
//  Created by Michael Molien on 10/10/14.
//  Copyright 2013 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgReporting.h>

extern "C"
{
    void __bfgReporting__logSingleFireEvent(const char* name)
    {
        NSString *ns_name = [NSString stringWithUTF8String:name];
        [bfgReporting logSingleFireEvent:ns_name];
    }

    void __bfgReporting__logEvent(const char* name)
    {
        NSString *ns_name = [NSString stringWithUTF8String:name];
        [bfgReporting logEvent:ns_name];
    }


    void __bfgReporting__logEventWithParameters(
                                    const char* name,
                                    const char* const* additionalDetailsKeys,
                                    const char* const* additionalDetailsValues,
                                    const int additionalDetailsCount)
    {
        NSString *ns_name = [NSString stringWithUTF8String:name];
        NSMutableDictionary *ns_additionalDetails =[[NSMutableDictionary alloc] init];

        if (additionalDetailsKeys && additionalDetailsValues)
        {
            for (int i=0; i < additionalDetailsCount; ++i)
            {
                [ns_additionalDetails setObject:[NSString stringWithUTF8String:additionalDetailsValues[i]]
                                         forKey:[NSString stringWithUTF8String:additionalDetailsKeys[i]]];
            }
        }

        [bfgReporting logEvent:ns_name
                withParameters:ns_additionalDetails];
    }

    // Add any custom events here
}
